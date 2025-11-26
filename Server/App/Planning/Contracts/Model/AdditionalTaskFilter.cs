//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;

namespace Planning.Contracts.Model
{
    public class AdditionalTaskFilter : Filter<AdditionalTask>
    {
        public AdditionalTaskFilter(int? size, int? page, string sort, string name, Guid projectId) : base(size, page, sort)
        {
            Name = name;
            ProjectId = projectId;
        }
       
        public string Name { get; set; }      
        public Guid? ProjectId { get; set; }
    }
}
