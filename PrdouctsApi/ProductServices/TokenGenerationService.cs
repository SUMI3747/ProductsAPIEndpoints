using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductInventoryManagerAPI.Data;
using ProductInventoryManagerAPI.Models.DTOs;
using ProductInventoryManagerAPI.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductInventoryManagerAPI.ProductServices
{
    public class TokenGenerationService : ITokenGenerationService
    {
        private readonly IConfiguration _configuration;
        private readonly UserCredetialsContext _context;

        public TokenGenerationService(IConfiguration configuration, UserCredetialsContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> AddNewCredetialsToDB(UserCredetialsDto userCredetialsDto)
        {
            if (await _context.UserCredetials.AnyAsync(user => user.Username == userCredetialsDto.Username))
            {
                return "Employee already exists";
            }

            var newUser = new UserCredetials
            {
                Username = userCredetialsDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCredetialsDto.Password),
                Role = userCredetialsDto.Role.ToLower()
            };

            _context.UserCredetials.Add(newUser);
            await _context.SaveChangesAsync();

            return $"New Employee registration successful. EmployeeID: {newUser.UserId}";
        }

        public async Task<string> ReturnJWTTokenIfCredetialsAreValid(LoginDto loginDto)
        {
            var user = await _context.UserCredetials
                .FirstOrDefaultAsync(uid => uid.UserId == loginDto.UserId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return "Invalid Credentials";
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            return await GenerateToken(claims);
        }

        public async Task<string> GenerateToken(IEnumerable<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
