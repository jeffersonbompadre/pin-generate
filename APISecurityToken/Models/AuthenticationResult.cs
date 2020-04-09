namespace APISecurityToken.Models
{
    public class AuthenticationResult
    {
        public AuthenticationResult(string token, bool pinIsValid, string message, AuthenticationData authenticationData)
        {
            Token = token;
            PINIsValid = pinIsValid;
            Message = message;
            AuthenticationData = authenticationData;
        }

        public string Token { get; set; }
        public bool PINIsValid { get; set; }
        public string Message { get; set; }
        public AuthenticationData AuthenticationData { get; set; }
    }
}
