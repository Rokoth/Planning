//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Schedule
{
    public class ScheduleCreator
    {
        [Display(Name = "ИД проекта")]       
        public Guid? ProjectId { get; set; }
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
        public Guid? DirectionId { get; set; }
    }
}
