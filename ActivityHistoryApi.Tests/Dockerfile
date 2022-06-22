FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

# Install and run sonar cloud scanner
RUN apt-get update && apt-get install -y openjdk-11-jdk
RUN dotnet tool install --global dotnet-sonarscanner --version 5.6.0
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_activity-history-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"

# Copy csproj and nuget config and restore as distinct layers
COPY ./ActivityHistoryApi.sln ./
COPY ./ActivityHistoryApi/ActivityHistoryApi.csproj ./ActivityHistoryApi/
COPY ./ActivityHistoryApi.Tests/ActivityHistoryApi.Tests.csproj ./ActivityHistoryApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ActivityHistoryApi/ActivityHistoryApi.csproj
RUN dotnet restore ./ActivityHistoryApi.Tests/ActivityHistoryApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out ActivityHistoryApi/ActivityHistoryApi.csproj
RUN dotnet build -c debug -o out ActivityHistoryApi.Tests/ActivityHistoryApi.Tests.csproj

CMD dotnet test
RUN dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
