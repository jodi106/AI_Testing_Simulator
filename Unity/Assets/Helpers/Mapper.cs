using Entities;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Helpers
{
    public static class Mapper
    {
        public static Entities.ScenarioInfo MapScenarioInfo(Models.ScenarioInfo scenarioInfoModel)
        {
            var scenarioInfo = new Entities.ScenarioInfo
            {
                Name = scenarioInfoModel.Name,
                Pedestrians = MapModelToEntity<Entities.Pedestrian, Models.Pedestrian>(scenarioInfoModel.Pedestrians.ToList()),
                MapURL = scenarioInfoModel.MapURL,
                WorldOptions = MapModelToEntity(scenarioInfoModel.WorldOptions),
                EgoVehicle = MapModelToEntity<Entities.Ego, Models.Ego>(scenarioInfoModel.EgoVehicle),
                Vehicles = MapModelToEntity<Entities.Vehicle, Models.Vehicle>(scenarioInfoModel.Vehicles.ToList())
            };

            return scenarioInfo;
        }

        private static Entities.WorldOptions MapModelToEntity(Models.WorldOptions model)
        {
            return new Entities.WorldOptions(
                model.RainIntersity,
                model.FogIntensity,
                model.SunIntensity,
                model.CloudState,
                model.PrecipitationTypes,
                model.PrecipitationIntensity
            );
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
