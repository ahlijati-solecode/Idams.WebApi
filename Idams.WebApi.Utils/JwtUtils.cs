using System.IdentityModel.Tokens.Jwt;

namespace Idams.WebApi.Utils
{
    public class JwtUtils
    {
        public static bool isEmptyOrInvalid(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return true;
            }

            var jwtToken = new JwtSecurityToken(token);
            return (jwtToken == null) || (jwtToken.ValidFrom.ToUniversalTime() > DateTime.UtcNow) || (jwtToken.ValidTo.ToUniversalTime() < DateTime.UtcNow);
        }
    }
}