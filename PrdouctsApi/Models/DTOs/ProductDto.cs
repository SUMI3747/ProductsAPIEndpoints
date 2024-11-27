using System.ComponentModel.DataAnnotations;

namespace PrdouctsApi.Models.DTOs
{
    //Used to get Data From Payload
    public class ProductDto
    {
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "StockAvailable is required.")]
        public int StockAvailable { get; set; }
    }
}
