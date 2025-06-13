//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using Contracts.Model.Common;
using System;

namespace Contracts.Model.Project
{
    public class Notify : Entity
    {       
        public Guid UserId { get; set; }
       
        public string Text { get; set; }
        
        public bool IsSend { get; set; }
    }
}
