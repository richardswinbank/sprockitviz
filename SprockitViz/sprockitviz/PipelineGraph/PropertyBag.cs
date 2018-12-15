using System.Collections.Generic;

namespace FireFive.PipelineVisualiser.PipelineGraph
{
  /*
   * PropertyBag class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Generic container for a Dictionary collection.
   */
  public class PropertyBag<T>
  {
    private Dictionary<string, T> properties;

    public PropertyBag()
    {
      properties = new Dictionary<string, T>();
    }

    public void SetProperty(string propertyName, T propertyValue)
    {
      properties[propertyName] = propertyValue;
    }

    public bool HasProperty(string propertyName)
    {
      return properties.ContainsKey(propertyName);
    }

    public T GetProperty(string propertyName)
    {
      return properties[propertyName];
    }

  }
}