//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1

using Contracts.Model.Common;
using Contracts.Model.Schedule;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.User
{
    public class UserSettings : Entity
    {
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Тип построения расписания")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [EnumDataType(typeof(ScheduleMode))]
        public ScheduleMode ScheduleMode { get; set; }
        [Display(Name = "Количество элементов (для типа по кол-ву)")]
        public int? ScheduleCount { get; set; }
        [Display(Name = "Промежуток расписания (для типа по времени)")]
        public int? ScheduleTimeSpan { get; set; } // hours       
        [Display(Name = "Время задачи по умолчанию")]
        public int DefaultProjectTimespan { get; set; }
        [Display(Name = "Только листовые элементы")]
        public bool LeafOnly { get; set; }
        [Display(Name = "Сдвиг расписания (в мин)")]
        public int ScheduleShift { get; set; }
        [Display(Name = "Пользователь")]
        public string User { get; set; }
    }
}
