using Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Helpers
{
    public static class Mapper
    {
        public static ScenarioInfo MapScenarioInfo(ScenarioInfoModel scenarioInfoModel)
        {
            var scenarioInfo = new ScenarioInfo
            {
                Name = scenarioInfoModel.Name,
                Pedestrians = MapModelToEntity<Pedestrian, PedestrianModel>(scenarioInfoModel.Pedestrians.ToList()),
                MapURL = scenarioInfoModel.MapURL,
                WorldOptions = scenarioInfoModel.WorldOptions,
                EgoVehicle = MapModelToEntity<Ego, EgoModel>(scenarioInfoModel.EgoVehicle),
                Vehicles = MapModelToEntity<Vehicle, VehicleModel>(scenarioInfoModel.Vehicles.ToList())
            };

            return scenarioInfo;
        }

        private static E MapModelToEntity<E, M>(M model) where E : BaseEntity, new() where M : BaseModel
        {
            var entity = new E();

            var entityProperties = entity.GetType().GetProperties();

            var modelProperties = model.GetType().GetProperties();

            if (entityProperties.Count() != modelProperties.Count())
            {
                //replace with better exception
                throw new ArgumentOutOfRangeException();
            }

            for (var index = 0; index < entityProperties.Count(); index++)
            {
                entityProperties[index].SetValue(entity, modelProperties[index].GetValue(model), null);
            }

            return entity;

        }

        public static List<E> MapModelToEntity<E,M>(List<M> modelList) where E : BaseEntity, new() where M : BaseModel
        {
            var entities = new List<E>();

            foreach(var model in modelList)
            {
                try
                {
                    entities.Add(MapModelToEntity<E, M>(model));
                }
                catch (Exception)
                {
                    // add more error logging here
                    throw;
                }
            }

            return entities;
        }


        //public static Pedestrian mapModelToEntity(PedestrianModel pedestrianModel)
        //{
        //    return new Pedestrian(
        //        pedestrianModel.Id, 
        //        pedestrianModel.SpawnPoint, 
        //        pedestrianModel.Model, 
        //        pedestrianModel.Type, 
        //        pedestrianModel.Path
        //    );
        //}

    }
}
