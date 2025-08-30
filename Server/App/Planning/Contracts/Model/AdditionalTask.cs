//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1

using Planning.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Planning.Contracts.Model
{
    public class AdditionalTask : Entity
    {
        [Display(Name = "Наименование дополнительного задания")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public string Name { get; set; }

        [Display(Name = "Тип дополнительного задания")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [EnumDataType(typeof(AdditionalTaskType))]
        public AdditionalTaskType TypeId { get; set; }

        [Display(Name = "Тип сработки дополнительного задания")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [EnumDataType(typeof(AdditionalTaskCondition))]
        public AdditionalTaskCondition ConditionId { get; set; }

        [Display(Name = "Дополнительные данные")]
        public string TaskData { get; set; }

        [Display(Name = "ИД проекта")]
        public Guid ProjectId { get; set; }
    }
}
