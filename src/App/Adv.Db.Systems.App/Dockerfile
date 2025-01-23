FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ARG BUILD_CONFIGURATION=Release

COPY App/Adv.Db.Systems.App/Adv.Db.Systems.App.csproj Adv.Db.Systems.App/
RUN dotnet restore Adv.Db.Systems.App/Adv.Db.Systems.App.csproj

COPY App/Adv.Db.Systems.App/ Adv.Db.Systems.App
RUN dotnet build Adv.Db.Systems.App/Adv.Db.Systems.App.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
RUN dotnet publish Adv.Db.Systems.App/Adv.Db.Systems.App.csproj -c $BUILD_CONFIGURATION -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sleep", "infinity"]
