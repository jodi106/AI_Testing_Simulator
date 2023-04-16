using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * The name in the ActionType constructors should be like the name described in each constructor (e.g. "SpeedAction" or "StopAction")! 
 * This must be the case for Export functionality.
 */

namespace Entity
{
    [Serializable]
    public class ActionType : ICloneable
    /// <summary>Defines a user specified action for a Waypoint object</summary>
    {
        private static int autoIncrementId = 0;
        public ActionType(string name)
        {
            ID = autoIncrementId++;
            Name = name;
        }
        public ActionType(string name, double absoluteTargetSpeedValue, string speedActionDynamicsShape = "step", double speedActionDynamicsValue = 0.0, string dynamicsDimension = "time")
        /// for "SpeedAction"
        {
            ID = autoIncrementId++;
            Name = name;
            AbsoluteTargetSpeedValueKMH = absoluteTargetSpeedValue;
            DynamicsShape = "linear";
            SpeedActionDynamicsValue = speedActionDynamicsValue;
            DynamicDimensions = dynamicsDimension;
        }
        public ActionType(string name, double stopduration, double speedValue, string speedActionDynamicsShape = "step", double speedActionDynamicsValue = 0.0, string dynamicsDimension = "time")
        /// for "StopAction"
        {
            ID = autoIncrementId++;
            Name = name;
            StopDuration = stopduration;
            AbsoluteTargetSpeedValueKMH = speedValue;
            DynamicsShape = "linear";
            SpeedActionDynamicsValue = speedActionDynamicsValue;
            DynamicDimensions = dynamicsDimension;

        }

        public ActionType(string name, string entityRef, int relativeTargetLaneValue, string dynamicsShape = "linear", double laneChangeActionDynamicsValue = 13, string dynamicsDimension = "distance")
        /// for "LaneChangeAction": laneChangeActionDynamicsValue must be bigger than 0, otherwise runtime error
        {
            ID = autoIncrementId++;
            Name = name;
            LaneChangeActionDynamicsValue = laneChangeActionDynamicsValue;
            EntityRef = entityRef; //specify the entity which is referenzed for relative lane change (can be same entity as executing entity)
            RelativeTargetLaneValue = relativeTargetLaneValue;
            DynamicsShape = dynamicsShape;
            DynamicDimensions = dynamicsDimension;
        }


        /// <summary>
        /// Creates a new instance of the <see cref="ActionType"/> class.
        /// </summary>
        public ActionType(string name, List<Location> positions)
        /// for "AssignRouteAction" (List length > 2) or AcquirePositionAction (list length == 2)
        {
            ID = autoIncrementId++;
            Name = name;
            Positions = positions;
            CalculateLocationsCarla();
        }


        /// <summary>
        /// Creates a new empty instance of the <see cref="ActionType"/> class.
        /// </summary>
        public ActionType()
        {

        }

        public int ID { get; private set; }
        public string Name { get; set; } // TODO has enum ActionTypeName; examples: SpeedAction, LaneChangeAction, AssignRouteAction
        public double AbsoluteTargetSpeedValueKMH { get; set; } // double from 0 to infinitive(but ~300kmh should be max value); unit: meter per second; needed for SpeedAction
        public string DynamicsShape { get; set; } // has enum; good values: linear, step; only in advanced settings
        public double SpeedActionDynamicsValue { get; set; } // double: 0 to infinitive, good value: 0
        public double LaneChangeActionDynamicsValue { get; set; } // double: ~25 to infinitive, needs to be bigger than 0
        public string DynamicDimensions { get; set; } // has enum: distance, time, rate; only in advanced settings
        public List<Location> Positions { get; set; }
        public List<Location> PositionsCarla { get; set; }
        public string EntityRef { get; set; } // example: "adversary2" --> "adversary"+id
        public int RelativeTargetLaneValue { get; set; } // -1 or 1
        public double StopDuration { get; set; }


        /// <summary>
        /// Calculates the corresponding Carla locations for each position in the Positions list, using the SnapController class.
        /// </summary>
        /// <remarks>
        /// The method converts each Unity position and rotation into its corresponding Carla position and rotation, and adds it to the PositionsCarla list.
        /// </remarks>
        public void CalculateLocationsCarla()
        {
            PositionsCarla = new List<Location>();
            foreach (Location pos in Positions)
            {
                (float xCarla, float yCarla) = SnapController.UnityToCarla(pos.X, pos.Y);
                float rotCarla = SnapController.UnityRotToRadians(pos.Rot);
                rotCarla = (float)Math.Round(rotCarla * 100f) / 100f; // otherwise we'll have a number like this 3.339028E-05
                PositionsCarla.Add(new Location(xCarla, yCarla, 0.3f, rotCarla));
            }
        }


        /// <summary>
        /// Creates a deep copy of the current ActionType object.
        /// </summary>
        /// <returns>A new ActionType object with the same properties as the current object.</returns>
        public object Clone()
        {      
            ActionType cloneActionType = new ActionType();

            cloneActionType.Name = String.IsNullOrEmpty(this.Name) ? String.Empty : string.Copy(this.Name);
            cloneActionType.ID = this.ID;
            cloneActionType.AbsoluteTargetSpeedValueKMH = this.AbsoluteTargetSpeedValueKMH;
            cloneActionType.DynamicsShape = this.DynamicsShape;
            cloneActionType.SpeedActionDynamicsValue = this.SpeedActionDynamicsValue;
            cloneActionType.LaneChangeActionDynamicsValue = this.LaneChangeActionDynamicsValue;
            cloneActionType.DynamicDimensions = this.DynamicDimensions;

            //Cloning the Location, so we don't just get references to the Location of this Object.
            cloneActionType.Positions = new();
            if (this.Positions != null)
                cloneActionType.Positions = this.Positions.Select(x => (Location)x.Clone()).ToList();

            cloneActionType.PositionsCarla = new();
            if (this.PositionsCarla != null)
                cloneActionType.PositionsCarla = this.PositionsCarla.Select(x => (Location)x.Clone()).ToList();

            cloneActionType.EntityRef = String.IsNullOrEmpty(this.EntityRef) ? String.Empty : string.Copy(this.EntityRef); //Value
            cloneActionType.RelativeTargetLaneValue = this.RelativeTargetLaneValue;
            cloneActionType.StopDuration = this.StopDuration;

            return cloneActionType;
        }
    }
}

