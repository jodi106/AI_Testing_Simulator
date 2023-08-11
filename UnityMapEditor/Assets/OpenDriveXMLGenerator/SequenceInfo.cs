using System.Collections.Generic;

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
        public bool IsJunction { get; set; }

        /**
         * List of all the ids of the predecessors/successors
         */
        public List<int> Ids { get; set; }

        /**
         * List of all the lane ids of the predecessors/successors lanes for the left lane
         */
        public List<int> LeftLaneIds { get; set; }

        /**
         * List of all the lane ids of the predecessors/successors lanes for the right lane
         */
        public List<int> RightLaneIds { get; set; }


        public SequenceInfo()
        {

        }

        public SequenceInfo(bool isJunction, List<int> ids, List<int> leftLaneIds, List<int> rightLaneIds)
        {
            IsJunction = isJunction;
            Ids = ids;
            this.LeftLaneIds = leftLaneIds;
            this.RightLaneIds = rightLaneIds;
        }
    }
}

