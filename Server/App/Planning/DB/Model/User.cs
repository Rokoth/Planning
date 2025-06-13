using Planning.DB.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planning.DB.Context
{
    [TableName("user")]
    public class User : Entity, IIdentity
    {
        [ColumnName("name")]
        public string Name { get; set; }
        [ColumnName("description")]
        public string Description { get; set; }
        [ColumnName("login")]
        public string Login { get; set; }
        [ColumnName("password")]
        public byte[] Password { get; set; }
        [ColumnName("formula_id")]       
        public Guid FormulaId { get; set; }

        [ForeignKey("FormulaId")]
        [Ignore]
        public Formula Formula { get; set; }
        [Ignore]
        public List<Project> Projects { get; set; }
        [Ignore]
        public List<Schedule> Schedules { get; set; }
    }
}
