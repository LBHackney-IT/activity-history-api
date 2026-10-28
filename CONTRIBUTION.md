# Contributing to the Activity History API

This document covers local setup, build and run, testing, and the development/deployment workflow for the
Activity History API. For an overview of the service, its endpoints and dependencies, see the
[README](README.md).

## Prerequisites

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

## Build and run

```sh
git clone git@github.com:LBHackney-IT/activity-history-api.git
cd activity-history-api
make build     # docker compose build activity-history-api
make serve     # build and start the API plus a local DynamoDB (port 3000)
make shell     # bash inside the API container
```

`make serve` starts the API on <http://localhost:3000> together with the `dynamodb-database` service
(`amazon/dynamodb-local`, port 8000). Swagger UI is available at `/swagger`; the health check is at
`/api/v1/healthcheck/ping`.

Running the API outside Docker requires a local DynamoDB on port 8000 and the runtime variables listed in
[`.env.sample`](.env.sample) (`DynamoDb_LocalMode`, `DynamoDb_LocalServiceUrl`, `AWS_REGION`, dummy AWS
credentials), with `ASPNETCORE_ENVIRONMENT=DevelopmentLocal` to load `appsettings.DevelopmentLocal.json`.

## Testing

```sh
make test
```

`make test` builds the test image ([`ActivityHistoryApi.Tests/Dockerfile`](ActivityHistoryApi.Tests/Dockerfile))
and runs the xUnit suite inside it against the compose-provided DynamoDB instance. The image also runs the
SonarCloud scanner (requires `SONAR_TOKEN` when run in CI).

The suite covers the controller/use case/gateway units plus BDDfy E2E stories that boot the real HTTP pipeline
through `Microsoft.AspNetCore.Mvc.Testing`'s `WebApplicationFactory` and assert the DynamoDB-backed responses
and the health endpoint.

### Agreed testing approach

- xUnit with `TestStack.BDDfy` for E2E stories, FluentAssertions, Moq and AutoFixture.
- Tests must be independent of each other.
- Gateway tests interact with a real (containerised) DynamoDB instance.
- Unit and E2E tests run in CI.
- Test coverage should never go down.

## Development workflow

- **Formatting** — code must pass `dotnet-format --check`; run `make lint` (or `dotnet format`) to fix
  formatting locally before pushing. CI fails on formatting drift.
- **Branches and review** — work on a feature branch and open a pull request; changes are reviewed and
  approved before merging. The PR template asks for a JIRA link, a description of the problem and the
  solution, a review checklist, and any follow-up actions.
- **Static analysis** — SonarCloud (`sonar-project.properties`) runs as part of the build-and-test job.
- **Secret scanning** — the repository uses `ggshield` via [pre-commit](.pre-commit-config.yaml); install it
  with `pre-commit install` if you want the hook locally.
- **Infrastructure changes** — environment configuration lives under [`terraform/`](terraform) (one folder
  per environment) and is applied by CI.

### CI/CD and deployment

CircleCI ([`.circleci/config.yml`](.circleci/config.yml), visualised in
[`docs/circle_ci_workflow.png`](docs/circle_ci_workflow.png)) gates deployment on a code-formatting check
(`dotnet-format --check`), the Docker test suite and SonarCloud analysis, then deploys infrastructure with
**Terraform** and the Lambda with the **Serverless Framework**.

**Stages and triggers:**

- **Any branch** — formatting check, tests, and a Terraform plan plus compliance pass against development.
  Nothing is applied or deployed.
- **`master`** — after the checks pass, Terraform applies to development and the function is deployed to
  **development**.
- **`release`** — tests, then Terraform and deployment to **staging**. Production follows only after the
  `permit-production-terraform-release` and `permit-production-release` manual approvals.

**Packaging and runtime:**

- [`ActivityHistoryApi/build.sh`](ActivityHistoryApi/build.sh) installs `Amazon.Lambda.Tools` and runs
  `dotnet lambda package --configuration release --framework net10.0 --output-package ./bin/release/net10.0/activity-history-api.zip`.
  (`build.cmd` is the Windows equivalent for local use.)
- The deploy job runs `npx --yes serverless@^4 deploy --stage <stage> --conceal`.
- [`serverless.yml`](ActivityHistoryApi/serverless.yml) declares `runtime: dotnet10`, consumes that artifact,
  and exposes `ANY /{proxy+}` behind the request authorizer plus a public `GET /swagger/{proxy+}`, with the
  `Platform_APIs_Web_ACL` WAF attached.

[docker-download]: https://www.docker.com/products/docker-desktop
