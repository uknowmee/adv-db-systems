# Advanced Db Systems

#### [Docs](docs/Readme.md)

## Project

1. To run use `dotnet run --project .\src\App\Adv.Db.Systems.App\Adv.Db.Systems.App.csproj`, or just `dotnet run` in project dir. 
2. To publish self-contained app with runtime use
    - linux: `dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish/linux`
    - win: `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/windows`
3. To publish linux AOT without runtime: `dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true -o publish/linux/aot`

## Runtime 

- just install `dotnet-runtime-8.0`

### Debian:

```
wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-runtime-8.0
```
