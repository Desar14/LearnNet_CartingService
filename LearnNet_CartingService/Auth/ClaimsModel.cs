using System.Security.Claims;

namespace LearnNet_CartingService.Auth
{
    public class ClaimModel
    {
        public string _issuer { get; set; }
        public string _type { get; set; }
        public string _value { get; set; }
        public string _valueType { get; set; }
    }
}
