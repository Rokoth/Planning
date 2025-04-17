//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Direction
{
    public class DirectionProjectUpdater : IEntity
    {
        [Display(Name = "ИД")]
        [Required(ErrorMessage = "Поле должно быть установлено")]
        public Guid Id { get; set; }
        [Display(Name = "Направление")]
        public Guid DirectionId { get; set; }
        [Display(Name = "Проект")]
        public Guid ProjectId { get; set; }
    }
}
