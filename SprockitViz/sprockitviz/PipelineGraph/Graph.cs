using System;
using System.Collections.Generic;
using System.Text;

namespace FireFive.PipelineVisualiser.PipelineGraph
{
  /*
   * Graph class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Class representing an ETL pipeline graph. 
   * 
   * ETL pipelines are directed, acyclic graphs (DAGs). This implementation is inherently directed 
   * because its edges are directed, but it does not explicitly prevent the introduction of cycles.
   * An instance of this class containing cycles would be error-prone (e.g. the recursive GetSize()
   * method would fail to terminate).
   */
  public class Graph
  {
    private Node centre;

    public List<Node> Nodes { get; private set; }
    public List<DirectedEdge> Edges { get; private set; }

    public Graph()
    {
      Nodes = new List<Node>();
      Edges = new List<DirectedEdge>();
    }

    public Graph(Node centre) : this()
    {
      AddNode(centre);
      this.centre = centre;
    }

    public bool IsCentre(Node n)
    {
      return n == centre;
    }

    public string Name
    {
      get
      {
        if (centre == null)
          return "Pipeline";
        return centre.LongName;
      }
    }

    public Size GetSize()
    {
      List<Node> nodes = new List<Node>();
      foreach (Node n in Nodes)
        nodes.Add(n);
      Size size = Size.Empty;
      GetSize(nodes, size);
      return size;
    }

    private void GetSize(List<Node> nodes, Size size)
    {
      if (nodes.Count == 0)
        return;

      List<Node> roots = Roots(nodes);
      size.Width = roots.Count > size.Width ? roots.Count : size.Width;
      size.Height += 1;

      foreach (Node root in roots)
        nodes.Remove(root);
      GetSize(nodes, size);
    }
    
    private List<Node> Roots(List<Node> nodes)
    {
      List<Node> roots = new List<Node>();
      foreach (Node n in nodes)
        if (!n.HasParent(nodes, Edges))
          roots.Add(n);
      return roots;
    }

    internal Graph Subgraph(Node centre, int radius)
    {
      var subgraph = new Graph(centre);
      subgraph.AddAncestors(centre, radius, this);
      subgraph.AddDescendants(centre, radius, this);

      foreach (DirectedEdge e in Edges)
        if (subgraph.Contains(e.Start) && subgraph.Contains(e.End))
          subgraph.Edges.Add(e);

      foreach(Node start in subgraph.Nodes)
        foreach(Node end in subgraph.Nodes)
          if(start.LeadsTo(end, Edges) && ! start.LeadsTo(end, subgraph.Edges))
            subgraph.Edges.Add(new DirectedPath(start, end));

      for(int i=subgraph.Edges.Count - 1; i>=0; i--)
        if(subgraph.Edges[i] is DirectedPath)
        {
          DirectedPath ie = (DirectedPath)subgraph.Edges[i];
          subgraph.Edges.Remove(ie);
          if (!ie.Start.LeadsTo(ie.End, subgraph.Edges))
            subgraph.Edges.Add(ie);
        }

      return subgraph;
    }

    private void AddAncestors(Node child, int radius, Graph context)
    {
      if (radius <= 0)
        return;
      foreach (DirectedEdge e in context.Edges)
      {
        if (e.End == child && !Contains(e.Start))
        {
          AddNode(e.Start);
          AddAncestors(e.Start, radius - 1, context);
        }
      }
    }

    private void AddDescendants(Node parent, int radius, Graph context)
    {
      if (radius <= 0)
        return;
      foreach (DirectedEdge e in context.Edges)
      {
        if (e.Start == parent && !Contains(e.End))
        {
          AddNode(e.End);
          AddDescendants(e.End, radius - 1, context);
        }
      }
    }

    private bool Contains(Node newNode)
    {
      foreach (Node n in Nodes)
        if (n == newNode)
          return true;
      return false;
    }

    internal void AddNode(Node node)
    {
      foreach (Node n in Nodes)
        if (n.Id == node.Id)
          throw new Exception("Node Id " + n.Id + " is already present");
      Nodes.Add(node);
    }

    internal void AddEdge(string startId, string endId)
    {
      Node start = null;
      foreach (Node n in Nodes)
        if (n.Id == startId)
          start = n;

      Node end = null;
      foreach (Node n in Nodes)
        if (n.Id == endId)
          end = n;

      Edges.Add(new DirectedEdge(start, end));
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();

      foreach (Node n in Nodes)
        sb.AppendLine(n.ToString());
      foreach (DirectedEdge n in Edges)
        sb.AppendLine(n.ToString());

      return sb.ToString();
    }
  }
}