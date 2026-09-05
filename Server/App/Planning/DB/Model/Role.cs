using Planning.DB.Attributes;

namespace Planning.DB.Context
{
    public class Role : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("description")]
        public string Description { get; set; }
    }
}
