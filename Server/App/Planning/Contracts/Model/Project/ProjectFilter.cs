//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;

namespace Contracts.Model.Project
{

    public class ProjectFilter : Filter<Project>
    {
        public ProjectFilter(Guid? userId, int? size, int? page, string sort, string name, 
            bool? isLeaf, DateTimeOffset? lastUsedDateBegin, DateTimeOffset? lastUsedDateEnd, Guid? parentId, string path) : base(size, page, sort)
        {
            Name = name;
            Path = path;
            IsLeaf = isLeaf;
            LastUsedDateBegin = lastUsedDateBegin;
            LastUsedDateEnd = lastUsedDateEnd;
            ParentId = parentId;
            UserId = userId;
        }
       
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
        public string Path { get; set; }
        /// <summary>
        /// parent filter
        /// </summary>
        public Guid? ParentId { get; set; }
        public Guid? UserId { get; set; }
    }
}
