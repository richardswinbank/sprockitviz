namespace FireFive.PipelineVisualiser.GraphSource.Sprockit
{
  class CCSprockitGraph : SprockitGraphSource
  {
    /*
     * CCSprockitGraph class
     * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
     * http://richardswinbank.net/sprockitviz
     *
     * Implementation of SprockitGraphSource providing a graph combining ETL
     * processes and resources, eliminating stored-query process nodes.
     */

    // create a new instance with a specified configuration
    public CCSprockitGraph(ISprockitSettings settings) : base(settings)
    {
    }

    // Return SQL query to retrieve list of nodes for the ETL pipeline graph.
    // For this implementation that means:
    //  - unique list of configured resources 
    //  - configured processes *except* stored queries used to enforce stored query 
    //    input dependendencies (these appear in the list of conigured resources)
    public override string GetNodeQuery()
    {
      return @"
SELECT 
  'P' + CAST(p.ProcessId AS VARCHAR) AS NodeId
, CASE
    WHEN p.ProcessType = 'SSIS' THEN ProcessName
    WHEN p.IsStoredQuery = 1 AND p.ProcessType != 'V' THEN p.SchemaName + '.' + p.ProcessName + '()'
    ELSE p.SchemaName + '.' + p.ProcessName 
  END AS ShortName
, p.FqProcessName AS LongName
, p.DbName
, ProcessType AS ObjectType
, CASE
    WHEN ProcessType IN ('SSIS', 'P') THEN AvgDuration
  END AvgDuration
FROM sprockit.uvw_Process p
  LEFT JOIN sprockit.uvw_Resource sq 
    ON sq.ProcessId = p.ProcessId
    AND sq.IsInput = 0
    AND sq.IsStoredQuery = 1
    AND sq.FqProcessName = sq.FqResourceName
WHERE sq.ProcessId IS NULL  -- exclude stored-query processes

UNION  -- ensures ResourceUid unique in next SELECT

SELECT 
  'R' + CAST(ResourceUid AS VARCHAR) AS NodeId
, SchemaName + '.' + ResourceName AS ShortName
, FqResourceName AS LongName
, DbName
, ResourceType AS ObjectType
, NULL
FROM sprockit.uvw_Resource
";
    }

    // Return SQL query to retrieve list of edges for the ETL pipeline graph.
    // For this implementation that means:
    //   - the list of edges from resources to processes (process inputs)
    //   - the list of edges from processed to resources (process outputs), unless the 
    //     process is a stored query used to enforce stored query input dependendencies.  
    //     These are replaced by a direct edge from the input resource to the output.
    public override string GetEdgeQuery()
    {
      return @"
SELECT DISTINCT
  CASE r.IsInput
    WHEN 1 THEN 'R' + CAST(r.ResourceUid AS VARCHAR)
    ELSE 'P' + CAST(r.ProcessId AS VARCHAR)
  END AS StartNodeId
, CASE r.IsInput
    WHEN 1 THEN
      COALESCE(
      'R' + CAST(sq.ResourceUid AS VARCHAR) 
    , 'P' + CAST(r.ProcessId AS VARCHAR)
    )
    ELSE 'R' + CAST(r.ResourceUid AS VARCHAR) 
  END AS EndNodeId 
FROM sprockit.uvw_Resource r 
  LEFT JOIN sprockit.uvw_Resource sq 
    ON sq.ProcessId = r.ProcessId
    AND sq.IsInput = 0
    AND sq.IsStoredQuery = 1
    AND sq.FqProcessName = sq.FqResourceName
WHERE r.IsInput = 1
OR (r.IsInput = 0 AND sq.ResourceId IS NULL)
";
    }
  }
}
