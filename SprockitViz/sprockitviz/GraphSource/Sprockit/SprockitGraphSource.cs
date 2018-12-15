using FireFive.PipelineVisualiser.PipelineGraph;
using System;
using System.Data;
using System.Data.SqlClient;

namespace FireFive.PipelineVisualiser.GraphSource.Sprockit
{
  /*
   * SprockitGraphSource class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/sprockitviz
   *
   * Implementation of AbstractGraphSource for Sprockit ETL pipelines.
   */
  public abstract class SprockitGraphSource : AbstractGraphSource
  {
    private ISprockitSettings settings;

    public SprockitGraphSource(ISprockitSettings settings)
    {
      this.settings = settings;
    }

    public override Graph GetGraph()
    {
      Graph graph = new Graph();
      using (SqlConnection conn = new SqlConnection(settings.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand(GetNodeQuery(), conn);
        conn.Open();
        using (SqlDataReader rdr = cmd.ExecuteReader())
        {
          var schema = rdr.GetSchemaTable().Rows;
          //foreach (DataRow col in schema)
          //  Console.WriteLine(col["ColumnName"]);

          while (rdr.Read())
          {
            Node n = new Node((string)rdr["NodeId"]);
            foreach (DataRow col in schema)
            {
              string columnName = (string)col["ColumnName"];
              switch (columnName)
              {
                case "NodeId":
                  break;
                case "ShortName":
                  n.ShortName = (string)rdr["ShortName"];
                  break;
                case "LongName":
                  n.LongName = (string)rdr["LongName"];
                  break;
                case "DbName":
                  n.DbName = (string)rdr["DbName"];
                  break;
                case "ObjectType":
                  n.Type = GetNodeType(rdr["ObjectType"] == DBNull.Value ? "" : (string)rdr["ObjectType"]);
                  break;
                default:
                  if(rdr[columnName] != DBNull.Value)
                    n.SetProperty(columnName, rdr[columnName].ToString());
                  break;

              }
            }
            graph.AddNode(n);
          }
        }
      }

      using (SqlConnection conn = new SqlConnection(settings.ConnectionString))
      {
        SqlCommand cmd = new SqlCommand(GetEdgeQuery(), conn);
        conn.Open();
        using (SqlDataReader rdr = cmd.ExecuteReader())
          while (rdr.Read())
            graph.AddEdge((string)rdr["StartNodeId"], (string)rdr["EndNodeId"]);
      }

      return graph;
    }

    private NodeType GetNodeType(string objectType)
    {
      switch (objectType)
      {
        case "P":
          return NodeType.StoredProcedure;
        case "V":
          return NodeType.View;
        case "IF":
          return NodeType.TableValuedFunction;
        case "FN":
          return NodeType.ScalarFunction;
        case "SSIS":
          return NodeType.SsisPackage;
        case "U":
          return NodeType.Table;
        default:
          return NodeType.Unknown;
      }
    }

    public abstract string GetNodeQuery();

    public abstract string GetEdgeQuery();

  }
}
