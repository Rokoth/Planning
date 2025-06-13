using Planning.Common;
using Planning.DB.Attributes;
using System;

namespace Planning.DB.Context
{
    [TableName("additional_task")]
    public class AdditionalTask : Entity
    {
        [ColumnName("name")]
        public string Name { get; set; }

        [ColumnName("type_id")]
        public AdditionalTaskType TypeId { get; set; }

        [ColumnName("condition_id")]
        public AdditionalTaskCondition ConditionId { get; set; }

        [ColumnName("task_data")]
        public string TaskData { get; set; }

        [ColumnName("project_id")]
        public Guid ProjectId { get; set; }
    }
}
