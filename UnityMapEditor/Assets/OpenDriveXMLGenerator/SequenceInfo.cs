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
        public bool IsJunction
        {
            get { return this.IsJunction; }
            set { this.IsJunction = value; }
        }

        public int Id
        {
            get { return this.Id; }
            set { this.Id = value; }
        }

        public List<int> leftLaneIds
        {
            get { return this.leftLaneIds; }
            set { this.leftLaneIds = value; }
        }

        public List<int> rightLaneIds
        {
            get { return this.rightLaneIds; }
            set { this.rightLaneIds = value; }
        }


        public SequenceInfo()
        {

        }

        public SequenceInfo(bool IsJunction, int Id, List<int> leftLaneIds, List<int> rightLaneIds)
        {
            this.IsJunction = IsJunction;
            this.Id = Id;
            this.leftLaneIds = leftLaneIds;
            this.rightLaneIds = rightLaneIds;

        }
    }
}

