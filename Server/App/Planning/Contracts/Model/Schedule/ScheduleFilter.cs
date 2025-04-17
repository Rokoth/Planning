//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;

namespace Contracts.Model.Schedule
{
    /// <summary>
    /// Filter for user model
    /// </summary>
    public class ScheduleFilter : Filter<Schedule>
    {
        public ScheduleFilter(int? size, int? page, string sort, string name
            , Guid? projectId, Guid userId, bool? onlyActive = null, DateTimeOffset? fromDate = null
            , DateTimeOffset? toDate = null) : base(size, page, sort)
        {
            Name = name;
            UserId = userId;
            ProjectId = projectId;
            OnlyActive = onlyActive;
            FromDate = fromDate;
            ToDate = toDate;
           
        }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }
        public Guid UserId { get; set; }
        public Guid? ProjectId { get; set; }
        public bool? OnlyActive { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }       
    }
}
