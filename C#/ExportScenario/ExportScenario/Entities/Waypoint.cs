using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ExportScenario.Entities
{
    public class Waypoint // Event in .xosc
    {
        public Waypoint(int id, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite")
        {
            Id = id;
            ActionTypeInfo = actionTypeInfo;
            Priority = priority;
            TriggerList = triggerList;
        }

        // In my option the involvedEntities are unecessary and never used. SO can we remove the Attribute "involvedEntities" in the class?
        // Bc: The waypoint(s) is added to a Path. This path is added to a vehicle. From the vehicle we can get the involvedEntity. Or am I missing sth?
        // In my option this Coord3D is unnecssary and never used. So we can remove the Attribute "position" in the class?
        public Waypoint(int id, Coord3D position, List<EntityModel> involvedEntities, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite")
        {
            Id = id;
            Position = position;
            InvolvedEntities = involvedEntities;
            ActionTypeInfo = actionTypeInfo; 
            Priority = priority;
            TriggerList = triggerList;
        }

        public int Id { get; set; }
        public Coord3D Position { get; set; }
        public List<EntityModel> InvolvedEntities { get; set; }
        public ActionType ActionTypeInfo { get; set; }
        public string Priority { get; set; }
        /* Priority types
        overwrite: All other Events in the scope are stopped and the Event starts.
        skip: The Event does not leave the standbyState until other Events have finished.
        parallel: The Event starts without taking into consideration already running Events.
         */
        public List<TriggerInfo> TriggerList { get; set; }
        // One Waypoint can have mutliple triggers for an event

    }
}
