//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Direction
{
    public class DirectionHistory : EntityHistory
    {
        [Display(Name = "Наименование направления")]
        public string Name { get; set; }
        [Display(Name = "Описание направления")]
        public string Description { get; set; }
        [Display(Name = "Категория направлений")]
        public Guid DirectionCategoryId { get; set; }

        [Display(Name = "ИД пользователя")]
        public Guid UserId { get; set; }
        [Display(Name = "Приоритет направления")]
        public int Priority { get; set; }
        [Display(Name = "Дата начала направления")]
        public DateTime? BeginDate { get; set; }
    }
}
