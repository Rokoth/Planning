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
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <param name="sort"></param>
        /// <param name="name"></param>
        /// <param name="isDefault"></param>
        public FormulaFilter(int? size, int? page, string sort, string name, bool? isDefault) : base(size, page, sort)
        {
            Name = name;
            IsDefault = isDefault;
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// По умолчанию
        /// </summary>
        public bool? IsDefault { get; }
    }
}
