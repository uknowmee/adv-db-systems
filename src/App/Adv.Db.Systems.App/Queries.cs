namespace Adv.Db.Systems.App;

public static class Queries
{
    public static string ChildrenOfNode
        => """
           MATCH (:Category {name: $nodeName})-[:HAS_SUBCATEGORY]->(child:Category)
           RETURN child
           """;

    public static string ChildCount
        => """
           MATCH (:Category {name: $nodeName})-[:HAS_SUBCATEGORY]->(child:Category)
           RETURN count(child) AS childCount
           """;

    public static string Grandchildren
        => """
           MATCH (:Category {name: $nodeName})-[:HAS_SUBCATEGORY]->(:Category)-[:HAS_SUBCATEGORY]->(grandchild:Category)
           RETURN grandchild
           """;

    public static string Parents
        => """
           MATCH (parent:Category)-[:HAS_SUBCATEGORY]->(:Category {name: $nodeName})
           RETURN parent
           """;

    public static string ParentsCount
        => """
           MATCH (parent:Category)-[:HAS_SUBCATEGORY]->(:Category {name: $nodeName})
           RETURN count(parent) AS parentCount
           """;

    public static string Grandparents
        => """
           MATCH (grandparent:Category)-[:HAS_SUBCATEGORY]->(:Category)-[:HAS_SUBCATEGORY]->(:Category {name: $nodeName})
           RETURN grandparent
           """;

    public static string UniqueName
        => """
           MATCH (n:Category)
           RETURN count(DISTINCT n.name) AS uniqueNameCount
           """;

    public static string NotSubCategories
        => """
           MATCH (category:Category)
           WHERE NOT exists((:Category)-[:HAS_SUBCATEGORY]->(category))
           RETURN category
           """;

    public static string NotSubCategoriesCount
        => """
           MATCH (category:Category)
           WHERE NOT exists((:Category)-[:HAS_SUBCATEGORY]->(category))
           RETURN COUNT(category) AS notSubCategoriesCount
           """;

    public static string LargestNumberOfChildren
        => """
           MATCH (parent:Category)-[:HAS_SUBCATEGORY]->(child:Category)
           WITH parent, COUNT(child) AS childCount
           WITH childCount, COLLECT(parent) AS parents
           ORDER BY childCount DESC
           LIMIT $limit
           UNWIND parents AS parentWithMaxChildren
           RETURN parentWithMaxChildren, childCount
           """;

    public static string SmallestNumberOfChildren
        => """
           MATCH (parent:Category)-[:HAS_SUBCATEGORY]->(child:Category)
           WITH parent, COUNT(child) AS childCount
           WHERE childCount > 0
           WITH childCount, COLLECT(parent) AS parents
           ORDER BY childCount ASC
           LIMIT $limit
           UNWIND parents AS parentWithMinChildren
           RETURN parentWithMinChildren, childCount
           """;

    public static string ChangeName
        => """
           MATCH (category:Category {name: $oldNodeName})
           SET category.name = $newNodeName
           RETURN category
           """;

    public static string PopularityChange
        => """
           MATCH (n:Category {name: $nodeName})
           OPTIONAL MATCH (n)-[relationNodePopularity:HAS_POPULARITY]->(oldPopularity:Popularity)
           WITH n, oldPopularity, relationNodePopularity
           FOREACH (_ IN CASE WHEN relationNodePopularity IS NOT NULL THEN [1] ELSE [] END |
             DELETE relationNodePopularity
           )
           WITH n, oldPopularity
           OPTIONAL MATCH (otherNode:Category)-[:HAS_POPULARITY]->(oldPopularity:Popularity)
           WITH n, oldPopularity, COALESCE(oldPopularity.id, "NONE") AS oldPopularityId, COUNT(otherNode) AS relatedCount
           FOREACH (_ IN CASE WHEN oldPopularity IS NOT NULL AND relatedCount = 0 THEN [1] ELSE [] END |
             DELETE oldPopularity
           )
           MERGE (newPopularity:Popularity {id: $newNodePopularity})
           MERGE (n)-[:HAS_POPULARITY]->(newPopularity)
           RETURN n, oldPopularityId, newPopularity
           """;

    public static string AllPaths(int numberOfHops)
        => $$"""
             MATCH path=(:Category {name: $firstNodeName})-[relationships * ..{{numberOfHops}}]->(:Category {name: $secondNodeName})
             RETURN path, relationships
             """;

    public static string AllPathsCount(int numberOfHops)
        => $$"""
             MATCH path=(:Category {name: $firstNodeName})-[relationships * ..{{numberOfHops}}]->(:Category {name: $secondNodeName})
             UNWIND (nodes(path)) AS n
             RETURN count(DISTINCT(n)) AS differentNodes
             """;

    public static string NeighborhoodPopularity(int radius)
        => $$"""
             MATCH (n:Category {name: $nodeName})-[relations *1..{{radius}}]-(neighbor:Category)
             OPTIONAL MATCH (neighbor)-[rp:HAS_POPULARITY]->(p:Popularity)
             OPTIONAL MATCH (n)-[rnp:HAS_POPULARITY]->(np:Popularity)
             WITH DISTINCT n, neighbor, rp, p, rnp, np, COALESCE(ToInteger(p.id), 0) AS popularity, COALESCE(ToInteger(np.id), 0) AS node_popularity
             RETURN
               n.name AS node_name,
               node_popularity,
               COUNT(neighbor) AS neighbor_count,
               COLLECT(neighbor.name, popularity) AS neighbor_popularity_tuples,
               SUM(popularity) + node_popularity AS neighborhood_popularity
             """;

    public static string ShortestPathPopularity
        => """
           MATCH path=(n1:Category {name: $firstNodeName})-[:HAS_SUBCATEGORY *allShortest (r, n | 1)]->(n2:Category {name: $secondNodeName})
           WITH path, nodes(path) AS path_nodes
           UNWIND path_nodes AS n
           OPTIONAL MATCH (n)-[:HAS_POPULARITY]->(pop:Popularity)
           WITH path, path_nodes, SUM(COALESCE(TOINTEGER(pop.id), 0)) AS path_popularity
           RETURN path, path_popularity
           ORDER BY path_popularity ASC
           """;

    public static string DirectedPathWithHighestPopularity(int numberOfHops)
        => $$"""
             MATCH path=(n:Category {name: $firstNodeName})-[relationships:HAS_SUBCATEGORY *..{{numberOfHops}}]->(m:Category {name: $secondNodeName})
             WITH path, nodes(path) AS path_nodes
             UNWIND path_nodes AS node
             OPTIONAL MATCH (node)-[:HAS_POPULARITY]->(pop:Popularity)
             WITH path, SUM(COALESCE(TOINTEGER(pop.id), 0)) AS path_popularity
             RETURN path, path_popularity
             ORDER BY path_popularity DESC
             LIMIT $limit
             """;
}