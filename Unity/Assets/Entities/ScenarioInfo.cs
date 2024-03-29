﻿using Assets.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System;

namespace Entity
{
    [Serializable]
    public class ScenarioInfo : ICloneable
    /// <summary>Create ScenarioInfo Object. Contains all GUI-Userinputs</summary>
    {

        /// <summary>
        /// Creates a new instance of ScenarioInfo class with default values.
        /// </summary>
        public ScenarioInfo()
        {
            Path = null;
            Pedestrians = new ObservableCollection<Adversary>();
            MapURL = null;
            WorldOptions = new WorldOptions();
            EgoVehicle = null;
            Vehicles = new ObservableCollection<Adversary>();
        }

        /// <summary>
        /// Converts the Model Representation, where Pedestrians are also Vehicles to the Scenario Representation, 
        /// where Pedestrians are represented as their own Objects.
        /// Creates a Deepcopy. 
        /// </summary>
        /// <returns>ScenarioInfo Object ready for XML-Export</returns>
        public object Clone()
        {
            string CopyPath = String.IsNullOrEmpty(this.Path)? String.Empty : string.Copy(this.Path); //Value
            string CopyMapURL = String.IsNullOrEmpty(this.MapURL) ? String.Empty : string.Copy(this.MapURL); //Value

            WorldOptions CopyWorldOptions = new();
            if (this.WorldOptions != null)
                CopyWorldOptions = (WorldOptions)this.WorldOptions.Clone();       

            Ego CopyEgoVehicle = new(); 
            if (this.EgoVehicle != null)
                CopyEgoVehicle = this.EgoVehicle;

            ObservableCollection<Adversary> exPedestrians = new();
            ObservableCollection<Adversary> exVehicles = new();
            
            foreach (Adversary v in this.Vehicles)
            {
                if (v.Category == AdversaryCategory.Pedestrian)
                {
                    //Didn't implement ICloneable interface, since Path can be reference to the Model Object. 
                    Adversary CopyPedestrian = (Adversary)v.Clone();
                    exPedestrians.Add(CopyPedestrian);
                }
                else
                {
                    Adversary cloneVehicle = (Adversary)v.Clone();
                    exVehicles.Add(cloneVehicle); //Value
                }
            }


            var info = new ScenarioInfo
            {
                Path = CopyPath,
                Pedestrians = exPedestrians,
                MapURL = CopyMapURL,
                WorldOptions = CopyWorldOptions,
                EgoVehicle = CopyEgoVehicle,
                Vehicles = exVehicles,
            };
            info.onEgoChanged = this.onEgoChanged;
            return info;
        }

        /// </summary>
        /// <param name="ego">The Ego object to set.</param>
        /// <returns>void</returns>
        public void setEgo(Ego ego)
        {
            this.EgoVehicle = ego;
            onEgoChanged();
        }
        
        public string Path { get; set; }
        public ObservableCollection<Adversary> Pedestrians { get; set; }
        public string MapURL { get; set; }
        public WorldOptions WorldOptions { get; set; } // MapRepository.cs has possible Maps. 
        public Ego EgoVehicle { get; private set; }
        public ObservableCollection<Adversary> Vehicles { get; set; }

        [NonSerialized]
        public System.Action onEgoChanged;

        public List<BaseEntity> allEntities
        {
            get
            {
                List<BaseEntity> allEntities = new List<BaseEntity>();
                if (EgoVehicle is not null)
                {
                    allEntities.Add(EgoVehicle);
                }
                allEntities.AddRange(Vehicles);
                allEntities.AddRange(Pedestrians);
                return allEntities;
            }
        }

    }
}