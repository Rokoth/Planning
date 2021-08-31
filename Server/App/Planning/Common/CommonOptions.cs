//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref 1
namespace Planning.Common
{
    /// <summary>
    /// Options
    /// </summary>
    public class CommonOptions
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// client's working mode
        /// </summary>
        public ClientMode ClientMode { get; set; }
        /// <summary>
        /// options for error' notify lib
        /// </summary>
        public ErrorNotifyOptions ErrorNotifyOptions { get; set; }
    }
}
