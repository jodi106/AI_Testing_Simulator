using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ExportScenario.Entities
{
    public class Waypoint
    {
        public Waypoint(int id, Coord3D position, List<BaseEntity> involvedEntities, string actionType, string priority, TriggerInfo triggerInfo)
        {
            Id = id;
            Position = position;
            InvolvedEntities = involvedEntities;
            ActionType = actionType;
            Priority = priority;
            Trigger_Info = triggerInfo;
        }

        public int Id { get; set; }
        public Coord3D Position { get; set; }
        public List<BaseEntity> InvolvedEntities { get; set; }
        public string ActionType { get; set; }
        public string Priority { get; set; }
        /* Priority types
        overwrite: All other Events in the scope are stopped and the Event starts.
        skip: The Event does not leave the standbyState until other Events have finished.
        parallel: The Event starts without taking into consideration already running Events.
         */
        public TriggerInfo Trigger_Info { get; set; }
        // consider creating a list of TriggerInfo objects to allow multiple triggers for one ActionType
        // TriggerInfo requires new class containing info about trigger attirbutes of an ActionType
    }
}
