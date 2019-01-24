using System;

namespace FireFive.PipelineVisualiser.Visualiser
{
  /*
   * VisualiserRenderingException class
   * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Exception representing a visualiser execution error not related to configuration problems
   */
  internal class VisualiserRenderingException : Exception
  {
    public VisualiserRenderingException(string message) : base(message) { }
  }
}