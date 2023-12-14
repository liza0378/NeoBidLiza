using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NeoBid.Server.Data;
using NeoBid.Server.Data.Entities;
using NeoBid.Server.Data.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static NeoBid.Server.Data.Helpers.AccountHelper;

namespace NeoBid.Server.Auth
{
    public class AuthenticationService(IDbContextFactory<ApplicationDbContext> dbFactory, IConfiguration configuration, ProtectedSessionStorage protectedSessionStorage)
    {
        public event Action<ClaimsPrincipal>? UserChanged;
        private ClaimsPrincipal? currentUser;

        public ClaimsPrincipal CurrentUser
        {
            get { return currentUser ?? new(); }
            set
            {
                currentUser = value;

                if (UserChanged is not null)
                {
                    UserChanged(currentUser);
                }
            }
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipal()
        {
            try
            {
                var token = await protectedSessionStorage.GetAsync<string>("token");

                if (!token.Success)
                    return new();

                var principal = GetPrincipalFromExpiredToken(token.Value);

                return principal;
            }
            catch
            {
                return new();
            }
        }

        public async Task<Account?> GetCurrentAccount()
        {
            var token = await protectedSessionStorage.GetAsync<string>("token");

            if (!token.Success)
                return null;

            var principal = GetPrincipalFromExpiredToken(token.Value);

            var userEmailClaim = principal.FindFirst(ClaimTypes.Email);

            return await AccountHelper.GetAccountByEmail(dbFactory, userEmailClaim.Value);
        }

        public async Task<bool> SignIn(string login, string password)
        {
            var account = await AccountHelper.SignIn(dbFactory, login, password);

            if(account == null)
                return false;

            var claims = GetClaims(account);

            var identity = new ClaimsIdentity(
                claims,
                "Custom Authentication");

            var newUser = new ClaimsPrincipal(identity);

            CurrentUser = newUser;

            var token = GenerateJwtToken(claims);

            await protectedSessionStorage.SetAsync("token", token);

            return true;
        }

        public async Task SignOut()
        {
            await protectedSessionStorage.DeleteAsync("token");

            CurrentUser = null;
        }

        private Claim[] GetClaims(Account account)
            => new []
            {
                new Claim(ClaimTypes.Name, account.FullName),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role.ToString()),
            };

        private string GenerateJwtToken(Claim[] claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Issuer"],
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
