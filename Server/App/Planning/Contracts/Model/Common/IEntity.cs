using System;

namespace Contracts.Model.Common
{
    public interface IEntity
    {
        Guid Id { get; set; }        
    }
}