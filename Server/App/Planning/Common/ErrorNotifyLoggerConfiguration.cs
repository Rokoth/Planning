//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref 1

using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Planning.Common
{
    public class ErrorNotifyLoggerConfiguration
    {
        public int EventId { get; set; }

        public ErrorNotifyOptions Options { get; set; }

        public List<LogLevel> LogLevels { get; set; } = new List<LogLevel>()
        {
            LogLevel.Error,
            LogLevel.Critical
        };
    }
}
