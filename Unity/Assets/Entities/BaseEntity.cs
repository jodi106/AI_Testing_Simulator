namespace Entity
{
    public class BaseEntity
    /// <summary>Creates BaseEntity Object which contains Coord3D SpawnPoint for entities (Veh, Ped)</summary>
    {
        public BaseEntity()
        {
        }

        public BaseEntity(int id)
        {
            Id = id;
        }

        public BaseEntity(int id, Location spawnPoint)
        {
            Id = id;
            SpawnPoint = spawnPoint;
        }

        public BaseEntity( Location spawnPoint, double initialSpeed)
        {
            SpawnPoint = spawnPoint;
            InitialSpeed = initialSpeed;
        }

        public BaseEntity(int id, Location spawnPoint, double initialSpeed)
        {
            Id = id;
            SpawnPoint = spawnPoint;
            InitialSpeed = initialSpeed;
        }

        public int Id { get; set; }
        public Location SpawnPoint { get; protected set; }
        public double InitialSpeed { get; set; }
    }
}