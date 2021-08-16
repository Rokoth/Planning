//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;

namespace Planning.Contract.Model
{
    /// <summary>
    /// Filter class
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    public abstract class Filter<T> : IFilter<T> where T : IEntity
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="size">Page size</param>
        /// <param name="page">Page number</param>
        /// <param name="sort">Sort field</param>
        public Filter(int? size, int? page, string sort)
        {
            Size = size;
            Page = page;
            Sort = sort;
        }
        /// <summary>
        /// Page size
        /// </summary>
        public int? Size { get; }
        /// <summary>
        /// Page number
        /// </summary>
        public int? Page { get; }
        /// <summary>
        /// Sort field
        /// </summary>
        public string Sort { get; }
    }

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

    public class ProjectHistoryFilter : Filter<ProjectHistory>
    {
        public ProjectHistoryFilter(int size, int page, string sort, string name,
            DateTimeOffset? changedDateBegin, DateTimeOffset? changedDateEnd, Guid? id, Guid userId) : base(size, page, sort)
        {
            Name = name;

            ChangedDateBegin = changedDateBegin;
            ChangedDateEnd = changedDateEnd;
            Id = id;
            UserId = userId;
        }
                
        /// <summary>
        /// Used date filter
        /// </summary>
        public DateTimeOffset? ChangedDateBegin { get; set; }
        /// <summary>
        /// Used date filter
        /// </summary>
        public DateTimeOffset? ChangedDateEnd { get; set; }
        /// <summary>
        /// Name filter
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// parent filter
        /// </summary>
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
    }

    public interface IFilter<T> where T : IEntity
    {
        int? Page { get; }
        int? Size { get; }
        string Sort { get; }
    }
}
