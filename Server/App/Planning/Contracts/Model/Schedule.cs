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
        [Required(ErrorMessage = "Поле должно быть установлено")]
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
        [Required(ErrorMessage = "Поле должно быть установлено")]
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
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid ProjectId { get; set; }
        [Display(Name = "ИД пользователя")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid UserId { get; set; }       
        [Display(Name = "Дата начала")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        public DateTimeOffset BeginDate { get; set; }        
        [Display(Name = "Наименование проекта")]
        public string Project { get; set; }
        [Display(Name = "Установить дату начала")]
        public bool SetBeginDate { get; set; }
    }

    public class ScheduleUpdater: IEntity
    {
        [Display(Name = "ИД")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid Id { get; set; }
        [Display(Name = "ИД проекта")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid ProjectId { get; set; }
        [Display(Name = "ИД пользователя")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid UserId { get; set; }
        [Display(Name = "Дата начала")]
        public DateTimeOffset BeginDate { get; set; }
        [Display(Name = "Наименование проекта")]
        public string Project { get; set; }
    }

    /// <summary>
    /// Filter for user model
    /// </summary>
    public class ScheduleFilter : Filter<Schedule>
    {
        public ScheduleFilter(int size, int page, string sort, string name, Guid? projectId, Guid userId) : base(size, page, sort)
        {
            Name = name;
            UserId = userId;
            ProjectId = projectId;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }
        public Guid UserId { get; set; }
        public Guid? ProjectId { get; set; }
    }

    public class ScheduleHistoryFilter : Filter<ScheduleHistory>
    {
        public ScheduleHistoryFilter(int size, int page, string sort, string name, Guid? id) : base(size, page, sort)
        {
            Name = name;
            Id = id;
        }
        public string Name { get; }
        public Guid? Id { get; }
    }
}
