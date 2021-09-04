//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;

namespace Planning.DB.Attributes
{
    /// <summary>
    /// Name of column Attribute
    /// </summary>
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; }

        public ColumnNameAttribute(string name)
        {
            Name = name;
        }
    }
}
