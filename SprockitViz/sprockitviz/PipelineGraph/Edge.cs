﻿using System;

namespace FireFive.PipelineVisualiser.PipelineGraph
{
  /*
   * Edge class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Class representing an edge in a directed graph.
   */
  public class Edge
  {
    public Node Start { get; private set; }
    public Node End { get; set; }

    public Edge(Node start, Node end)
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

  public class IndirectEdge : Edge
  {
    public IndirectEdge(Node start, Node end) : base(start, end)
    {
    }
  }

}