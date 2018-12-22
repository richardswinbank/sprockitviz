using FireFive.PipelineVisualiser.PipelineGraph;
using System.Text;

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
    // create a new instance with a specified configuration
    public BasicVisualiser(IGraphvizSettings settings) : base(settings)
    {
    }

    #region GraphvizVisualiser implementation

    // return a DOT script for a specified graph
    public override string GetDotScript(Graph g)
    {
      StringBuilder sb = new StringBuilder();

      // graph header
      sb.AppendLine("digraph " + Enquote(g.Name) + " {");
      sb.AppendLine("  node[shape=box,fontname=helvetica];");

      // add nodes
      foreach (Node n in g.Nodes)
        sb.AppendLine("  " + Enquote(n.Id)
          + " [label=" + Enquote(GetLabel(n))
          + ",tooltip=" + Enquote(GetTooltip(n))
          + ",style=" + Enquote(GetNodeStyle(n, g) + ",rounded")
          + ",fontcolor=" + Enquote(GetFontColor(n))
          + ",href=" + Enquote(n.LongName + ".svg")
          + (g.IsCentre(n) ? ",fillcolor=gold" : "")
          + ",penwidth=" + GetPenWidth(n)
          + "];");

      // add edges
      foreach (DirectedEdge e in g.Edges)
        sb.AppendLine("  " + Enquote(e.Start.Id) + " -> " + Enquote(e.End.Id) 
          + " [style=" + Enquote(GetEdgeStyle(e)) + "];");

      // graph closing brace
      sb.AppendLine("}");

      return sb.ToString();
    }

    #endregion GraphvizVisualiser implementation

    #region Helper functions

    // return Graphviz edge style for a specified edge
    private string GetEdgeStyle(DirectedEdge e)
    {
      if (e is DirectedPath)
        return "dashed";
      return "solid";
    }

    // return Graphviz font color for a specified node
    private string GetFontColor(Node n)
    {
      if (n.Type == DbObjectType.SsisPackage)
        return "red";
      return GetDbColor(n);
    }

    // return display text for a specified node
    private string GetLabel(Node n)
    {
      string suffix = "";
      switch (n.Type)
      {
        case DbObjectType.ScalarFunction:
        case DbObjectType.TableValuedFunction:
          suffix = "()";
          break;
      }
      return n.ShortName + suffix;
    }

    // return Graphviz style for a specified node (in the context of a specific graph)
    private string GetNodeStyle(Node n, Graph context)
    {
      string style = "solid";

      switch (n.Type)
      {
        case DbObjectType.View:
        case DbObjectType.ScalarFunction:
          style = "dashed";
          break;
        case DbObjectType.Unknown:
          style = "dotted";
          break;
      }

      if (context.IsCentre(n))
        style += ",filled";

      return style;
    }

    // return Graphviz pen width for a specified node's outline
    private float GetPenWidth(Node n)
    {
      switch (n.Type)
      {
        case DbObjectType.StoredProcedure:
        case DbObjectType.SsisPackage:
          return 2;
        default:
          return 1;
      }
    }

    // return tooltip text for a specified node
    private string GetTooltip(Node n)
    {
      var duration = GetDurationDescription(n);
      return n.LongName + "&#13;&#10;" + GetTypeDescription(n.Type) + " (" + (duration.Length > 0 ? duration + ", " : "") + GetIdDescription(n.Id) + ")";
    }

    // surround a string with double quotes
    private string Enquote(string s)
    {
      return "\"" + s + "\"";
    }

    #endregion Helper functions
  }
}
