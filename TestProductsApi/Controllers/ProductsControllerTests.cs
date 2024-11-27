using Microsoft.AspNetCore.Mvc;
using Moq;
using PrdouctsApi.Controllers;
using PrdouctsApi.Helpers;
using PrdouctsApi.Models.DTOs;
using PrdouctsApi.Models.Entities;
using PrdouctsApi.ProductServices;


namespace ProductInventoryManagerAPI.Tests.Controllers
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProductService> _productService;
        private Mock<IProductValidationHelper> _productValidationHelper;
        private Mock<IServiceResponseHelper> _serviceResponseHelper;
        private ProductsController _productsController;

        [SetUp]
        public void SetUp()
        {
            _productService = new Mock<IProductService>();
            _productValidationHelper = new Mock<IProductValidationHelper>();
            _serviceResponseHelper = new Mock<IServiceResponseHelper>();

            _productsController = new ProductsController(
                _productService.Object,
                _productValidationHelper.Object,
                _serviceResponseHelper.Object);
        }

        private BadRequestObjectResult GetBadRequestResult(string errorMessage)
        {
            return new BadRequestObjectResult(errorMessage);
        }

        private OkObjectResult GetOkResult(object response)
        {
            return new OkObjectResult(response);
        }

        [Test]
        public async Task AddProduct_WhenProductDetailsDoesNotExistInBodyData_ShouldReturnBadRequestResponse()
        {
            // Arrange
            ProductDto productDto = null;
            _productValidationHelper
                .Setup(v => v.ValidateProductDto(It.IsAny<ProductDto>()))
                .Returns(GetBadRequestResult("Product data cannot be null"));

            // Act
            var response = await _productsController.AddProduct(productDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual(400, (response as BadRequestObjectResult).StatusCode);
            Assert.AreEqual("Product data cannot be null", (response as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task AddProduct_WhenProductNameIsEmptyInBody_ShouldReturnBadRequestResponse()
        {
            // Arrange
            var productDto = new ProductDto { ProductName = "", StockAvailable = 10 };
            _productValidationHelper
                .Setup(v => v.ValidateProductDto(It.IsAny<ProductDto>()))
                .Returns(GetBadRequestResult("Product name cannot be empty"));

            // Act
            var response = await _productsController.AddProduct(productDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual(400, (response as BadRequestObjectResult).StatusCode);
            Assert.AreEqual("Product name cannot be empty", (response as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task AddProduct_WhenProductStockAvailableIsLessThanOrEqualToZero_ShouldReturnBadRequestResponse()
        {
            // Arrange
            var productDto = new ProductDto { ProductName = "Test", StockAvailable = -5 };
            _productValidationHelper
                .Setup(v => v.ValidateProductDto(It.IsAny<ProductDto>()))
                .Returns(GetBadRequestResult("Enter Stock Quantity must be greater than 0"));

            // Act
            var response = await _productsController.AddProduct(productDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual(400, (response as BadRequestObjectResult).StatusCode);
            Assert.AreEqual("Enter Stock Quantity must be greater than 0", (response as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task AddProduct_WhenProductDetailsAreValid_ShouldReturnOkMessage()
        {
            // Arrange
            var productDto = new ProductDto { ProductName = "Test", StockAvailable = 10 };
            var mockResponse = new ProductRequestResponse
            {
                statusMessage = "New Product Added successfully",
                productDetails = new Products { ProductName = "TestProduct", StockAvailable = 10 }
            };
            _productValidationHelper
                .Setup(v => v.ValidateProductDto(It.IsAny<ProductDto>()))
                .Returns((ActionResult)null);
            _productService
                .Setup(s => s.AddProductAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(mockResponse);
            _serviceResponseHelper
                .Setup(x => x.HandleServiceResponse(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(GetOkResult(mockResponse));

            // Act
            var result = await _productsController.AddProduct(productDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var response = (result as OkObjectResult).Value as ProductRequestResponse;
            Assert.AreEqual("New Product Added successfully", response.statusMessage);
        }

        [Test]
        public async Task GetAllProducts_WhenCalled_ShouldReturnOKResponseIfAnyProductFound()
        {
            // Arrange
            var products = new List<Products>
            {
                new Products { ProductId = 100001, ProductName = "Product1", StockAvailable = 100 },
                new Products { ProductId = 100002, ProductName = "Product2", StockAvailable = 200 }
            };
            _productService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _productsController.GetAllProducts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAllProducts_WhenCalled_ShouldReturnNotFoundIfNoProductsAvailable()
        {
            // Arrange
            _productService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(new List<Products>());

            // Act
            var result = await _productsController.GetAllProducts();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetProductById_WhenCalledWithNonExistingProductIdInDB_ShouldReturnNotFound()
        {
            // Arrange
            int productId = 100003;
            _productService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync((Products)null);

            // Act
            var result = await _productsController.GetProductById(productId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task DeleteProduct_WhenCalledWithExistingProductIdInDB_ShouldReturnOkResponseWithStatusMessage()
        {
            // Arrange
            int productId = 100001;
            _productService.Setup(s => s.DeleteProductByIdAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _productsController.DeleteProduct(productId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteProduct_WhenCalledWithNonExistingProductIdInDB_ShouldReturnNotFound()
        {
            // Arrange
            int productId = 100003;
            _productService.Setup(s => s.DeleteProductByIdAsync(productId)).ReturnsAsync(false);

            // Act
            var result = await _productsController.DeleteProduct(productId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}
