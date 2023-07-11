using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenDriveXMLGenerator
{
    /**
 * Each piece of road has two objects of the type SequenceInfo. One which contains the predecessors of the road piece for all lanes
 * and one which contains the successors of the road piece of all lanes.
*/
    public class SequenceInfo
    {
        /**
         * describes what type of predecessor/successor is
         * true: junction, false: road
         */
        public bool IsJunction
        {
            get { return this.IsJunction; }
            set { this.IsJunction = value; }
        }

        /**
         * List of all the ids of the predecessors/successors
         */
        public List<int> Ids
        {
            get { return this.Ids; }
            set { this.Ids = value; }
        }

        /**
         * List of all the lane ids of the predecessors/successors lanes for the left lane
         */
        public List<int> leftLaneIds
        {
            get { return this.leftLaneIds; }
            set { this.leftLaneIds = value; }
        }

        /**
         * List of all the lane ids of the predecessors/successors lanes for the right lane
         */
        public List<int> rightLaneIds
        {
            get { return this.rightLaneIds; }
            set { this.rightLaneIds = value; }
        }


        public SequenceInfo()
        {

        }

        public SequenceInfo(bool IsJunction, List<int> Ids, List<int> leftLaneIds, List<int> rightLaneIds)
        {
            this.IsJunction = IsJunction;
            this.Ids = Ids;
            this.leftLaneIds = leftLaneIds;
            this.rightLaneIds = rightLaneIds;

        }
    }
}

