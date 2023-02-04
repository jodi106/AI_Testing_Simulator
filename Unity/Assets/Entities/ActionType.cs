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
        public ActionType(string name, double absoluteTargetSpeedValue, string speedActionDynamicsShape = "step", double speedActionDynamicsValue = 0.0, string dynamicsDimension = "time")
        /// for SpeedAction
        {
            ID = autoIncrementId++;
            Name = name;
            AbsoluteTargetSpeedValue = absoluteTargetSpeedValue;
            DynamicsShape = "linear";
            SpeedActionDynamicsValue = speedActionDynamicsValue;
            DynamicDimensions = dynamicsDimension;
        }

        public ActionType(string name, double speedActionDynamicsValue, string speedActionDynamicsShape = "step", string dynamicsDimension = "time")
        /// for BreakAction
        {
            ID = autoIncrementId++;
            Name = name;
            //SpeedActionDynamicsShape = speedActionDynamicsShape;
            SpeedActionDynamicsValue = speedActionDynamicsValue;
            DynamicDimensions = dynamicsDimension;
        }
        public ActionType(string name, string entityRef, int relativeTargetLaneValue, string dynamicsShape = "linear", double laneChangeActionDynamicsValue = 13, string dynamicsDimension = "distance")
        /// for LaneChangeAction: laneChangeActionDynamicsValue must be bigger than 0, otherwise runtime error
        {
            ID = autoIncrementId++;
            Name = name;
            LaneChangeActionDynamicsValue = laneChangeActionDynamicsValue;
            EntityRef = entityRef; //specify the entity which is referenzed for relative lane change (can be same entity as executing entity)
            RelativeTargetLaneValue = relativeTargetLaneValue;
            DynamicsShape = dynamicsShape;
            DynamicDimensions = dynamicsDimension;
        }

        public ActionType(string name, List<Location> positions)
        /// for AssignRouteAction (List lentgh > 1) or AcquirePositionAction (list length == 1)
        {
            ID = autoIncrementId++;
            Name = name;
            Positions = positions;
            CalculateLocationsCarla();
        }

        public int ID { get; private set; }
        public string Name { get; set; } // Todo rename?; has enum ActionTypeName; examples: SpeedAction, LaneChangeAction, AssignRouteAction
        public double AbsoluteTargetSpeedValue { get; set; } // double from 0 to infinitive(but ~300kmh should be max value); unit: meter per second; needed for SpeedAction
        public string DynamicsShape { get; set; } // has enum; good values: linear, step; only in advanced settings
        public double SpeedActionDynamicsValue { get; set; } // double: 0 to infinitive, good value: 0
        public double LaneChangeActionDynamicsValue { get; set; } // double: ~25 to infinitive, needs to be bigger than 0
        public string DynamicDimensions { get; set; } // has enum: distance, time, rate; only in advanced settings
        public List<Location> Positions { get; set; }
        public List<Location> PositionsCarla { get; set; }
        public string EntityRef { get; set; } // example: "adversary2" --> "adversary"+id
        public int RelativeTargetLaneValue { get; set; } // TODO: -1 or 1

        
        public void CalculateLocationsCarla()
        {
            PositionsCarla = new List<Location>();
            foreach(Location pos in Positions)
            {
                (float xCarla, float yCarla) = SnapController.UnityToCarla(pos.X, pos.Y);
                float rotCarla = SnapController.UnityRotToRadians(pos.Rot);
                rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
                PositionsCarla.Add(new Location(xCarla, yCarla, 0.3f, rotCarla));
            }
            
        }
    }
}

