﻿using FireFive.PipelineVisualiser.PipelineGraph;
using System;
using System.Data;
using System.Data.SqlClient;

namespace FireFive.PipelineVisualiser.GraphSource.Sprockit
{
   /*
    * SprockitGraphSource class
    * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
    * http://richardswinbank.net/sprockitviz
    *
    * Implementation of AbstractGraphSource for Sprockit ETL pipelines.
    */
   public abstract class SprockitGraphSource : AbstractGraphSource
   {
      // configuration for this graph source
      private ISprockitSettings settings;

      // create a new instance with a specified configuration
      public SprockitGraphSource(ISprockitSettings settings)
      {
         this.settings = settings;
      }

      public override Graph GetGraph()
      {
         // instantiate an empty graph
         var builder = new SqlConnectionStringBuilder(settings.ConnectionString);
         string name = builder.DataSource + (builder.InitialCatalog == string.Empty ? "" : "[" + builder.InitialCatalog + "]");
         Graph graph = new Graph(name);

         // use the SQL query returned from GetNodeQuery() to construct the graph's nodes
         using (SqlConnection conn = new SqlConnection(settings.ConnectionString))
         {
            SqlCommand cmd = new SqlCommand(GetNodeQuery(), conn);
            conn.Open();
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
               var schema = rdr.GetSchemaTable().Rows;

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
                           n.ShortName = (string)rdr[columnName];
                           break;
                        case "LongName":
                           n.LongName = (string)rdr[columnName];
                           break;
                        case "DbName":
                           n.DbName = (string)rdr[columnName];
                           break;
                        case "ObjectType":
                           n.Type = GetNodeType(rdr[columnName] == DBNull.Value ? "" : (string)rdr[columnName]);
                           break;
                        case "AvgDuration":
                           if (rdr[columnName] != DBNull.Value)
                           {
                              n.Weight = (int)rdr[columnName];
                              n.SetProperty(columnName, rdr[columnName].ToString());
                           }
                           break;
                        default:
                           if (rdr[columnName] != DBNull.Value)
                              n.SetProperty(columnName, rdr[columnName].ToString());
                           break;
                     }
                  }
                  graph.AddNode(n);
               }
            }
         }

         // use the SQL query returned from GetEdgeQuery() to construct the graph's edges
         using (SqlConnection conn = new SqlConnection(settings.ConnectionString))
         {
            SqlCommand cmd = new SqlCommand(GetEdgeQuery(), conn);
            conn.Open();
            using (SqlDataReader rdr = cmd.ExecuteReader())
               while (rdr.Read())
                  graph.AddEdge((string)rdr["StartNodeId"], (string)rdr["EndNodeId"]);
         }

         // return the graph
         return graph;
      }

      // translate SQL Server database object types to DbObjectType enum members
      private DbObjectType GetNodeType(string objectType)
      {
         switch (objectType)
         {
            case "P":
               return DbObjectType.StoredProcedure;
            case "V":
               return DbObjectType.View;
            case "TF":
            case "IF":
               return DbObjectType.TableValuedFunction;
            case "FN":
               return DbObjectType.ScalarFunction;
            case "SSIS":
               return DbObjectType.SsisPackage;
            case "U":
               return DbObjectType.Table;
            default:
               return DbObjectType.Unknown;
         }
      }

      // Return SQL query to retrieve list of nodes for the ETL pipeline graph.
      public abstract string GetNodeQuery();

      // Return SQL query to retrieve list of edges for the ETL pipeline graph.
      public abstract string GetEdgeQuery();

   }
}
