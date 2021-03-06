﻿using FireFive.PipelineVisualiser.PipelineGraph;
using FireFive.PipelineVisualiser.Visualiser;
using FireFive.PipelineVisualiser.Visualiser.Graphviz;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace FireFive.PipelineVisualiser.SprockitViz
{
  /*
   * SprockitViz class
   * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/sprockitviz
   *
   * Main class to run the sprockitviz application
   */
  class SprockitViz
  {
    static void Main(string[] args)
    {
      if (args.Length == 0)
        args = new string[] { "_testing_201901061708" };  // just for testing in VS

      Console.WriteLine(@"
  ****************************************************************************
  *  Sprockit Pipeline Visualiser                                            *
  *  Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net)  *
  *  http://richardswinbank.net/sprockitviz                                  *
  ****************************************************************************
");
      Thread.Sleep(1500);
      SprockitVizSettings settings = null;
      try
      {
        if (args.Length != 1)
          throw new Exception("usage: sprockvit[.exe] <instanceAlias>");

        // set up the config
        settings = SprockitVizSettings.Settings;
        settings.SetContext(args[0]);
        new SprockitViz().Run(settings);
      }
      catch (Exception e)
      {
        Console.WriteLine("ERROR: " + e.Message);
        if (settings == null || settings.Verbose)
          Console.WriteLine(e);  // dump whole stack trace
      }
    }

    // build visualisations for an ETL pipeline graph using config info from SprockitVizSettings
    private void Run(SprockitVizSettings settings)
    {
      // copy CSS & JS files if required
      if (settings.OutputFormat == GraphvizOutputFormat.Html || settings.OutputFormat == GraphvizOutputFormat.App)
      {
        string fileName = settings.OutputFolder + "\\" + settings.HtmlStyleSheet;
        string fileContents = File.ReadAllText(settings.HtmlStyleSheet);
        File.WriteAllText(fileName, fileContents);

        fileName = settings.OutputFolder + "\\" + settings.JavaScriptFile;
        fileContents = File.ReadAllText(settings.JavaScriptFile);
        File.WriteAllText(fileName, fileContents);

        if(settings.OutputFormat == GraphvizOutputFormat.App)
        {
          fileName = settings.OutputFolder + "\\" + settings.HtmlAppFile;
          fileContents = File.ReadAllText(settings.HtmlAppFile);
          File.WriteAllText(fileName, fileContents);
        }
      }

      // get the graph
      var graph = settings.GraphSource.GetGraph();
      try
      {
        // visualise the whole thing (without failing if an error occurs)
        settings.Visualiser.Visualise(graph);
      }
      catch (VisualiserRenderingException e)
      {
        Console.WriteLine(" ERROR: " + e.Message);
      }

      // visualise the critical path
      settings.Visualiser.Visualise(graph.CriticalPath());

      // visualise the subgraph around each node in the graph
      int i = 0;
      StringBuilder nodeNames = new StringBuilder();
      foreach (Node n in graph.Nodes)
      {
        i++;
        nodeNames.AppendLine(", \"" + n.LongName + "\"");

        Console.WriteLine("Drawing subgraph " + i + " of " + graph.NodeCount + " (" + n.LongName + ")");
        try
        {
          // find the subgraph
          int radius = settings.SubgraphRadius > 0 ? settings.SubgraphRadius : 1;
          var subgraph = graph.Subgraph(n, radius);

          if (settings.SubgraphRadius == 0)  // find "best fit" radius heuristically
          {
            // build subgraphs with successively large radii until either 
            // the result is too big or it contains the entire graph
            while (true)
            {
              radius++;
              var biggerSubgraph = graph.Subgraph(n, radius);
              var size = biggerSubgraph.GetSize();
              if (size.Width > settings.MaxSize.Width || size.Height > settings.MaxSize.Height
                || biggerSubgraph.NodeCount <= subgraph.NodeCount)
                break;
              subgraph = biggerSubgraph;
            }
          }

          // visualise the subgraph
          settings.Visualiser.Visualise(subgraph);
        }
        catch (VisualiserRenderingException e)
        {
          Console.WriteLine(" ERROR: " + e.Message);
        }
      }

      if (settings.OutputFormat == GraphvizOutputFormat.App)
        File.WriteAllText(settings.OutputFolder + @"\_sprockitNodes.js", 
          @"function getNodes() { 
   return [
     " + nodeNames.ToString().Substring(2) + @"
     ];
   }");
    }
  }
}
