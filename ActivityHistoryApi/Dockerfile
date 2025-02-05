FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./ActivityHistoryApi/ActivityHistoryApi.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./ActivityHistoryApi.csproj

# Copy everything else and build
COPY ./ActivityHistoryApi ./

RUN dotnet build -c Release -o out ./ActivityHistoryApi.csproj
RUN dotnet publish -c Release -o out ./ActivityHistoryApi.csproj

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/ActivityHistoryApi.dll
