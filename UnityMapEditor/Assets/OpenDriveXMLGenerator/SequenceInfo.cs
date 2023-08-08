using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenDriveXMLGenerator
{
    /// <summary>
    /// Each piece of road has two objects of the type SequenceInfo. One which contains the predecessors of the road piece for all lanes
    /// and one which contains the successors of the road piece of all lanes. The object SequenceInfo contains all needed information of the 
    /// predecessor/successor, which is needed to connect the road piece to them.
    /// </summary>
    public class SequenceInfo
    {
        /// <summary>
        /// The attribute isJunction identifies whether the predecessor/successor is part of a junction.
        /// </summary>
        /// <returns>The setter gives back a boolean value, whether the predecessor/successor road is part of a junction (true) or not (false).</returns>
        public bool IsJunction { get; set; }

        /// <summary>
        /// The list Ids contains all the ids of the predecessors/successors of the road piece.
        /// </summary>
        /// <returns>The setter gives back the list of ids of the predecessor/successor.</returns>
        public List<int> Ids { get; set; }

        /// <summary>
        /// The list LeftLaneIds contains all the lane ids of predecessor/successor lanes for the left lane of the road piece.
        /// </summary>
        /// <returns>The setter gives back the list of the lane ids of the predecessor/successor.</returns>
        public List<int> LeftLaneIds { get; set; }

        /// <summary>
        /// The list RightLaneIds contains all the lane ids of predecessor/successor lanes for the right lane of the road piece.
        /// </summary>
        /// <returns>The setter gives back the list of the lane ids of the predecessor/successor.</returns>
        public List<int> RightLaneIds { get; set; }

        /// <summary>
        /// The default constructor of the object SequenceInfo.
        /// </summary>
        public SequenceInfo()
        {

        }

        /// <summary>
        /// The default constructor of the object SequenceInfo.
        /// </summary>
        /// <param name="isJunction">Information whether the predecessor/successor is part of a junction.</param>
        /// <param name="ids">Ids of the predecessors/successors of the raod piece.</param>
        /// <param name="leftLaneIds">Lane ids of the predecessors/successors for the left lane of the road piece.</param>
        /// <param name="rightLaneIds">Lane ids of the predecessors/successors for the right lane of the road piece.</param>
        public SequenceInfo(bool isJunction, List<int> ids, List<int> leftLaneIds, List<int> rightLaneIds)
        {
            IsJunction = isJunction;
            Ids = ids;
            this.LeftLaneIds = leftLaneIds;
            this.RightLaneIds = rightLaneIds;
        }
    }
}

