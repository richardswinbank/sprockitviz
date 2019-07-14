using System;
using System.Collections.Generic;
using System.Configuration;

namespace FireFive.PipelineVisualiser.Visualiser.Graphviz
{
   /*
    * IGraphvizSettings interface
    * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
    * http://richardswinbank.net/
    *
    * Settings for GraphvizVisualiser.
    */
   public interface IGraphvizSettings
   {
      // delete Graphviz input files when finished?
      bool DeleteWorkingFiles { get; }

      // location of Graphviz binaries
      string GraphvizAppFolder { get; }

      // folder into which to write output files
      string OutputFolder { get; }

      // expected available display size (in graph nodes)
      Size MaxSize { get; }

      // collection of text colours for specified database names
      Dictionary<string, string> DbColors { get; }

      // write extra stuff to the console during processing?
      bool Verbose { get; }

      // output file format
      GraphvizOutputFormat OutputFormat { get; }

      // optional name of CSS stylesheet for use with HTML output format
      string HtmlStyleSheet { get; }

      string JavaScriptFile { get; }

      string Version { get; }

      // wait no longer than this for Graphviz to execute
      int GraphvizTimeout { get; }
   }

   public enum GraphvizOutputFormat
   {
      Svg, Html, Jpeg, App
  }
}