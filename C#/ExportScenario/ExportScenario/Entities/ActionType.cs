using System;
using System.Collections.Generic;
using System.Text;

namespace ExportScenario.Entities
{
    public class ActionType
    {

        public ActionType(string name)
        {
            Name = name;
        }
        public ActionType(string name, double speed, double absoluteTargetSpeedValue, string speedActionDynamics = "step", double speedActionDynamicsValue = 0, string dynamicsDimension = "time")
        /// for SpeedAction
        {
            Name = name;
            Speed = speed;
            AbsoluteTargetSpeedValue = absoluteTargetSpeedValue;
            SpeedActionDynamics = speedActionDynamics;
            SpeedActionDynamicsValue = speedActionDynamicsValue;
            DynamicDimensions = dynamicsDimension;
        }

        public ActionType(string name, List<Coord3D> positions)
        /// for AssignRouteAction (List lentgh > 1) or AcquirePositionAction (list length == 1)
        {
            Name = name;
            Positions = positions;
        }

        public ActionType(string name, double laneChangeActionDynamicsValue, string entityRef, int relativeTargetLaneValue)
        /// for LaneChangeAction
        {
            Name = name;
            LaneChangeActionDynamicsValue = laneChangeActionDynamicsValue;
            EntityRef = entityRef; //specify the entity which is referenzed for relative lane change (can be same entity as executing entity)
            RelativeTargetLaneValue = relativeTargetLaneValue;
        }
        

        public string Name { get; set; }
        public double Speed { get; set; }
        public double AbsoluteTargetSpeedValue { get; set; }
        public string SpeedActionDynamics { get; set; }
        public double SpeedActionDynamicsValue { get; set; }
        public string DynamicDimensions { get; set; }
        public List<Coord3D> Positions { get; set; }
        public double LaneChangeActionDynamicsValue { get; set; }
        public string EntityRef { get; set; }
        public int RelativeTargetLaneValue { get; set; }

    }
}

