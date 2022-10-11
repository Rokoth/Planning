//Copyright 2021 Dmitriy Rokoth
//Licensed under the Apache License, Version 2.0
//
//ref1

namespace Planning.Contract.Model
{
    /// <summary>
    /// filter for formula entity
    /// </summary>
    public class FormulaFilter : Filter<Formula>
    {
        public FormulaFilter(int size, int page, string sort, string name, bool? isDefault) : base(size, page, sort)
        {
            Name = name;
            IsDefault = isDefault;
        }
        public string Name { get; }
        public bool? IsDefault { get; }
    }
}
