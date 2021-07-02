FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./ActivityHistoryApi.sln ./
COPY ./ActivityHistoryApi/ActivityHistoryApi.csproj ./ActivityHistoryApi/
COPY ./ActivityHistoryApi.Tests/ActivityHistoryApi.Tests.csproj ./ActivityHistoryApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ActivityHistoryApi/ActivityHistoryApi.csproj
RUN dotnet restore ./ActivityHistoryApi.Tests/ActivityHistoryApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out ActivityHistoryApi.Tests/ActivityHistoryApi.Tests.csproj

CMD dotnet test
