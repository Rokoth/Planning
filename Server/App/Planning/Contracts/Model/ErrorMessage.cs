namespace Planning.Contracts.Model
{
    /// <summary>
    /// Моедель сообщения об ошибке
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Источник (метод) ошибки
        /// </summary>
        public string Source { get; set; }
    }
}
