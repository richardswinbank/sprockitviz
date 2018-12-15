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
    public Node Start { get; private set; }
    public Node End { get; set; }

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

  public class DirectedPath : DirectedEdge
  {
    public DirectedPath(Node start, Node end) : base(start, end)
    {
    }
  }

}