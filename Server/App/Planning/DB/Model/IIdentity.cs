namespace Planning.DB.Context
{
    public interface IIdentity
    {
        string Login { get; set; }
        byte[] Password { get; set; }
    }
}
