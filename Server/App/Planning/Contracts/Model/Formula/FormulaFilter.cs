//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System;

namespace Contracts.Model.Formula
{
    /// <summary>
    /// filter for formula entity
    /// </summary>
    public class FormulaFilter : Filter<Formula>
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="sort"></param>
        /// <param name="name"></param>
        /// <param name="isDefault"></param>
        public FormulaFilter(int? size, int? page, string sort, string name, bool? isDefault) : base(size, page, sort)
        {
            Name = name;
            IsDefault = isDefault;
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// По умолчанию
        /// </summary>
        public bool? IsDefault { get; }
    }

    public class Formula : Entity
    {
        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckName", "Formula", ErrorMessage = "Name is not valid.")]
        public string Name { get; set; }
        [Display(Name = "Формула")]
        public string Text { get; set; }
        [Display(Name = "По умолчанию")]
        public bool IsDefault { get; set; }
    }

    public class FormulaHistory : EntityHistory
    {
        [Display(Name = "Наименование")]
        public string Name { get; set; }
        [Display(Name = "Формула")]
        public string Text { get; set; }
        [Display(Name = "По умолчанию")]
        public bool IsDefault { get; set; }
    }

    public class FormulaCreator
    {
        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckName", "Formula", ErrorMessage = "Name is not valid.")]
        public string Name { get; set; }
        [Display(Name = "Формула")]
        public string Text { get; set; }
        [Display(Name = "По умолчанию")]
        public bool IsDefault { get; set; }
    }

    public class FormulaUpdater : IEntity
    {
        public Guid Id { get; set; }
        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckNameEdit", "Formula", ErrorMessage = "Name is not valid.", AdditionalFields = "Id")]
        public string Name { get; set; }
        [Display(Name = "Формула")]
        public string Text { get; set; }
        [Display(Name = "По умолчанию")]
        public bool IsDefault { get; set; }
    }
}
