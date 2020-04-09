namespace PINDomain.Interfaces
{
    public interface IPINGenerator
    {
        string GetPIN();
        bool PINIsValid(string pin);
    }
}
