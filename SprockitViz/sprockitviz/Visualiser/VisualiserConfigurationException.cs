using System;

namespace FireFive.PipelineVisualiser.Visualiser
{
  /*
   * VisualiserConfigurationException class
   * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Exception representing an error in a visualiser's configuration 
   */
  internal class VisualiserConfigurationException : Exception
  {
    public VisualiserConfigurationException(string message) : base(message) { }
  }
}