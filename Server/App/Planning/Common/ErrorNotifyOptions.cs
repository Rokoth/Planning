//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref 1

namespace Planning.Common
{
    public class ErrorNotifyOptions
    {
        public bool SendError { get; set; }
        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FeedbackContact { get; set; }
        public string DefaultTitle { get; set; }
    }
}
