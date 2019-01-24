using FireFive.PipelineVisualiser.PipelineGraph;

namespace FireFive.PipelineVisualiser.GraphSource
{
  /*
   * AbstractGraphSource class
   * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Represents a source of PipelineGraph.Graph information
   */
  public abstract class AbstractGraphSource
  {
    // return a graph
    public abstract Graph GetGraph();

  }
}