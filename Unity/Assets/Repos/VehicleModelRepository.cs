using Assets.Enums;
using Models;
using System.Collections.Generic;

namespace Assets.Repos
{
    public class EntityModelRepository
    {
        private readonly Dictionary<VehicleCategory, List<Model>> _EntityModels;
        public EntityModelRepository()
        {
            _EntityModels = new Dictionary<VehicleCategory, List<Model>>();

            _EntityModels.Add(VehicleCategory.Car, new List<Model>
            {
                new Model(1,"Ambulance"),
                new Model(2,"AudiA2"),
                new Model(3,"AudiETron"),
                new Model(4,"AudiTT"),
                new Model(5,"BmwGrandTourer"),
                new Model(6,"BmwIsetta"),
                new Model(7,"CarlaCola"),
                new Model(8,"ChevroletImpala"),
                new Model(9,"CitroenC3"),
                new Model(9,"CitroenC3"),
                new Model(10,"Cybertruck"),
                new Model(11,"DodgeCharger2020"),
                new Model(12,"DodgeChargerPolice"),
                new Model(13,"FireTruck"),
                new Model(14,"Ford_Crown"),
                new Model(15,"JeepWranglerRubicon"),
                new Model(16,"LincolnMKZ2017"),
                new Model(17,"LincolnMKZ2020"),
                new Model(18,"Mercedes"),
                new Model(19,"Mini"),
                new Model(20,"Mini2021"),
                new Model(21,"Mustang"),
                new Model(22,"NissanMicra"),
                new Model(23,"NissanPatrol"),
                new Model(24,"NissanPatrol2021"),
                new Model(25,"SeatLeon"),
                new Model(26,"Sprinter"),
                new Model(27,"Tesla"),
                new Model(28,"ToyotaPrius"),
                new Model(29,"VolkswagenT2"),
                new Model(30,"Volkswagen_T2_2021"),
            });

            _EntityModels.Add(VehicleCategory.Bike, new List<Model>
            {
                new Model(31, "CrossBike"),
                new Model(32, "LeisureBike"),
                new Model(33, "RoadBike"),
            });

            _EntityModels.Add(VehicleCategory.Motorcycle, new List<Model>
            {
                new Model(34, "Harle,"),
                new Model(35, "KawasakiNinja"),
                new Model(36, "Yamaha"),
                new Model(37, "Vespa"),
            });

        }

        public List<Model> GetModelsBasedOnCategory(VehicleCategory category)
        {
            return _EntityModels[category];
        }


    }
}
