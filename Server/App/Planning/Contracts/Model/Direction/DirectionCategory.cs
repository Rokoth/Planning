//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Direction
{
    public class DirectionCategory : Entity
    {
        [Display(Name = "Наименование категории направлений")]
        public string Name { get; set; }
        [Display(Name = "Описание категории направлений")]
        public string Description { get; set; }
        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Приоритет категории направлений")]
        public int Priority { get; set; }
    }
}
