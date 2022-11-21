using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ExportScenario.Entities
{
    public class Waypoint // Event in .xosc
    /// <summary>Create Waypoint Object. Contains User defined Input for a specific Event on a Entity Path</summary>
    {
        public Waypoint(int id, Coord3D position, ActionType actionTypeInfo, List<TriggerInfo> triggerList, string priority = "overwrite")
        {
            Id = id;
            Position = position;
            ActionTypeInfo = actionTypeInfo;
            Priority = priority;
            TriggerList = triggerList;
        }

        public int Id { get; set; }
        public Coord3D Position { get; set; }
        public ActionType ActionTypeInfo { get; set; }
        public string Priority { get; set; }
        public List<TriggerInfo> TriggerList { get; set; }
        // One Waypoint can have mutliple triggers for an event

    }
}
