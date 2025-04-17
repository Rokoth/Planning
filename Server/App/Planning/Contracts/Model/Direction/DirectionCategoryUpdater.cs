//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Direction
{
    public class DirectionCategoryUpdater : IEntity
    {
        [Display(Name = "ИД")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid Id { get; set; }
        [Display(Name = "Наименование категории направлений")]
        public string Name { get; set; }
        [Display(Name = "Описание категории направлений")]
        public string Description { get; set; }
        [Display(Name = "Приоритет категории направлений")]
        public int Priority { get; set; }
        
    }
}
