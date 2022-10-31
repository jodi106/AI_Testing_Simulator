using Dtos;

namespace Models
{
    public class BaseModel
    {
        private static int autoIncrementId = 0;
        public BaseModel()
        {
            Id = autoIncrementId++;
        }

        public BaseModel(Coord3D spawnPoint)
        {
            Id = autoIncrementId++;
            SpawnPoint = spawnPoint;
        }

        public BaseModel(int id, Coord3D spawnPoint)
        {
            Id = id;
            SpawnPoint = spawnPoint;
        }

        public int Id { get; set; }
        public Coord3D SpawnPoint { get; set; }
    }

}