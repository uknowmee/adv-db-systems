# [Advanced Db Systems](../Readme.md) - docs

## Dataset

1. Taxonomy_iw.csv.gz: This file contains 5,771,611 records representing a directed graph where each line describes a relationship between a category and a subcategory.
    - "1880s_films","1889_films" indicates that the category "1880s_films" contains the subcategory "1889_films."
2. Popularity_iw.csv.gz: This file has 952,453 records that provide information about the popularity of categories.
    - "1889_films",34 indicates that the category "1889_films" has a popularity score of 34.

## Tasks:

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

