using System;
using System.Collections.Generic;

namespace Entity
{
    [Serializable]

    /// <summary>
    /// Represents a road on a road network in CARLA
    /// </summary>
    public class Road
    {
        /// <summary>
        /// Initializes a new instance of the Road class.
        /// param name="id">The ID of the road.</param>
        /// param name="lanes">A list of lanes that belong to the road.</param>
        /// </summary>
        public Road(int id, SortedList<int, Lane> lanes)
        {
            Id = id;
            Lanes = lanes;
        }

        public int Id { get; set; }


        public SortedList<int, Lane> Lanes { get; set; }
    }
}

