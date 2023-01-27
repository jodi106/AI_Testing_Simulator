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
                new EntityModel(1,"Ambulance", "vehicle.ford.ambulance"),
                new EntityModel(4,"AudiTT","vehicle.audi.tt"),
                new EntityModel(5,"BmwGrandTourer", "vehicle.bmw.grandtourer"),
                new EntityModel(7,"CarlaCola","vehicle.carlamotors.carlacola"),
                new EntityModel(8,"ChevroletImpala", "vehicle.chevrolet.impala"),
                new EntityModel(9,"CitroenC3", "vehicle.citroen.c3"),
                new EntityModel(10,"Cybertruck", "vehicle.tesla.cybertruck"),
                new EntityModel(12,"DodgeChargerPolice", "vehicle.dodge.charger_police_2020"),
                new EntityModel(13,"FireTruck","vehicle.carlamotors.firetruck"),
                new EntityModel(14,"Ford_Crown", "vehicle.ford.crown"),
                new EntityModel(15,"JeepWranglerRubicon", "vehicle.jeep.wrangler_rubicon"),
                new EntityModel(18,"Mercedes", "vehicle.mercedes.coupe"),
                new EntityModel(20,"Mini2021", "vehicle.mini.cooper_s_2021"),
                new EntityModel(21,"Mustang", "vehicle.ford.mustang"),
                new EntityModel(24,"NissanPatrol2021", "vehicle.nissan.patrol_2021"),
                new EntityModel(25,"SeatLeon", "vehicle.seat.leon"),
                new EntityModel(26,"Sprinter","vehicle.mercedes.sprinter"),
                new EntityModel(27,"Tesla", "vehicle.tesla.model3"),
                new EntityModel(30,"Volkswagen_T2_2021", "vehicle.volkswagen.t2_2021"),
            });

            _EntityModels.Add(VehicleCategory.Bike, new List<EntityModel>
            {
                new EntityModel(31, "CrossBike"),
                new EntityModel(32, "LeisureBike"),
                new EntityModel(33, "RoadBike"),
            });

            _EntityModels.Add(VehicleCategory.Motorcycle, new List<EntityModel>
            {
                new EntityModel(34, "Harle,"),
                new EntityModel(35, "KawasakiNinja"),
                new EntityModel(36, "Yamaha"),
                new EntityModel(37, "Vespa"),
            });

        }

        public static List<EntityModel> GetModelsBasedOnCategory(VehicleCategory category)
        {
            return _EntityModels[category];
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
