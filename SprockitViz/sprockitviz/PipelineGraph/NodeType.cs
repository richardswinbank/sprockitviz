namespace FireFive.PipelineVisualiser.PipelineGraph
{
  /*
   * NodeType enumeration
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Available types of Node.
   */
  public enum NodeType
  {
    StoredProcedure, SsisPackage,
    Table, View, ScalarFunction, TableValuedFunction,
    Unknown
  }
}