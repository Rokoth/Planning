//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;

namespace Planning.DB.Attributes
{
    /// <summary>
    /// Атрибут Имя таблицы
    /// </summary>
    public class TableNameAttribute : Attribute
    {
        /// <summary>
        /// наименование таблицы в базе данных
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name">наименование таблицы в базе данных</param>
        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }
}
