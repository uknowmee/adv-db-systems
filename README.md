# Advanced Db Systems

#### [Docs](docs/README.md)

---

## Requirements and dependencies

- [docker compose](https://docs.docker.com/desktop/install/linux/)
- if you are not working with released version u will need sdk to build app and runtime to run it. Check [wiki.archlinux.org](https://wiki.archlinux.org/title/.NET)
  or [learn.microsoft.com](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install?tabs=dotnet8&pivots=os-linux-ubuntu-2404).
    - dotnet sdk
    - dotnet runtime

## Installation and configuration instructions

### Run from source

1. To run `memgraph-compose` use `docker compose -f compose.yaml up -d`
2. To run `Adv.Db.Systems.Importer` use `dotnet run --project src/App/Adv.Db.Systems.Importer/`, or just `dotnet run` in specified project dir.
3. To run `Adv.Db.Systems.App` use `dotnet run --project src/App/Adv.Db.Systems.App/`, or just `dotnet run` in specified project dir.

### Released versions

1. If u run released version of `dbimporter` you should specify path to data dir. [Check data README.md](data/README.md).
2. If u run released version of `dbcli` you should specify args according to [dbcli MAN](docs/README.md#dbcli-man).

### Publish

1. To publish self-contained app with runtime use (you can skip self-contained and single-file for performance)
    - linux: `dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o publish/linux`
    - win: `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/windows`

### Benchmarks

1. To run benchmarks start `docker compose -f .\compose-benchmarks.yaml up -d`
2. Then modify script [benchmark.sh](src/benchmarks/benchmark.sh) according to your needs
3. Move results to new dir and run [results.ipynb](src/benchmarks/results.ipynb) to see results

---

Authors: [Piotr Sokolowski](https://github.com/sokoloowski), [Michal Palucki](https://github.com/uknowmee)