//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;
using System.ComponentModel.DataAnnotations;

namespace Planning.Contract.Model
{
    /// <summary>
    /// Базовый класс исторических моделей
    /// </summary>
    public abstract class EntityHistory : Entity
    {
        [Display(Name = "ИД")]
        public long HId { get; set; }
        [Display(Name = "Дата изменения")]
        public DateTimeOffset ChangeDate { get; set; }
        [Display(Name = "Удалённый")]
        public bool IsDeleted { get; set; }
    }
}
