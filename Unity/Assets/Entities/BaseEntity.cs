namespace Entities
{
    public class BaseEntity
    {
        public BaseEntity(int id, Coord3D spawnPoint)
        {
            Id = id;
            SpawnPoint = spawnPoint;
        }

        public int Id { get; set; }
        public Coord3D SpawnPoint { get; set; }
    }

}