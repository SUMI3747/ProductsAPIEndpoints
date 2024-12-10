using ProductInventoryManagerAPI.Models.DTOs;
using System.Security.Claims;

namespace ProductInventoryManagerAPI.ProductServices
{
    public interface ITokenGenerationService
    {
        Task<string> AddNewCredetialsToDB(UserCredetialsDto userCredetialsDto);

        Task<string> ReturnJWTTokenIfCredetialsAreValid(LoginDto loginDto);
    }
}
