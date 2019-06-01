using FireFive.PipelineVisualiser.GraphSource;
using FireFive.PipelineVisualiser.GraphSource.Sprockit;
using FireFive.PipelineVisualiser.Visualiser;
using FireFive.PipelineVisualiser.Visualiser.Graphviz;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

namespace FireFive.PipelineVisualiser.SprockitViz
{
   /*
    * SprockitVizSettings class
    * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
    * http://richardswinbank.net/sprockitviz
    *
    * Settings for the sprockitviz application. Extends ConfigurationSection to
    * customise App.config; implements ISprockitSettings, IGraphvizSettings to
    * pass settings to SprockitGraphSource, GraphvizVisualiser.
    */
   public class SprockitVizSettings : ConfigurationSection, ISprockitSettings, IGraphvizSettings
   {
      public string Version
      {
         get
         {
            return "sprockitviz 0.2";
         }
      }

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
            if (i.Alias == instanceAlias)
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

      // folder into which to write output files
      public string OutputFolder
      {
         get
         {
            return context.OutputFolder;
         }
      }

      // expected available display size (in graph nodes)
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

      // radius to use when calculating subgraphs
      public int SubgraphRadius
      {
         get
         {
            return context.SubgraphRadius;
         }
      }

      // collection of text colours for specified database names
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

      // connection string for Sprockit database
      public string ConnectionString
      {
         get
         {
            return context.ConnectionString;
         }
      }

      // location of Graphviz binaries
      [ConfigurationProperty("graphvizAppFolder", IsRequired = true)]
      public string GraphvizAppFolder
      {
         get
         {
            return this["graphvizAppFolder"].ToString();
         }
      }

      // file format for output
      [ConfigurationProperty("outputFormat", DefaultValue = GraphvizOutputFormat.Svg, IsRequired = false)]
      [TypeConverter(typeof(OutputFormatConverter))]
      public GraphvizOutputFormat OutputFormat
      {
         get
         {
            return (GraphvizOutputFormat)this["outputFormat"];
         }
      }

      private class OutputFormatConverter : ConfigurationConverterBase
      {
         public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo culture, object value)
         {
            switch (value.ToString().ToLower())
            {
               case "html":
                  return GraphvizOutputFormat.Html;
               case "jpeg":
               case "jpg":
                  return GraphvizOutputFormat.Jpeg;
               default:
                  return GraphvizOutputFormat.Svg;
            }
         }
      }

      [ConfigurationProperty("htmlStyleSheet", DefaultValue = "_sprockitviz.css", IsRequired = false)]
      public string HtmlStyleSheet
      {
         get
         {
            return (string)this["htmlStyleSheet"];
         }
      }

      public string JavaScriptFile
      {
         get
         {
            return "_sprockitviz.js";
         }
      }

      // write extra stuff to the console during processing?
      [ConfigurationProperty("verbose", DefaultValue = false, IsRequired = false)]
      public bool Verbose
      {
         get
         {
            return (bool)this["verbose"];
         }
      }

      // delete Graphviz input files when finished?
      [ConfigurationProperty("deleteWorkingFiles", DefaultValue = true, IsRequired = false)]
      public bool DeleteWorkingFiles
      {
         get
         {
            return (bool)this["deleteWorkingFiles"];
         }
      }

      // wait no longer than this for Graphviz to execute
      [ConfigurationProperty("graphvizTimeout", DefaultValue = 5, IsRequired = false)]
      public int GraphvizTimeout
      {
         get
         {
            return (int)this["graphvizTimeout"];
         }
      }

      // collection of configured Sprockit instances, each identified by a unique alias
      [ConfigurationProperty("SprockitInstances", IsRequired = true)]
      private SprockitInstanceCollection SprockitInstances
      {
         get
         {
            return (SprockitInstanceCollection)base["SprockitInstances"];
         }
      }

      // class to represent collection of configured Sprockit instances, each identified by a unique alias
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
            return ((SprockitInstance)element).Alias;
         }

      }

      // class to represent a configured Sprockit instance, identified by a unique alias
      private class SprockitInstance : ConfigurationElement
      {
         // instance's uniquely-identifying alias
         [ConfigurationProperty("alias", IsRequired = true, IsKey = true)]
         public string Alias
         {
            get
            {
               return (string)this["alias"];
            }
         }

         // connection string for instance database
         [ConfigurationProperty("connectionString", IsRequired = true)]
         public string ConnectionString
         {
            get
            {
               return (string)this["connectionString"];
            }
         }

         // folder into which to write output files
         [ConfigurationProperty("outputFolder", IsRequired = true)]
         public string OutputFolder
         {
            get
            {
               return (string)this["outputFolder"];
            }
         }

         // which AbstractVisualiser to use
         [ConfigurationProperty("displayMode", DefaultValue = "basic", IsRequired = false)]
         public string DisplayMode
         {
            get
            {
               return (string)this["displayMode"];
            }
         }

         // which SprockitGraphSource to use
         [ConfigurationProperty("graphType", DefaultValue = "compact", IsRequired = false)]
         public string GraphType
         {
            get
            {
               return (string)this["graphType"];
            }
         }

         // expected available width of display (in graph nodes)
         [ConfigurationProperty("maxWidth", IsRequired = true)]
         public int MaxWidth
         {
            get
            {
               return int.Parse(this["maxWidth"].ToString());
            }
         }

         // expected available height of display (in graph nodes)
         [ConfigurationProperty("maxHeight", IsRequired = true)]
         public int MaxHeight
         {
            get
            {
               return int.Parse(this["maxHeight"].ToString());
            }
         }

         // radius to use when calculating subgraphs
         [ConfigurationProperty("subgraphRadius", DefaultValue = 0, IsRequired = false)]
         public int SubgraphRadius
         {
            get
            {
               return int.Parse(this["subgraphRadius"].ToString());
            }
         }

         // collection of text colours for specified database names
         [ConfigurationProperty("dbColors", IsRequired = false)]
         public DbColorCollection DbColors
         {
            get
            {
               return (DbColorCollection)base["dbColors"];
            }
         }
      }

      // class to represent collection of text colours for specified database names
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

      // class to represent text colours for specified database name
      private class DbColor : ConfigurationElement
      {
         // database name
         [ConfigurationProperty("dbName", IsRequired = true, IsKey = true)]
         public string DbName
         {
            get
            {
               return (string)this["dbName"];
            }
         }

         // text color
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