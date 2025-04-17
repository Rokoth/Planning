//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Model.Direction
{
    public class DirectionProjectCreator
    {
        [Display(Name = "Направление")]
        public Guid DirectionId { get; set; }
        [Display(Name = "Проект")]
        public Guid ProjectId { get; set; }
    }
}
