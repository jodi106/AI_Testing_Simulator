namespace Models
{
    public class BaseModel
    {
        private static int autoIncrementId = 0;
        public BaseModel()
        {
            Id = autoIncrementId++;
        }

        public BaseModel(Location spawnPoint)
        {
            Id = autoIncrementId++;
            SpawnPoint = spawnPoint;
        }

        public BaseModel(int id, Location spawnPoint)
        {
            Id = id;
            SpawnPoint = spawnPoint;
        }

        public int Id { get; set; }
        public Location SpawnPoint { get; set; }
    }

}