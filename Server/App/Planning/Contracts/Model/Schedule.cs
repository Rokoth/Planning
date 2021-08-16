//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;
using System.ComponentModel.DataAnnotations;

namespace Planning.Contract.Model
{
    public class Schedule : Entity
    {       
        [Display(Name = "ИД проекта")]        
        public Guid ProjectId { get; set; }
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Порядковый комер")]
        public int Order { get; set; }
        [Display(Name = "Дата начала")]
        public DateTimeOffset BeginDate { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTimeOffset EndDate { get; set; }
        [Display(Name = "Текущий")]
        public bool IsRunning { get; set; }
        [Display(Name = "Наименование проекта")]
        public string Project { get; set; }
    }

    public class ScheduleHistory : EntityHistory
    {
        [Display(Name = "ИД проекта")]
        public Guid ProjectId { get; set; }
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Порядковый комер")]
        public int Order { get; set; }
        [Display(Name = "Дата начала")]
        public DateTimeOffset BeginDate { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTimeOffset EndDate { get; set; }
        [Display(Name = "Текущий")]
        public bool IsRunning { get; set; }
        [Display(Name = "Наименование проекта")]
        public string Project { get; set; }
    }

    public class ScheduleCreator
    {
        [Display(Name = "ИД проекта")]
        public Guid ProjectId { get; set; }
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Порядковый комер")]
        public int Order { get; set; }
        [Display(Name = "Дата начала")]
        public DateTimeOffset BeginDate { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTimeOffset EndDate { get; set; }
        [Display(Name = "Текущий")]
        public bool IsRunning { get; set; }
        [Display(Name = "Наименование проекта")]
        public string Project { get; set; }
    }

    public class ScheduleUpdater
    {
        [Display(Name = "ИД")]
        public Guid Id { get; set; }
        [Display(Name = "ИД проекта")]
        public Guid ProjectId { get; set; }
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Порядковый комер")]
        public int Order { get; set; }
        [Display(Name = "Дата начала")]
        public DateTimeOffset BeginDate { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTimeOffset EndDate { get; set; }
        [Display(Name = "Текущий")]
        public bool IsRunning { get; set; }
        [Display(Name = "Наименование проекта")]
        public string Project { get; set; }
    }
}
