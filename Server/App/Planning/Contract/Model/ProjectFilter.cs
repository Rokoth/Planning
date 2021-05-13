//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;

namespace Planning.Contract.Model
{
    /// <summary>
    /// Filter for Project model query
    /// </summary>
    public class ProjectFilter
    {
        /// <summary>
        /// Find only leaves of project tree
        /// </summary>
        public bool? IsLeaf { get; set; }        
        /// <summary>
        /// Used date filter
        /// </summary>
        public DateTimeOffset? LastUsedDateBegin { get; set; }
        /// <summary>
        /// Used date filter
        /// </summary>
        public DateTimeOffset? LastUsedDateEnd { get; set; }
        /// <summary>
        /// Name filter
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// parent filter
        /// </summary>
        public Guid? ParentId { get; set; }
    }
}
