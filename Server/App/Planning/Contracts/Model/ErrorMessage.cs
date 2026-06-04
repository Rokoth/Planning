namespace Planning.Contracts.Model
{
    /// <summary>
    /// Моедель сообщения об ошибке
    /// </summary>
    public class ErrorMessage
    {
        public ErrorMessage(string message, string source)
        {
            Message = message;
            Source = source;
        }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Источник (метод) ошибки
        /// </summary>
        public string Source { get; }
    }
}
