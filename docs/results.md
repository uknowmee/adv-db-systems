# [Advanced Db Systems docs](Readme.md) - results

## To achieve similar results you should:

- follow detailed instructions in [README.md](../README.md)
- up compose
- run dbimporter
- run dbcli with belows parameters

---

1. Finds all children of a given node.
    - Cli: `dbcli 1 "Universities_and_colleges_in_the_United_States_by_athletic_conference"`
2. Counts all children of a given node.
    - Cli: `dbcli 2 "Universities_and_colleges_in_the_United_States_by_athletic_conference"`
3. Finds all grandchildren of a given node.
    - Cli: `dbcli 3 "Universities_and_colleges_in_the_United_States_by_athletic_conference"`
4. Finds all parents of a given node.
    - Cli: `dbcli 4 "Srebrenica_massacre"`
5. Counts all parents of a given node.
    - Cli: `dbcli 5 "Srebrenica_massacre"`
6. Finds all grandparents of a given node.
    - Cli: `dbcli 6 "Srebrenica_massacre"`
7. Counts how many nodes have unique names.
    - Cli: `dbcli 7`
8. Finds nodes that are not subcategories of any other node.
    - Cli: `dbcli 8`
9. Counts nodes for purpose 8.
    - Cli: `dbcli 9`
10. Finds nodes with the largest number of children; there may be more than one.
    - Cli: `dbcli 10 10`
11. Finds nodes with the smallest number of children (the number of children is greater than zero).
    - Cli: `dbcli 11 10`
12. Changes the name of a given node.
    - Cli: `dbcli 12 "Universities_and_colleges_in_the_United_States_by_athletic_conference", "New Very Long Default Name"`
13. Changes the popularity of a given node.
    - Cli: `dbcli 13 "Universities_and_colleges_in_the_United_States_by_athletic_conference" 999999999`
14. Finds all paths between two specified nodes, with edges directed from the first to the second node.
    - Cli: `dbcli 14 "19th-century_works" "1880s_films" 15`
15. Counts nodes for purpose 14.
    - Cli: `dbcli 15 "19th-century_works" "1880s_films" 15`
16. Calculates popularity in the neighborhood of a node at a specified radius; the parameters are the node's name and the neighborhood radius; neighborhood popularity is
    the sum of the popularity of the given node and all nodes belonging to the neighborhood.
    - Cli: `dbcli 16 "Tourism_in_Uttarakhand" 1`
17. Calculates popularity on the shortest path between two given nodes, according to the direction; popularity on the shortest path is the sum of the popularity of all
    nodes located on the shortest path.
    - Cli: `dbcli 17 "19th-century_works" "1887_directorial_debut_films"`
18. Finds the directed path between two nodes with the highest popularity among all paths between those nodes.
    - Cli: `dbcli 18 "19th-century_works" "1887_directorial_debut_films" 15 50`
