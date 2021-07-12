//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System;

namespace Planning.Contract.Model
{
    public class ProjectUpdater : ProjectCreator, IEntity
    { 
       public Guid Id { get; set; }
    }

    
}
