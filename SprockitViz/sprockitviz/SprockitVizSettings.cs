using FireFive.PipelineVisualiser.GraphSource;
using FireFive.PipelineVisualiser.GraphSource.Sprockit;
using FireFive.PipelineVisualiser.Visualiser;
using FireFive.PipelineVisualiser.Visualiser.Graphviz;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace FireFive.PipelineVisualiser.SprockitViz
{
  /*
   * SprockitVizSettings class
   * Copyright (C) 2018 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/sprockitviz
   *
   * Settings for the sprockitviz application. Extends ConfigurationSection to
   * customise App.config; implements ISprockitSettings, IGraphvizSettings to
   * pass settings to SprockitGraphSource, GraphvizVisualiser.
   */
  public class SprockitVizSettings : ConfigurationSection, ISprockitSettings, IGraphvizSettings
  {
    private static SprockitVizSettings settings =
      (SprockitVizSettings)ConfigurationManager.GetSection("FireFive.PipelineVisualiser.SprockitViz");

    public static SprockitVizSettings Settings
    {
      get
      {
        return settings;
      }
    }

    // SprockitVizSettings represents *all* configured instances; an instance is selected
    // by specifying its alias as a command line argument. This field references the 
    // configured instance corresponding to the specified argument.
    private SprockitInstance context;

    // set the context from the specified command line argument
    internal void SetContext(string instanceAlias)
    {
      context = null;
      foreach (SprockitInstance i in SprockitInstances)
        if (i.InstanceAlias == instanceAlias)
          context = i;
      if (context == null)
        throw new VisualiserConfigurationException("Instance alias '" + instanceAlias + "' not defined.");
    }

    // get the graph source corresponding to the specified graphType
    public AbstractGraphSource GraphSource
    {
      get
      {
        switch (context.GraphType)
        {
          case "compact":
            return new CCSprockitGraph(this);
          default:
            throw new VisualiserConfigurationException("Graph type '" + context.GraphType + "' not recognised");
        }
      }
    }

    // get the graph visualiser corresponding to the specified displayMode
    public AbstractVisualiser Visualiser
    {
      get
      {
        switch (context.DisplayMode)
        {
          case "basic":
            return new BasicVisualiser(this);
          default:
            throw new VisualiserConfigurationException("Output mode '" + context.DisplayMode + "' not recognised");
        }
      }
    }

    public string OutputFolder
    {
      get
      {
        return context.OutputFolder;
      }
    }

    public Size MaxSize
    {
      get
      {
        return new Size()
        {
          Width = context.MaxWidth,
          Height = context.MaxHeight
        };
      }
    }

    public int SubgraphRadius
    {
      get
      {
        return context.SubgraphRadius;
      }
    }


    public Dictionary<string, string> DbColors
    {
      get
      {
        var colors = new Dictionary<string, string>();
        foreach (DbColor c in context.DbColors)
          colors.Add(c.DbName, c.Color);
        return colors;
      }
    }

    public string ConnectionString
    {
      get
      {
        return context.ConnectionString;
      }
    }

    [ConfigurationProperty("graphvizAppFolder", IsRequired = true)]
    public string GraphvizAppFolder
    {
      get
      {
        return this["graphvizAppFolder"].ToString();
      }
    }

    [ConfigurationProperty("verbose", DefaultValue = false, IsRequired = false)]
    public bool Verbose
    {
      get
      {
        return bool.Parse(this["verbose"].ToString());
      }
    }

    [ConfigurationProperty("deleteWorkingFiles", DefaultValue = true, IsRequired = false)]
    public bool DeleteWorkingFiles
    {
      get
      {
        return (bool)this["deleteWorkingFiles"];
      }
    }

    [ConfigurationProperty("SprockitInstances", IsRequired = true)]
    private SprockitInstanceCollection SprockitInstances
    {
      get
      {
        return (SprockitInstanceCollection)base["SprockitInstances"];
      }
    }

    [ConfigurationCollection(typeof(SprockitInstance), AddItemName = "Instance"
    , CollectionType = ConfigurationElementCollectionType.BasicMap)]
    private class SprockitInstanceCollection : ConfigurationElementCollection
    {
      protected override ConfigurationElement CreateNewElement()
      {
        return new SprockitInstance();
      }

      protected override Object GetElementKey(ConfigurationElement element)
      {
        return ((SprockitInstance)element).InstanceAlias;
      }

    }
    private class SprockitInstance : ConfigurationElement
    {
      [ConfigurationProperty("alias", IsRequired = true, IsKey = true)]
      public string InstanceAlias
      {
        get
        {
          return (string)this["alias"];
        }
      }

      [ConfigurationProperty("connectionString", IsRequired = true)]
      public string ConnectionString
      {
        get
        {
          return (string)this["connectionString"];
        }
      }

      [ConfigurationProperty("outputFolder", IsRequired = true)]
      public string OutputFolder
      {
        get
        {
          return (string)this["outputFolder"];
        }
      }

      [ConfigurationProperty("displayMode", DefaultValue = "basic", IsRequired = false)]
      public string DisplayMode
      {
        get
        {
          return (string)this["displayMode"];
        }
      }

      [ConfigurationProperty("graphType", DefaultValue = "compact", IsRequired = false)]
      public string GraphType
      {
        get
        {
          return (string)this["graphType"];
        }
      }


      [ConfigurationProperty("maxWidth", IsRequired = true)]
      public int MaxWidth
      {
        get
        {
          return int.Parse(this["maxWidth"].ToString());
        }
      }

      [ConfigurationProperty("maxHeight", IsRequired = true)]
      public int MaxHeight
      {
        get
        {
          return int.Parse(this["maxHeight"].ToString());
        }
      }

      [ConfigurationProperty("subgraphRadius", DefaultValue = 0, IsRequired = false)]
      public int SubgraphRadius
      {
        get
        {
          return int.Parse(this["subgraphRadius"].ToString());
        }
      }

      [ConfigurationProperty("dbColors", IsRequired = false)]
      public DbColorCollection DbColors
      {
        get
        {
          return (DbColorCollection)base["dbColors"];
        }
      }
    }

    [ConfigurationCollection(typeof(DbColor), AddItemName = "dbColor", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    private class DbColorCollection : ConfigurationElementCollection
    {
      protected override ConfigurationElement CreateNewElement()
      {
        return new DbColor();
      }

      protected override Object GetElementKey(ConfigurationElement element)
      {
        return ((DbColor)element).DbName;
      }
    }

    private class DbColor : ConfigurationElement
    {

      [ConfigurationProperty("dbName", IsRequired = true, IsKey = true)]
      public string DbName
      {
        get
        {
          return (string)this["dbName"];
        }
      }

      [ConfigurationProperty("color", IsRequired = true)]
      public string Color
      {
        get
        {
          return (string)this["color"];
        }
      }
    }
  }
}