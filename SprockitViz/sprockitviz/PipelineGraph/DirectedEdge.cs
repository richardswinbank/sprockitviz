﻿using System;

namespace FireFive.PipelineVisualiser.PipelineGraph
{
   /*
    * DirectedEdge class
    * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
    * http://richardswinbank.net/
    *
    * Class representing an edge in a directed graph.
    */
   public class DirectedEdge : IEquatable<DirectedEdge>
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

      // implementation of IEquatable<DirectedEdge>
      public bool Equals(DirectedEdge that)
      {
         return this.Start == that.Start && this.End == that.End;
      }

      // method implementation to support use of DirectedEdge as Dictionary key
      public override bool Equals(object obj)
      {
         return Equals((DirectedEdge)obj);
      }

      public override int GetHashCode()
      {
         return Start.GetHashCode() * 486187739 + End.GetHashCode();
      }
   }

   // class to represent a path by its endpoints (used in subgraphs)
   public class DirectedConnection : DirectedEdge
   {
      public DirectedConnection(Node start, Node end) : base(start, end)
      {
      }
   }

}