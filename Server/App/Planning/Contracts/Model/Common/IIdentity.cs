//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1

namespace Contracts.Model.Common
{
    public interface IIdentity
    {
        string Login { get; set; }
        string Password { get; set; }
    }
}
