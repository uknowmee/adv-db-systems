FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

COPY App/Adv.Db.Systems.Importer/Adv.Db.Systems.Importer.csproj Adv.Db.Systems.Importer/
RUN dotnet restore Adv.Db.Systems.Importer/Adv.Db.Systems.Importer.csproj

COPY App/Adv.Db.Systems.Importer/ Adv.Db.Systems.Importer
RUN dotnet build Adv.Db.Systems.Importer/Adv.Db.Systems.Importer.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
RUN dotnet publish Adv.Db.Systems.Importer/Adv.Db.Systems.Importer.csproj -c $BUILD_CONFIGURATION -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sleep", "infinity"]
