﻿namespace Dtos
{
    public class EntityModel
    {
        public EntityModel(int id, string name)
        {
            Id = id;
            Name = name;
            DisplayName = name;
        }
        public EntityModel(int id, string name, string displayName)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }

}