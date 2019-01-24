using FireFive.PipelineVisualiser.PipelineGraph;
using FireFive.PipelineVisualiser.Visualiser;
using System;
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
      //if (args.Length == 0)
      //  args = new string[] { "my_test_sprockit_instance" };  // just for testing in VS

      Console.WriteLine(@"
  ****************************************************************************
  *  Sprockit Pipeline Visualiser                                            *
  *  Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net)  *
  *  http://richardswinbank.net/sprockitviz                                  *
  ****************************************************************************
");
      Thread.Sleep(2000);
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
        if(settings != null && settings.Verbose)
          Console.WriteLine(e);  // dump whole stack trace
      }
    }

    // build visualisations for an ETL pipeline graph using config info from SprockitVizSettings
    private void Run(SprockitVizSettings settings)
    {
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

      // visualise the subgraph around each node in the graph
      int i = 0;
      foreach (Node n in graph.Nodes)
      {
        i++;
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
    }
  }
}
