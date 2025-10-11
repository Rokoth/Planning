//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Direction
{
    public class Direction : Entity
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

    public class DirectionCategoryFilter : Filter<DirectionCategory>
    {
        public DirectionCategoryFilter(int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }
    }

    public class DirectionFilter : Filter<Direction>
    {
        public DirectionFilter(int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }
    }

    public class DirectionProjectFilter : Filter<DirectionProject>
    {
        public DirectionProjectFilter(int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }
    }
}
