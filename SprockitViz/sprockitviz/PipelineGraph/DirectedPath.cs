using System.Collections.Generic;

namespace FireFive.PipelineVisualiser.PipelineGraph
{
   /*
   * DirectedPath class
   * Copyright (c) 2018-2019 Richard Swinbank (richard@richardswinbank.net) 
   * http://richardswinbank.net/
   *
   * Class representing an path as an ordered sequence of nodes.
   */

   class DirectedPath : List<Node>
   {
      // create a new path containing a single node
      public DirectedPath(Node n) : base()
      {
         Add(n);
      }

      // return the path's weight (the sum of its nodes' weight)
      public int Weight
      {
         get
         {
            int weight = 0;
            foreach (var n in this)
               weight += n.Weight;
            return weight;
         }
      }

      // add a new node at the *start* of this bpath
      internal DirectedPath Prefix(Node n)
      {
         Insert(0, n);
         return this;
      }
   }
}
