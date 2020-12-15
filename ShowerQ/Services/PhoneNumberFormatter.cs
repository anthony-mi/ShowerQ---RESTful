using PhoneNumbers;

namespace ShowerQ.Services
{
    public class PhoneNumberFormatter : IPhoneNumberFormatter
    {
        public string ConvertToInternationalFormat(string phoneNumber, string region)
        {
            var phoneNumberObj = PhoneNumberUtil.GetInstance().Parse(phoneNumber, region);
            return PhoneNumberUtil.GetInstance().Format(phoneNumberObj, PhoneNumberFormat.INTERNATIONAL);
        }
    }
}
