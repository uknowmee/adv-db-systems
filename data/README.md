# [Advanced Db Systems docs](../docs/README.md) - data

This is the default folder where data that will be imported to db should be stored.
Importer will unpack all data directly from the compressed dir.
When running released version of `dbimporter` the path to `adv-db-systems/data` dir should be specified in args.

- `uknowme@DESKTOP-DJJJ3DV:../publish/linux$ ./dbimporter /mnt/d/projects/uknowmee/adv-db-systems/data`

```
.
├── compressed
│   ├── popularity_iw.csv.gz
│   └── taxonomy_iw.csv.gz
└── README.md
```
