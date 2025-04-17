//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Schedule
{
    public enum ScheduleMode
    {
        [Display(Name = "Вручную")] Manual = 0,
        [Display(Name = "По количеству")] ByCount = 1,
        [Display(Name = "По времени")] ByTimeSpan = 2
    }
}
