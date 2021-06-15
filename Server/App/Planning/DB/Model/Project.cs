﻿using System;

namespace Planning.DB.Context
{
    public class Project : Entity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsLeaf { get; set; }
        public DateTimeOffset? LastUsedDate { get; set; }
        public int? Period { get; set; } //In minutes
        public int Priority { get; set; }
    }

    public class ProjectHistory : EntityHistory
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsLeaf { get; set; }
        public DateTimeOffset? LastUsedDate { get; set; }
        public int? Period { get; set; } //In minutes
        public int Priority { get; set; }
    }
}
