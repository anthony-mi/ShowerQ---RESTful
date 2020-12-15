namespace ShowerQ.Services
{
    public interface IPhoneNumberFormatter
    {
        string ConvertToInternationalFormat(string phoneNumber, string region);
    }
}
