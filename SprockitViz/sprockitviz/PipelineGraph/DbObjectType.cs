namespace FireFive.PipelineVisualiser.PipelineGraph
{
  /*
   * DbObjectType enumeration
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Available types of database object.
   */
  public enum DbObjectType
  {
    StoredProcedure, SsisPackage,
    Table, View, ScalarFunction, TableValuedFunction,
    Unknown
  }
}