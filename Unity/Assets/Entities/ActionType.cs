using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class ActionType
    /// <summary>Defines a user specified action for a Waypoint object</summary>
    {
        private static int autoIncrementId = 0;
        public ActionType(string name)
        {
            ID = autoIncrementId++;
            Name = name;
        }
        public ActionType(string name, double absoluteTargetSpeedValue, string speedActionDynamics = "step", double speedActionDynamicsValue = 0.0, string dynamicsDimension = "time")
        /// for SpeedAction
        {
            ID = autoIncrementId++;
            Name = name;
            AbsoluteTargetSpeedValue = absoluteTargetSpeedValue;
            SpeedActionDynamics = speedActionDynamics;
            SpeedActionDynamicsValue = speedActionDynamicsValue;
            DynamicDimensions = dynamicsDimension;
        }

        public ActionType(string name, List<Location> positions)
        /// for AssignRouteAction (List lentgh > 1) or AcquirePositionAction (list length == 1)
        {
            ID = autoIncrementId++;
            Name = name;
            Positions = positions;
        }

        public ActionType(string name, double laneChangeActionDynamicsValue, string entityRef, int relativeTargetLaneValue)
        /// for LaneChangeAction: laneChangeActionDynamicsValue must be bigger than 0, otherwise runtime error
        {
            ID = autoIncrementId++;
            Name = name;
            LaneChangeActionDynamicsValue = laneChangeActionDynamicsValue;
            EntityRef = entityRef; //specify the entity which is referenzed for relative lane change (can be same entity as executing entity)
            RelativeTargetLaneValue = relativeTargetLaneValue;
        }
        

        public int ID { get; private set; }
        public string Name { get; set; } // Todo rename?; has enum ActionTypeName; examples: SpeedAction, LaneChangeAction, AssignRouteAction
        public double AbsoluteTargetSpeedValue { get; set; } // double from 0 to infinitive; unit: meter per second; needed for SpeedAction
        public string SpeedActionDynamics { get; set; }
        public double SpeedActionDynamicsValue { get; set; }
        public string DynamicDimensions { get; set; }
        public List<Location> Positions { get; set; }
        public double LaneChangeActionDynamicsValue { get; set; }
        public string EntityRef { get; set; } // example: "adversary2" --> "adversary"+id
        public int RelativeTargetLaneValue { get; set; }

    }
}

