using System.IdentityModel.Tokens.Jwt;

namespace IkJetApp.Helpers
{
    public class JwtHelper
    {
        public static string GetClaimValueFromToken(string token, string claimType)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claim = jwtToken?.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
