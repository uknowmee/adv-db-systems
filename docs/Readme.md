# [Advanced Db Systems](../Readme.md) - docs

## Dataset

1. Taxonomy_iw.csv.gz: This file contains 5,771,611 records representing a directed graph where each line describes a relationship between a category and a subcategory.
    - "1880s_films","1889_films" indicates that the category "1880s_films" contains the subcategory "1889_films."
2. Popularity_iw.csv.gz: This file has 952,453 records that provide information about the popularity of categories.
    - "1889_films",34 indicates that the category "1889_films" has a popularity score of 34.

## Initial tasks

1. Finds all children of a given node.
2. Counts all children of a given node.
3. Finds all grandchildren of a given node.
4. Finds all parents of a given node.
5. Counts all parents of a given node.
6. Finds all grandparents of a given node.
7. Counts how many nodes have unique names.
8. Finds nodes that are not subcategories of any other node.
9. Counts nodes for purpose 8.
10. Finds nodes with the largest number of children; there may be more than one.
11. Finds nodes with the smallest number of children (the number of children is greater than zero).
12. Changes the name of a given node.
13. Changes the popularity of a given node.
14. Finds all paths between two specified nodes, with edges directed from the first to the second node.
15. Counts nodes for purpose 14.
16. Calculates popularity in the neighborhood of a node at a specified radius; the parameters are the node's name and the neighborhood radius; neighborhood popularity is
    the sum of the popularity of the given node and all nodes belonging to the neighborhood.
17. Calculates popularity on the shortest path between two given nodes, according to the direction; popularity on the shortest path is the sum of the popularity of all
    nodes located on the shortest path.
18. Finds the directed path between two nodes with the highest popularity among all paths between those nodes.

## Technology

- [docker compose](https://docs.docker.com/compose/)
- [memgraph](https://memgraph.com/docs)
- [.net 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [python](https://www.python.org/)

## Architecture

<img src="assets/architecture.png" alt="popularity-affected" width="600" />

## dbcli MAN

1. `dbcli 1 <node_name>` for example -> `"1880s_films"`
2. `dbcli 2 <node_name>`
3. `dbcli 3 <node_name>`
4. `dbcli 4 <node_name>`
5. `dbcli 5 <node_name>`
6. `dbcli 6 <node_name>`
7. `dbcli 7`
8. `dbcli 8`
9. `dbcli 9`
10. `dbcli 10 <limit>` default limit -> `1`
11. `dbcli 11 <limit>` default limit -> `1`
12. `dbcli 12 <old_node_name> <new_node_name>`
13. `dbcli 13 <node_name> <new_node_popularity>`
14. `dbcli 14 <first_node_name> <second_node_name> <n_of_hops>` Return all the results when the path is equal to or shorter than given number of hops. Try: `15`.
15. `dbcli 15 <first_node_name> <second_node_name> <n_of_hops>`
16. `dbcli 16 <node_name> <radius>` Radius is basically number of hops. Try: `1`.
17. `dbcli 17 <first_node_name> <second_node_name>`
18. `dbcli 18 <first_node_name> <second_node_name> <n_of_hops>`


## Results

- [results](results.md)

## Self-assessment

How can u grade the implementation?

- what quantities should be measured?
- which queries should be used?
- what should be the specific values of query parameters?
- should something else be measured in addition to queries?

## Strategies for future mitigation of identified shortcomings

- change mamgraph data import from csv one to cypherl
- introduce lb to handle more tasks in one time
- not unique memgraph db ids
- 

## Changelog

- experiments
- implement db importer
- update readme
- 

---

## Incorrect Data Format

- [popularity_iw.csv fix](popularity.md)

## Compressed Data Folder

- [README.md](../data/README.md)