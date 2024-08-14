namespace Idams.Core.Enums
{
    public class ResultCodeMessages
    {
        private readonly static Dictionary<ErrorCode, string> ErrorCodes;
        static ResultCodeMessages()
        {
            Dictionary<ErrorCode, string> dictionary = new Dictionary<ErrorCode, string>();
            dictionary.Add(ErrorCode.TokenEmpty, "Invalid Token");
            dictionary.Add(ErrorCode.ExpiredToken, "Token is Expired");
            dictionary.Add(ErrorCode.Forbidden, "Permission denied");
            ErrorCodes = dictionary;
        }
        public static string GetResultCode(string code)
        {
            return ErrorCodes[(ErrorCode)Enum.Parse(typeof(ErrorCode), code)];
        }
        public static string GetResultCode(ErrorCode code)
        {
            return ErrorCodes[code];
        }

    }
}
