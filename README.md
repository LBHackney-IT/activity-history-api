# Activity History API

The Activity History API is the London Borough of Hackney (LBH) **MTFH** service that exposes the
**activity/audit history for a target entity**. Other platform services publish domain events describing what
changed; this API returns the recorded history for a given `targetId`, newest first, with pagination.

It is a read-only, DynamoDB-backed API.

## Stack

- **.NET 10** (`net10.0`) — ASP.NET Core Web API hosted on **AWS Lambda** via `Amazon.Lambda.AspNetCoreServer`
  (the managed `dotnet10` runtime).
- **DynamoDB** as the datastore (via `Hackney.Core.DynamoDb` / `Hackney.Shared.ActivityHistory`).
- **Swashbuckle** for Swagger/OpenAPI, **FluentValidation** for request validation.
- **MVC API versioning** (URL segment reader) and `System.Text.Json` with `JsonStringEnumConverter`.
- **AWS X-Ray** for tracing.
- **xUnit** + `TestStack.BDDfy` + FluentAssertions/Moq/AutoFixture for tests.

## Endpoints

Versioning uses `UrlSegmentApiVersionReader` with `DefaultApiVersion = 1.0` and
`AssumeDefaultVersionWhenUnspecified = true`. Only v1 exists.

| Verb | Route | Request | Response |
| --- | --- | --- | --- |
| GET | `/api/v1/activityHistory` | Query: `targetId` (**required**, `Guid`), `paginationToken` (optional), `pageSize` (optional) | `200` → `PagedResult<ActivityHistoryResponseObject>`: `{ "results": [ ... ], "paginationDetails": { "hasNext": bool, "nextToken": string\|null } }`. `404` when no history exists for the target, `400` on model validation failure, `500` on error. |
| GET | `/api/v1/healthcheck/ping` | — | `200` → health report written by `HealthCheckResponseWriter` (unversioned; mapped outside MVC). Includes a DynamoDB health check. |
| GET | `/swagger` | — | Swagger UI / OpenAPI document, one document per API version. |

An activity history record contains `Id`, `TargetId`, `TargetType`, `Type`, `CreatedAt`,
`AuthorDetails { FullName, Email }`, `NewData`, `OldData` and `SourceDomain`.

Serialisation uses MVC's System.Text.Json with web defaults (camelCase) plus a `JsonStringEnumConverter`, so
enum values are emitted as strings. All endpoints sit behind the API Gateway request authorizer and API key.

## Data sources and external dependencies

| Dependency | Detail |
| --- | --- |
| DynamoDB table `ActivityHistory` | `targetId` (HASH) + `id` (RANGE), local secondary index `ActivityHistoryByCreatedAt` (range `createdAt`), TTL attribute `TimetoLiveForRecord`, `PAY_PER_REQUEST` billing, point-in-time recovery enabled. Defined identically in every environment under `terraform/*/dynamodb.tf`. |
| DynamoDB access | `dynamodb:*` granted on `table/ActivityHistory` and `table/ActivityHistory/index/*` (`serverless.yml`). |
| Local DynamoDB | `amazon/dynamodb-local` on port 8000 (`docker-compose.yml`), with `DynamoDb_LocalMode=true` and `DynamoDb_LocalServiceUrl=http://localhost:8000` (`appsettings.DevelopmentLocal.json`). |
| AWS region | `eu-west-2`. |
| AWS X-Ray | `AWSSDKHandler.RegisterXRayForAllServices()`, `AWSXRayRecorder.InitializeInstance/RegisterLogger`, `app.UseXRay("activity-history-api")`; `AWSXRayDaemonWriteAccess` on the Lambda role. |
| AWS SSM Parameter Store | `/housing-tl/{stage}/aspnetcore-environment`; CloudWatch alarm topic ARN. |
| S3 | Only the Terraform remote-state buckets — the application itself does not use S3. `database.env` (Postgres credentials) is a stale template leftover: this service has no SQL database. |

## Setup

### Prerequisites

1. [Docker][docker-download] with the **`docker compose`** v2 plugin.
2. A **`LBHPACKAGESTOKEN`** with `read:packages` access to the `LBHackney-IT` GitHub Packages feed — the
   `Hackney.*` packages are not published to nuget.org:
   ```sh
   export LBHPACKAGESTOKEN=<token>
   ```
3. Optionally copy [`.env.sample`](.env.sample) to `.env` and set `LBHPACKAGESTOKEN` there instead of
   exporting it — `docker compose` reads `.env` for the values referenced in `docker-compose.yml`. The file
   also lists every environment variable the API reads at runtime; those are **not** loaded from `.env`, so
   export them (or add them to the compose service) if you need to override the defaults.

### Build and run

```sh
git clone git@github.com:LBHackney-IT/activity-history-api.git
cd activity-history-api
make build     # docker compose build activity-history-api
make serve     # build and start the API plus a local DynamoDB (port 3000)
make shell     # bash inside the API container
```

`make test-database`-style helpers are not available; the local DynamoDB service is named
`dynamodb-database` in `docker-compose.yml`.

## Testing

```sh
make test
```

`make test` builds the test image (`ActivityHistoryApi.Tests/Dockerfile`) and runs the xUnit suite inside it
against the compose-provided DynamoDB instance.

The suite covers the controller/use case/gateway units plus BDDfy E2E stories that boot the real HTTP pipeline
through `Microsoft.AspNetCore.Mvc.Testing`'s `WebApplicationFactory` and assert the DynamoDB-backed responses
and the health endpoint.

### Agreed testing approach

- xUnit with `TestStack.BDDfy` for E2E stories, FluentAssertions, Moq and AutoFixture.
- Tests should be independent of each other.
- Gateway tests interact with a real (containerised) DynamoDB instance.
- Unit and E2E tests run in CI.
- Test coverage should never go down.

## Deployment

CircleCI (`.circleci/config.yml`) gates deployment on a code-formatting check (`dotnet-format --check`), the
Docker test suite and SonarCloud analysis, then deploys infrastructure with **Terraform** and the Lambda with
the **Serverless Framework**. Changes are reviewed and approved on a branch before merging.

**Stages and triggers:**

- **Any branch** — formatting check, tests, and a Terraform plan plus compliance pass against development.
  Nothing is applied or deployed.
- **`master`** — after the checks pass, Terraform applies to development and the function is deployed to
  **development**.
- **`release`** — tests, then Terraform and deployment to **staging**. Production follows only after the
  `permit-production-terraform-release` and `permit-production-release` manual approvals.
- **`release`** — the separate `deploy-terraform-pre-production` and `deploy-code-pre-production` workflows
  deploy **pre-production**, each behind its own manual approval.

**Packaging and runtime:**

- `build.sh` installs `Amazon.Lambda.Tools` and runs
  `dotnet lambda package --configuration release --framework net10.0 --output-package ./bin/release/net10.0/activity-history-api.zip`.
  (`build.cmd` is the Windows equivalent for local use.)
- The deploy job runs `npx --yes serverless@^4 deploy --stage <stage> --conceal`.
- `serverless.yml` declares `runtime: dotnet10`, consumes that artifact, and exposes `ANY /{proxy+}` plus
  `GET /swagger/{proxy+}` behind the request authorizer, with the `Platform_APIs_Web_ACL` WAF attached.

## Contacts

**Housing product team** — [mmh-project-team@hackney.gov.uk](mailto:mmh-project-team@hackney.gov.uk)

[docker-download]: https://www.docker.com/products/docker-desktop
[AWS-CLI]: https://aws.amazon.com/cli/
