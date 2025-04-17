//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
namespace Contracts.Model.Common
{
    public interface IFilter<T> where T : IEntity
    {
        int? Page { get; }
        int? Size { get; }
        string Sort { get; }
    }
}
