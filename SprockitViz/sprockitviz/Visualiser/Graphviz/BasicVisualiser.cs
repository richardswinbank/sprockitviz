using FireFive.PipelineVisualiser.PipelineGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireFive.PipelineVisualiser.Visualiser.Graphviz
{
  /*
   * BasicVisualiser class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Style-specific implementation of GraphvizVisualiser -- provides an implementation of 
   * GetDotScript() using a set of display conventions specific to this implementation.
   */
  class BasicVisualiser : GraphvizVisualiser
  {
    public BasicVisualiser(IGraphvizSettings settings) : base(settings)
    {
    }

    public override string GetDotScript(Graph g)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("digraph \"" + g.Name + "\" {");
      sb.AppendLine("  node[shape=\"box\",fontname=\"helvetica\"];");

      foreach (Node n in g.Nodes)
        sb.AppendLine("  " + n.Id + " [label=\"" + GetLabel(n)
          + "\",tooltip=\"" + GetTooltip(n)
          + "\",style=\"" + GetNodeStyle(n, g) + ",rounded"
          + "\",fontcolor=\"" + GetFontColour(n)
          + "\",href=\"" + n.LongName + ".svg"
          + (g.IsCentre(n) ? "\",fillcolor=\"gold" : "")
          + "\",penwidth=\"" + GetPenWidth(n) + "\"];");

      foreach (DirectedEdge e in g.Edges)
        sb.AppendLine("  " + e.Start.Id + " -> " + e.End.Id + " [style=\"" + GetEdgeStyle(e) + "\"];");

      sb.AppendLine("}");
      return sb.ToString();
    }

    private string GetEdgeStyle(DirectedEdge e)
    {
      if (e is DirectedPath)
        return "dashed";
      return "solid";
    }

    private string GetFontColour(Node n)
    {
      if (n.Type == NodeType.SsisPackage)
        return "red";
      return GetDbColor(n);
    }

    private string GetLabel(Node n)
    {
      string suffix = "";
      switch (n.Type)
      {
        case NodeType.ScalarFunction:
        case NodeType.TableValuedFunction:
          suffix = "()";
          break;
      }
      return n.ShortName + suffix;
    }

    private string GetNodeStyle(Node n, Graph context)
    {
      string style = "solid";

      switch (n.Type)
      {
        case NodeType.View:
        case NodeType.ScalarFunction:
          style = "dashed";
          break;
        case NodeType.Unknown:
          style = "dotted";
          break;
      }

      if (context.IsCentre(n))
        style += ",filled";

      return style;
    }

    private float GetPenWidth(Node n)
    {
      switch (n.Type)
      {
        case NodeType.StoredProcedure:
        case NodeType.SsisPackage:
          return 2;
        default:
          return 1;
      }
    }

    private string GetTooltip(Node n)
    {
      var duration = GetDurationDescription(n);
      return n.LongName + "&#13;&#10;" + GetTypeDescription(n.Type) + " (" + (duration.Length > 0 ? duration + ", " : "") + GetIdDescription(n.Id) + ")";
    }

  }
}
