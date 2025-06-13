//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref 1
using System.Threading.Tasks;

namespace Planning.Common
{
    public interface IErrorNotifyService
    {
        Task Send(string message, MessageLevelEnum level = MessageLevelEnum.Error, string title = null);
    }
}
