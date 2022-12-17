﻿using Assets.Enums;

namespace Entity
{
    public class Vehicle : BaseEntity
    {
        private static int autoIncrementId = 0;
        public Vehicle() : base(++autoIncrementId) { }

        public Vehicle(Location spawnPoint, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId,spawnPoint, initialSpeed)
        {
            Category = category;
        }

        public Vehicle(Location spawnPoint, Path path, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId, spawnPoint, initialSpeed)
        {
            Path = path;
            Category = category;
        }

        public Vehicle(Location spawnPoint, EntityModel model, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId, spawnPoint, initialSpeed)
        {
            Model = model;
            Category = category;
        }


        public Vehicle(Location spawnPoint, EntityModel model, Path path, VehicleCategory category = VehicleCategory.Null, double initialSpeed = 0) : base(++autoIncrementId, spawnPoint, initialSpeed)
        {
            Model = model;
            Path = path;
            Category = category;

        }

        //TODO: maybe use templates instead
        public void setView(IVehicleView view)
        {
            base.setView(view);
            this.VehicleView = view;
        }
        public EntityModel Model { get; set; }
        public VehicleCategory Category { get; set; }
        public Path Path { get; set; }

        private IVehicleView VehicleView;
    }
}
