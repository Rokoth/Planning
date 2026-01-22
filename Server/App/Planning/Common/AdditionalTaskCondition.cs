using System.ComponentModel;

namespace Planning.Common
{
    public enum AdditionalTaskCondition
    {
        [Description("При добавлении в расписание")]
        OnAdd = 1,

        [Description("При завершении")]
        OnClose = 2,

        [Description("При отсрочке")]
        OnPostpone = 3,

        [Description("При закрытии")]
        OnDelete = 4
    }
}
