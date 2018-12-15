using FireFive.PipelineVisualiser.PipelineGraph;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FireFive.PipelineVisualiser.Visualiser.Graphviz
{
  /*
   * GraphvizVisualiser class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Implementation of AbstractVisualiser as wrapper for Graphviz application.
   */
  public abstract class GraphvizVisualiser : AbstractVisualiser
  {
    private IGraphvizSettings settings;

    public GraphvizVisualiser(IGraphvizSettings settings)
    {
      this.settings = settings;
    }

    private StringBuilder stdOut;
    private StringBuilder stdErr;

    public override void Visualise(Graph g)
    {
      string style = "svg";
      string algorithm = "dot";

      string outputFolder = settings.OutputFolder;
      if (!Directory.Exists(outputFolder))
        Directory.CreateDirectory(outputFolder);

      string ufFile = outputFolder + "\\" + g.GetName() + ".uf";
      string dotFile = outputFolder + "\\" + g.GetName() + ".gv";
      string svgFile = outputFolder + "\\" + g.GetName() + ".svg";

      // get Graphviz input
      string gvInputScript = GetDotScript(g);
      File.WriteAllText(dotFile, gvInputScript);

      // unflatten?
      int maxLeafStagger = GetMaxLeafStagger(g);
      if(maxLeafStagger > 0)
      {
        File.WriteAllText(ufFile, gvInputScript);

        // call unflatten
        Process uf = new Process();
        uf.StartInfo.CreateNoWindow = true;
        uf.StartInfo.UseShellExecute = false;
        uf.StartInfo.FileName = settings.GraphvizAppFolder + @"\unflatten.exe";
        if (!File.Exists(uf.StartInfo.FileName))
          throw new VisualiserConfigurationException("Graphviz executable " + uf.StartInfo.FileName + " not found.");
        uf.StartInfo.Arguments = "-fl" + maxLeafStagger  + " \"" + ufFile + "\"";

        uf.StartInfo.RedirectStandardOutput = true;
        stdOut = new StringBuilder();
        uf.OutputDataReceived += StdOutDataReceived;

        uf.StartInfo.RedirectStandardError = true;
        stdErr = new StringBuilder();
        uf.ErrorDataReceived += StdErrDataReceived;

        uf.Start();
        uf.BeginOutputReadLine();
        uf.BeginErrorReadLine();
        if (!uf.WaitForExit(5000))
        {
          uf.Kill();
          uf.WaitForExit();
        }
        if (uf.ExitCode == 0)
          File.WriteAllText(dotFile, stdOut.ToString());
      }

      // call dot
      Process dot = new Process();
      dot.StartInfo.CreateNoWindow = true;
      dot.StartInfo.UseShellExecute = false;
      dot.StartInfo.FileName = settings.GraphvizAppFolder + @"\dot.exe";
      if (!File.Exists(dot.StartInfo.FileName))
        throw new VisualiserConfigurationException("Graphviz executable " + dot.StartInfo.FileName + " not found.");
      dot.StartInfo.Arguments = "-T" + style + " -K" + algorithm + " -o \"" + svgFile + "\" \"" + dotFile + "\"";

      dot.StartInfo.RedirectStandardError = true;
      stdErr = new StringBuilder();
      dot.ErrorDataReceived += StdErrDataReceived;

      dot.Start();
      dot.BeginErrorReadLine();
      if (!dot.WaitForExit(5000))
      {
        dot.Kill();
        dot.WaitForExit();
        RaiseGraphvizRenderingException(svgFile, dotFile);
      }
      if(dot.ExitCode != 0)
        RaiseGraphvizRenderingException(svgFile, dotFile);

      // tidy up
      try
      {
        if (settings.DeleteWorkingFiles)
        {
          File.Delete(dotFile);
          File.Delete(ufFile);
        }
      }
      catch { }
    }

    // Calculate an appropriate maxLeafStagger value for Graphviz's unflatten program. Basically:
    //  - if a graph is too wide for the configured MaxSize, aim to unflatten it to make it narrow enough 
    //  - if a graph is too wide *and* too tall, aim to unflatten it into something approaching the same aspect ratio as MaxSize
    private int GetMaxLeafStagger(Graph g)
    {
      Size size = g.GetSize();
      float overSize = (float)size.Width / settings.MaxSize.Width;

      if (overSize <= 1)  // not too wide
        return 0;

      int maxLeafStagger = 0;
      string trace = size.ToString() + "; match ";

      if (overSize * size.Height > settings.MaxSize.Height)
      // compressing to MaxWidth wouldn't fit on the page -- try to match aspect ratio
      {
        float density = (float)g.Nodes.Count / (size.Width * size.Height);
        density = 1;
        float targetCount = settings.MaxSize.Width * settings.MaxSize.Height * density;
        float targetWidth = g.Nodes.Count / targetCount * settings.MaxSize.Width;

        maxLeafStagger = (int)((g.Nodes.Count + 1) / targetWidth);
        trace += "aspect ratio";
      }
      else // try to compress to MaxWidth
      {
        maxLeafStagger = (g.Nodes.Count + 1) / settings.MaxSize.Width;
        trace += "max width";
      }

      trace += "; maxLeafStagger = " + maxLeafStagger;
      if(settings.Verbose)
        Console.WriteLine(trace);

      return maxLeafStagger;
    }

      private void RaiseGraphvizRenderingException(string svgFile, string dotFile)
    {
      throw new VisualiserRenderingException("Graphviz error rendering " + svgFile
        + " from " + dotFile + ": " + stdErr.ToString());
    }

    private void StdOutDataReceived(object sender, DataReceivedEventArgs e)
    {
      stdOut.AppendLine(e.Data);
    }

    private void StdErrDataReceived(object sender, DataReceivedEventArgs e)
    {
      stdErr.AppendLine(e.Data);
    }

    public abstract string GetDotScript(Graph g);

    protected string GetDurationDescription(Node n)
    {
      if (!n.HasProperty("AvgDuration"))
        return "";
      int duration = 0;
      int.TryParse(n.GetProperty("AvgDuration"), out duration);
      TimeSpan t = TimeSpan.FromSeconds(duration);
      return "Duration = " + string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
    }

    protected string GetIdDescription(string id)
    {
      if (id[0] == 'R')
        return "ResourceUid = " + id.Substring(1, id.Length - 1);
      return "ProcessId = " + id.Substring(1, id.Length - 1);
    }

    protected string GetTypeDescription(NodeType type)
    {
      switch (type)
      {
        case NodeType.StoredProcedure:
          return "Stored procedure";
        case NodeType.SsisPackage:
          return "SSIS package";
        case NodeType.ScalarFunction:
          return "Scalar function";
        case NodeType.TableValuedFunction:
          return "Table-valued function";
        default:
          return type.ToString();
      }
    }

    protected string GetDbColor(Node n)
    {
      if(settings.DbColors.ContainsKey(n.DbName))
          return settings.DbColors[n.DbName];
      return "black";
    }
  }
}
