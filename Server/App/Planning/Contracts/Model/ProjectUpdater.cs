//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace Planning.Contract.Model
{
    public class ProjectUpdater : IEntity
    {
        [Display(Name = "ИД")]       
        public Guid Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        [Display(Name = "Наименование")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckNameEdit", "Project", ErrorMessage = "Имя уже используется", AdditionalFields = "Id,ParentId")]
        public string Name { get; set; }
        /// <summary>
        /// Наименование для ФС
        /// </summary>
        [Display(Name = "Путь")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        [Remote("CheckPathEdit", "Project", ErrorMessage = "Путь уже используется", AdditionalFields = "Id,ParentId")]
        public string Path { get; set; }
        /// <summary>
        /// Родитель
        /// </summary>
        [Display(Name = "ИД родителя")]
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Является ли листом дерева
        /// </summary>
        [Display(Name = "Листовой элемент")]
        public bool IsLeaf { get; set; }
        /// <summary>
        /// Дата последнего добавления в расписание
        /// </summary>
        [Display(Name = "Дата последнего добавления в расписание")]
        public DateTimeOffset? LastUsedDate { get; set; }
        /// <summary>
        /// Период действия в расписании (в минутах)
        /// </summary>
        [Display(Name = "Период действия")]
        public int? Period { get; set; }
        /// <summary>
        /// Приоритет (0-10000)
        /// </summary>
        [Display(Name = "Приоритет")]
        public int Priority { get; set; }
        /// <summary>
        /// Дополнительное время
        /// </summary>
        [Display(Name = "Дополнительное время")]
        public int AddTime { get; set; }
        /// <summary>
        /// Родитель
        /// </summary>
        [Display(Name = "Родитель")]
        public string Parent { get; set; }

        public Guid UserId { get; set; }
    }

    
}
