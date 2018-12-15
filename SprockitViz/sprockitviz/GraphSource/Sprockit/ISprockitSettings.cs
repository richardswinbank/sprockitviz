using System;
using System.Configuration;

namespace FireFive.PipelineVisualiser.GraphSource.Sprockit
{
  /*
   * ISprockitSettings interface
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/sprockitviz
   *
   * Settings for SprockitGraphSource.
   */
  public interface ISprockitSettings
  {
    string ConnectionString { get; }
  } 
}