//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref 1

namespace Planning.Common
{
    public class Response<TResp> where TResp : class
    {
        public ResponseEnum ResponseCode { get; set; }
        public TResp ResponseBody { get; set; }
    }
}
