# Advanced Db Systems

#### [Docs](docs/Readme.md)

## Prerequisites

### Debian:

```
apt-get update && \
apt-get install -y wget && \
wget -qO packages-microsoft-prod.deb https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb && \
dpkg -i packages-microsoft-prod.deb && \
rm packages-microsoft-prod.deb && \
apt-get update && \
apt-get install -y dotnet-sdk-8.0 dotnet-runtime-8.0
```

### Arch:

```
pacman -Syu --noconfirm && \
pacman -S --noconfirm dotnet-sdk dotnet-runtime
```

## Startup

1. Start compose `docker compose -f compose.yaml up -d`
2. To run Db Importer use `dotnet run --project src/App/Adv.Db.Systems.Importer/`, or just `dotnet run` in specified project dir.
3. To run App use `dotnet run --project src/App/Adv.Db.Systems.App/`, or just `dotnet run` in specified project dir.

## Publish

1. To publish self-contained app with runtime use
    - linux: `dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish/linux`
    - win: `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/windows`
2. To publish linux AOT without runtime: `dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true -o publish/linux/aot`
