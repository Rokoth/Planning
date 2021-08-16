//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1
namespace Planning.Contract.Model
{
    /// <summary>
    /// Filter for user model
    /// </summary>
    public class UserFilter : Filter<User>
    {
        public UserFilter(int size, int page, string sort, string name) : base(size, page, sort)
        {
            Name = name;
        }
        /// <summary>
        /// User Name
        /// </summary>
        public string Name { get; }
    }
}
