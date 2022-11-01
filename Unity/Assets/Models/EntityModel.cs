﻿namespace Models
{
    public class Model
    {
        public Model(int id, string name)
        {
            Id = id;
            Name = name;
            DisplayName = name;
        }
        public Model(int id, string name, string displayName)
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
