FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'
ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN
WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config
COPY ./FinanceDataMigrationApi.sln ./
COPY ./FinanceDataMigrationApi/FinanceDataMigrationApi.csproj ./FinanceDataMigrationApi/
COPY ./FinanceDataMigrationApi.Tests/FinanceDataMigrationApi.Tests.csproj ./FinanceDataMigrationApi.Tests/

RUN dotnet restore ./FinanceDataMigrationApi/FinanceDataMigrationApi.csproj
RUN dotnet restore ./FinanceDataMigrationApi.Tests/FinanceDataMigrationApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out FinanceDataMigrationApi.Tests/FinanceDataMigrationApi.Tests.csproj

CMD dotnet test
