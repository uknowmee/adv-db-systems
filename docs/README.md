# [Advanced Db Systems](../README.md) - docs

## Dataset

1. Taxonomy_iw.csv.gz: This file contains 5,771,611 records representing a directed graph where each line describes a relationship between a category and a subcategory.
    - `"1880s_films","1889_films"` indicates that the category `"1880s_films"` contains the subcategory `"1889_films."`
2. Popularity_iw.csv.gz: This file has 952,453 records that provide information about the popularity of categories.
    - `"1889_films",34` indicates that the category `"1889_films"` has a popularity score of `34`.

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

**RUNNING QUERIES WILL SAVE EXTENDED INFO TO `5f602e2f-ef92-46fa-9fe1-b865163f7a9a` IN YOUR TMP DIR**

1. `dbcli 1 <node_name>` For example -> `"1880s_films"`. Returns list of sub-category names for given category.
2. `dbcli 2 <node_name>` Returns count of sub-categories for given category.
3. `dbcli 3 <node_name>` Returns list of sub-category names for given category's sub-categories.
4. `dbcli 4 <node_name>` Returns list of parent category names for given category.
5. `dbcli 5 <node_name>` Returns count of parent categories for given category.
6. `dbcli 6 <node_name>` Returns list of parent category names for given category's parent categories.
7. `dbcli 7` Returns count of unique category names.
8. `dbcli 8` Returns list of category names that are not sub-categories of any other category.
9. `dbcli 9` Returns count of category names that are not sub-categories of any other category.
10. `dbcli 10 <limit>` Default `<limit>` -> `1` Returns list of tuples: `(category_name, n_of_children)` for categories with the largest number of sub-categories.
11. `dbcli 11 <limit>` Default `<limit>` -> `1` Returns list of tuples: `(category_name, n_of_children)` for categories with the smallest number of sub-categories.
12. `dbcli 12 <old_node_name> <new_node_name>` Returns new category name.
13. `dbcli 13 <node_name> <new_node_popularity>` Returns tuple `(category_name, old_popularity, new_popularity)` for given category
14. `dbcli 14 <first_node_name> <second_node_name> <n_of_hops>` Default `<n_of_hops>` -> `10` Returns list of paths (as category names) between two categories.
15. `dbcli 15 <first_node_name> <second_node_name> <n_of_hops>` Default `<n_of_hops>` -> `10` Returns count of different nodes existing on paths between two categories.
16. `dbcli 16 <node_name> <radius>` Radius is basically number of hops. Default: `<radius>` -> `1`. Returns: category name, category popularity, neighbor count,
    popularity of neighbor categories, neighbor popularity
17. `dbcli 17 <first_node_name> <second_node_name>` Returns popularity on shortest paths / paths.
18. `dbcli 18 <first_node_name> <second_node_name> <n_of_hops> <limit>` Default `<n_of_hops>` -> `15`, `<limit>` -> `1` (You can specify either none or all params).
    Returns path / list of paths with the highest.
    popularity

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
- add unique to memgraph db ids
- standardize names across db queries

## Changelog

- experiments
- implement db importer
- update readme
- implement queries
- optimize cli results
- update readme

---

## Incorrect Data Format

- [popularity_iw.csv fix](popularity.md)

## Compressed Data Folder

- [README.md](../data/README.md)