//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref 1
using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    /// <summary>
    /// Common entity class
    /// </summary>
    public abstract class Entity : IEntity
    {
        [ColumnName("id")]
        public Guid Id { get; set; }
        [ColumnName("is_deleted")]
        public bool IsDeleted { get; set; }
        [ColumnName("version_date")]
        public DateTimeOffset VersionDate { get; set; }
    }
}
