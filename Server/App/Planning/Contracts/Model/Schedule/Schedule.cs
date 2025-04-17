//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Schedule
{
    public class Schedule : Entity
    {       
        [Display(Name = "ИД проекта")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid ProjectId { get; set; }
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }       
        [Display(Name = "Дата начала")]
        public DateTimeOffset BeginDate { get; set; }
        [Display(Name = "Дата окончания")]
        public DateTimeOffset EndDate { get; set; }
        [Display(Name = "Текущий")]
        public bool IsRunning { get; set; }
        [Display(Name = "Наименование проекта")]
        public string Project { get; set; }
        [Display(Name = "Путь проекта")]
        public string ProjectPath { get; set; }
    }
}
