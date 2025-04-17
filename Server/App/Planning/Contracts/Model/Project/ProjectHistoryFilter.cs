//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;

namespace Contracts.Model.Project
{
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
}
