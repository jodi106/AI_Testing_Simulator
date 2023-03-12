using Assets.Enums;
using Entity;
using System.Collections.Generic;

namespace Assets.Repos
{
    public class VehicleModelRepository
    {
        private readonly static Dictionary<VehicleCategory, List<EntityModel>> _EntityModels = new Dictionary<VehicleCategory, List<EntityModel>>();
        static VehicleModelRepository()
        {
            _EntityModels.Add(VehicleCategory.Car, new List<EntityModel>
            {
                new EntityModel("Ambulance", "vehicle.ford.ambulance"),
                new EntityModel("Audi TT","vehicle.audi.tt"),
                new EntityModel("Bmw Grand Tourer", "vehicle.bmw.grandtourer"),
                new EntityModel("CarlaCola","vehicle.carlamotors.carlacola"),
                new EntityModel("Chevrolet Impala", "vehicle.chevrolet.impala"),
                new EntityModel("Citroen C3", "vehicle.citroen.c3"),
                new EntityModel("Cybertruck", "vehicle.tesla.cybertruck"),
                new EntityModel("Dodge Charger Police", "vehicle.dodge.charger_police_2020"),
                new EntityModel("Fire Truck","vehicle.carlamotors.firetruck"),
                new EntityModel("Ford Crown", "vehicle.ford.crown"),
                new EntityModel("Jeep Wrangler Rubicon", "vehicle.jeep.wrangler_rubicon"),
                new EntityModel("Mercedes", "vehicle.mercedes.coupe"),
                new EntityModel("Mini2021", "vehicle.mini.cooper_s_2021"),
                new EntityModel("Mustang", "vehicle.ford.mustang"),
                new EntityModel("NissanPatrol2021", "vehicle.nissan.patrol_2021"),
                new EntityModel("SeatLeon", "vehicle.seat.leon"),
                new EntityModel("Sprinter","vehicle.mercedes.sprinter"),
                new EntityModel("Tesla", "vehicle.tesla.model3"),
                new EntityModel("Volkswagen T2", "vehicle.volkswagen.t2_2021"),
            });

            _EntityModels.Add(VehicleCategory.Bike, new List<EntityModel>
            {
                new EntityModel("Cross Bike"),
                new EntityModel("Leisure Bike"),
                new EntityModel("Road Bike"),
            });

            _EntityModels.Add(VehicleCategory.Motorcycle, new List<EntityModel>
            {
                new EntityModel("Harley"),
                new EntityModel("Kawasaki Ninja"),
                new EntityModel("Yamaha"),
                new EntityModel("Vespa"),
            });

            _EntityModels.Add(VehicleCategory.Pedestrian, new List<EntityModel>
            {
                new EntityModel("Female", "BP_Walker_Female1_v1"),
                new EntityModel("Male", "BP_Walker_Male1_v1"),
                new EntityModel("Girl", "BP_WalkerEuGirl02_v1"),
                new EntityModel("Boy", "BP_WalkerKid1_v1"),
            });

        }


        public static List<EntityModel> GetModelsBasedOnCategory(VehicleCategory category)
        {
            return _EntityModels[category];
        }

        public static EntityModel getDefaultModel(VehicleCategory cat)
        {
            switch(cat)
            {
                case VehicleCategory.Car:
                    return getDefaultCarModel();
                case VehicleCategory.Bike:
                    return getDefaultBikeModel();
                case VehicleCategory.Motorcycle:
                    return getDefaultMotorcycleModel();
                case VehicleCategory.Pedestrian:
                    return _EntityModels[cat][0];
            }
            return null;
        }

        public static EntityModel getDefaultCarModel()
        {
            return _EntityModels[VehicleCategory.Car][1];
        }

        public static EntityModel getDefaultBikeModel()
        {
            return _EntityModels[VehicleCategory.Bike][0];
        }

        public static EntityModel getDefaultMotorcycleModel()
        {
            return _EntityModels[VehicleCategory.Motorcycle][0];
        }

        public static EntityModel findModel(string description)
        {
            EntityModel model = null;
            foreach (var cat in _EntityModels)
            {
                model = cat.Value.Find((x) =>
                {
                    return x.DisplayName == description;
                });
                if(model is not null)
                {
                    return model;
                }
            }
            return model;
        }
    }
}
