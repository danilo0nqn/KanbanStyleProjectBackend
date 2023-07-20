using KanbanStyleBackEnd.Models.DataModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KanbanStyleBackEnd.Helpers
{
    public static class JwtHelpers
    {
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, Guid id)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("id", userAccounts.Id.ToString()),
                new Claim(ClaimTypes.Email, userAccounts.EmailId),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt"))
            };

            if (userAccounts.UserType == "Admin")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            } else if (userAccounts.UserType == "User")
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                claims.Add(new Claim("UserOnly", "User"));
            }
            return claims;
        }
        public static IEnumerable<Claim> GetClaims(this UserTokens userAccounts, out Guid id)
        {
            id = Guid.NewGuid();
            return GetClaims(userAccounts, id);
        }

        public static UserTokens GenTokenKey(UserTokens model, JwtSettings jwtSettings)
        {
            try
            {
                var userToken = new UserTokens();
                if(model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }
                //Obtain SECRET KEY
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);

                Guid id;

                //Expires in 1 Day
                DateTime expireTime = DateTime.UtcNow.AddDays(1);

                //Validity of our token
                userToken.Validity = expireTime.TimeOfDay;

                //GENERATE OUR TOKEN
                var jwToken = new JwtSecurityToken
                    (
                    issuer: jwtSettings.ValidIssuer,
                    audience: jwtSettings.ValidAudience,
                    claims: GetClaims(model, out id),
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                    expires: new DateTimeOffset(expireTime).DateTime,
                    signingCredentials: new SigningCredentials
                        (
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256
                        )
                    );

                userToken.Token = new JwtSecurityTokenHandler().WriteToken(jwToken);
                userToken.Id = model.Id;
                userToken.UserType = model.UserType;
                userToken.GuidId = id;

                return userToken;
            }
            catch ( Exception exception )
            {
                throw new Exception("Error Generating the JWT", exception);
            }
        }

    }
}
