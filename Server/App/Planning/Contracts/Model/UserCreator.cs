//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;
using System.ComponentModel.DataAnnotations;

namespace Planning.Contract.Model
{
    /// <summary>
    /// User Creator model
    /// </summary>
    public class UserCreator
    {
        /// <summary>
        /// Имя
        /// </summary>
        [Display(Name = "Имя")]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [Display(Name = "Описание")]
        public string Description { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        [Display(Name = "Логин")]
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "ИД формулы")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid FormulaId { get; set; }

        [Display(Name = "Формула")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string Formula { get; set; }

        [Display(Name = "Тип построения расписания")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [EnumDataType(typeof(ScheduleMode))]
        public ScheduleMode ScheduleMode { get; set; }
        [Display(Name = "Количество элементов (для типа по кол-ву)")]
        public int? ScheduleCount { get; set; }
        [Display(Name = "Промежуток расписания (для типа по времени)")]
        public int? ScheduleTimeSpan { get; set; } // hours       
        [Display(Name = "Время задачи по умолчанию")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public int DefaultProjectTimespan { get; set; }
        [Display(Name = "Только листовые элементы")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public bool LeafOnly { get; set; }
        [Display(Name = "Сдвиг расписания (в мин)")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public int ScheduleShift { get; set; }
    }
    public enum ScheduleMode
    {
        [Display(Name = "Вручную")] Manual = 0,
        [Display(Name = "По количеству")] ByCount = 1,
        [Display(Name = "По времени")] ByTimeSpan = 2
    }
}
