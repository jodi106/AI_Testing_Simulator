using Assets.Enums;
using Entity;
using System.Collections.Generic;

namespace Assets.Repos
{
    public class VehicleModelRepository
    {
        private readonly Dictionary<VehicleCategory, List<EntityModel>> _EntityModels;
        public VehicleModelRepository()
        {
            _EntityModels = new Dictionary<VehicleCategory, List<EntityModel>>();

            _EntityModels.Add(VehicleCategory.Car, new List<EntityModel>
            {
                new EntityModel(1,"Ambulance"),
                new EntityModel(2,"AudiA2"),
                new EntityModel(3,"AudiETron"),
                new EntityModel(4,"AudiTT"),
                new EntityModel(5,"BmwGrandTourer"),
                new EntityModel(6,"BmwIsetta"),
                new EntityModel(7,"CarlaCola"),
                new EntityModel(8,"ChevroletImpala"),
                new EntityModel(9,"CitroenC3"),
                new EntityModel(9,"CitroenC3"),
                new EntityModel(10,"Cybertruck"),
                new EntityModel(11,"DodgeCharger2020"),
                new EntityModel(12,"DodgeChargerPolice"),
                new EntityModel(13,"FireTruck"),
                new EntityModel(14,"Ford_Crown"),
                new EntityModel(15,"JeepWranglerRubicon"),
                new EntityModel(16,"LincolnMKZ2017"),
                new EntityModel(17,"LincolnMKZ2020"),
                new EntityModel(18,"Mercedes"),
                new EntityModel(19,"Mini"),
                new EntityModel(20,"Mini2021"),
                new EntityModel(21,"Mustang"),
                new EntityModel(22,"NissanMicra"),
                new EntityModel(23,"NissanPatrol"),
                new EntityModel(24,"NissanPatrol2021"),
                new EntityModel(25,"SeatLeon"),
                new EntityModel(26,"Sprinter"),
                new EntityModel(27,"Tesla"),
                new EntityModel(28,"ToyotaPrius"),
                new EntityModel(29,"VolkswagenT2"),
                new EntityModel(30,"Volkswagen_T2_2021"),
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

        public List<EntityModel> GetModelsBasedOnCategory(VehicleCategory category)
        {
            return _EntityModels[category];
        }


    }
}
