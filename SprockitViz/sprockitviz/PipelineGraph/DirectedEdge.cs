using System;

namespace FireFive.PipelineVisualiser.PipelineGraph
{
  /*
   * DirectedEdge class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Class representing an edge in a directed graph.
   */
  public class DirectedEdge
  {
    // the node at the start of the edge
    public Node Start { get; private set; }

    // the node at the end of the edge
    public Node End { get; set; }

    // instantiate an edge between two non-null nodes
    public DirectedEdge(Node start, Node end)
    {
      if (start == null) throw new Exception("Edge start node cannot be null");
      this.Start = start;
      if (end == null) throw new Exception("Edge end node cannot be null");
      this.End = end;
    }

    public override string ToString()
    {
      return "Edge[" + Start.Id + "->" + End.Id + "]";
    }
  }

  // class representing a path (used in subgraphs)
  public class DirectedPath : DirectedEdge
  {
    public DirectedPath(Node start, Node end) : base(start, end)
    {
    }
  }

}