using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Planning.Contract.Model
{
    public class UserHistory : EntityHistory
    {
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }
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
