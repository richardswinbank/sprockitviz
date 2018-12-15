using System;
using System.Collections.Generic;

namespace FireFive.PipelineVisualiser.PipelineGraph
{
  /*
   * Node class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Class representing a node in an ETL pipeline graph.
   */
  public class Node : PropertyBag<string>
  {
    public Node (string id)
    {
      if (id == null || id.Length == 0)
        throw new Exception("Node ID must be non-null and non-zero in length");
      Id = id;
    }

    public string Id { get; private set; }
    public string ShortName { get; set; }
    public string LongName { get; set; }
    public NodeType Type { get; set; }
    public string DbName { get; set; }

    public override string ToString()
    {
      return "Node[Id=" + Id + ",LongName=" + LongName + ",Type=" + Type;
    }

    internal bool LeadsTo(Node end, List<Edge> edges)
    {
      foreach (Edge e in edges)
        if (e.Start == this && (e.End == end || e.End.LeadsTo(end, edges)))
          return true;
      return false;
    }

    internal bool HasParent(List<Node> nodes, List<Edge> edges)
    {
      foreach (Edge e in edges)
        if (e.End == this && nodes.Contains(e.Start))
          return true;
      return false;
    }
  }
}