//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;

namespace Planning.Contract.Model
{
    /// <summary>
    /// Проект
    /// </summary>
    public class Project : Entity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Наименование для ФС
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Родитель
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Является ли листом дерева
        /// </summary>
        public bool IsLeaf { get; set; }        
        /// <summary>
        /// Дата последнего добавления в расписание
        /// </summary>
        public DateTimeOffset? LastUsedDate { get; set; }
        /// <summary>
        /// Период действия в расписании (в минутах)
        /// </summary>
        public int? Period { get; set; } 
        /// <summary>
        /// Приоритет (0-10000)
        /// </summary>
        public int Priority { get; set; }        
    }

    /// <summary>
    /// Проект
    /// </summary>
    public class ProjectHistory : EntityHistory
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Наименование для ФС
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Родитель
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Является ли листом дерева
        /// </summary>
        public bool IsLeaf { get; set; }
        /// <summary>
        /// Дата последнего добавления в расписание
        /// </summary>
        public DateTimeOffset? LastUsedDate { get; set; }
        /// <summary>
        /// Период действия в расписании (в минутах)
        /// </summary>
        public int? Period { get; set; }
        /// <summary>
        /// Приоритет (0-10000)
        /// </summary>
        public int Priority { get; set; }
    }

    public class ProjectCreator
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Наименование для ФС
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Родитель
        /// </summary>
        public Guid? ParentId { get; set; }        
        /// <summary>
        /// Период действия в расписании (в минутах)
        /// </summary>
        public int? Period { get; set; }
        /// <summary>
        /// Приоритет (0-10000)
        /// </summary>
        public int Priority { get; set; }
    }

    public class ProjectUpdater : ProjectCreator, IEntity
    { 
       public Guid Id { get; set; }
    }
}
