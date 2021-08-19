//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
using System.Threading.Tasks;

namespace Deploy
{
    public interface IDeployService
    {
        Task Deploy(int? num = null);
    }
}
