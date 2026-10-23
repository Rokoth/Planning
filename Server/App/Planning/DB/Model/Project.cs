using Planning.DB.Attributes;
using Planning.DB.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planning.DB.Model
{
    /// <summary>
    /// Модель проектов
    /// </summary>
    [TableName("project")]
    public class Project : Entity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [ColumnName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Путь
        /// </summary>
        [ColumnName("path")]
        public string Path { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        [ColumnName("parent_id")]
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Листовой элемент
        /// </summary>
        [ColumnName("is_leaf")]
        public bool IsLeaf { get; set; }

        /// <summary>
        /// Последняя дата выбора
        /// </summary>
        [ColumnName("last_used_date")]
        public DateTimeOffset? LastUsedDate { get; set; }

        /// <summary>
        /// Период
        /// </summary>
        [ColumnName("period")]
        public int? Period { get; set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        [ColumnName("priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [ColumnName("userid")]        
        public Guid UserId { get; set; }

        /// <summary>
        /// Добавочное  время
        /// </summary>
        [ColumnName("add_time")]
        public int AddTime { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        [ForeignKey("ParentId")]
        [Ignore]
        public Project Parent { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [ForeignKey("UserId")]
        [Ignore]
        public User User { get; set; }

        /// <summary>
        /// Элемаенты расписания
        /// </summary>
        [Ignore]
        public List<Schedule> Schedules { get; set; }
    }
}
