//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1

using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Common
{
    /// <summary>
    /// Базовый класс моделей
    /// </summary>
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// Идентификтаор
        /// </summary>
        [Display(Name = "Идентификатор")]
        public Guid Id { get; set; }
        /// <summary>
        /// Дата последнего изменения
        /// </summary>
        [Display(Name = "Дата последнего изменения")]
        public DateTimeOffset VersionDate { get; set; }       
    }
}
