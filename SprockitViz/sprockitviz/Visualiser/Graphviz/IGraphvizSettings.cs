using System;
using System.Collections.Generic;
using System.Configuration;

namespace FireFive.PipelineVisualiser.Visualiser.Graphviz
{
  /*
   * IGraphvizSettings interface
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Settings for GraphvizVisualiser.
   */
  public interface IGraphvizSettings
  {
    bool DeleteWorkingFiles { get; }

    string GraphvizAppFolder { get; }

    string OutputFolder { get; }

    int MaxWidth { get; }

    int MaxHeight { get; }

    Dictionary<string, string> DbColors { get; }
    bool Verbose { get; }
  }
}