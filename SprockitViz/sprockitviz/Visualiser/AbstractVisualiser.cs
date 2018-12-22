using FireFive.PipelineVisualiser.PipelineGraph;

namespace FireFive.PipelineVisualiser.Visualiser
{
  /*
   * AbstractVisualiser class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Represents a visualiser for PipelineGraph.Graph information
   */
  public abstract class AbstractVisualiser
  {
    // visualise a graph
    public abstract void Visualise(Graph g);

  }
}