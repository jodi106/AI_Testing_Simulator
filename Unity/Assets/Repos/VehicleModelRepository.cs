using Assets.Enums;
using Entity;
using System.Collections.Generic;

namespace Assets.Repos
{
    public class VehicleModelRepository
    {
        private readonly static Dictionary<AdversaryCategory, List<EntityModel>> _EntityModels = new Dictionary<AdversaryCategory, List<EntityModel>>();
        static VehicleModelRepository()
        {
            _EntityModels.Add(AdversaryCategory.Car, new List<EntityModel>
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

            _EntityModels.Add(AdversaryCategory.Bike, new List<EntityModel>
            {
                new EntityModel("Leisure Bike", "vehicle.gazelle.omafiets"),
                new EntityModel("Road Bike", "vehicle.diamondback.century"),
                new EntityModel("Cross Bike", "vehicle.bh.crossbike"),
            });

            _EntityModels.Add(AdversaryCategory.Motorcycle, new List<EntityModel>
            {
                new EntityModel("Kawasaki Ninja", "vehicle.kawasaki.ninja"),
                new EntityModel("Harley", "vehicle.harley-davidson.low_rider"),
                new EntityModel("Yamaha", "vehicle.yamaha.yzf"),
                new EntityModel("Vespa", "vehicle.vespa.zx125"),
            });

            _EntityModels.Add(AdversaryCategory.Pedestrian, new List<EntityModel>
            {
                new EntityModel("Female", "walker.pedestrian.0001"),
                new EntityModel("Male", "walker.pedestrian.0002"),
                new EntityModel("Girl", "walker.pedestrian.0010"),
                new EntityModel("Boy", "walker.pedestrian.0014"),
            });

        }


        public static List<EntityModel> GetModelsBasedOnCategory(AdversaryCategory category)
        {
            return _EntityModels[category];
        }

        public static EntityModel getDefaultModel(AdversaryCategory cat)
        {
            switch(cat)
            {
                case AdversaryCategory.Car:
                    return getDefaultCarModel();
                case AdversaryCategory.Bike:
                    return getDefaultBikeModel();
                case AdversaryCategory.Motorcycle:
                    return getDefaultMotorcycleModel();
                case AdversaryCategory.Pedestrian:
                    return _EntityModels[cat][0];
            }
            return null;
        }

        public static EntityModel getDefaultCarModel()
        {
            return _EntityModels[AdversaryCategory.Car][1]; // Audi TT
        }

        public static EntityModel getDefaultBikeModel()
        {
            return _EntityModels[AdversaryCategory.Bike][0];
        }

        public static EntityModel getDefaultMotorcycleModel()
        {
            return _EntityModels[AdversaryCategory.Motorcycle][0];
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
