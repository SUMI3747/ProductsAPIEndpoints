using Microsoft.EntityFrameworkCore;
using Moq;
using PrdouctsApi.Data;
using PrdouctsApi.Models.DTOs;
using PrdouctsApi.Models.Entities;
using PrdouctsApi.ProductServices;
using ProductInventoryManagerAPI.ProductServices;

namespace ProductInventoryManagerAPI.Tests.ProductServices
{
    internal class ProductServiceTests
    {
        private ProductDbContext _dbContext;
        private Mock<IStockUpdateService> _stockUpdateServiceMock;
        private Products _productObject;
        private ProductService _productService;

        [SetUp]
        public void SetUp()
        {
            // Set up in-memory database and mock dependencies before each test
            ProductDbContext _dbContext = CreateInMemoryDbContext();
            _stockUpdateServiceMock = new Mock<IStockUpdateService>();

            var _respObject = new ProductRequestResponse
            {
                statusMessage = string.Empty,
                productDetails = null
            };
            _productObject = new Products();

            _productService = new ProductService(this._dbContext, _stockUpdateServiceMock.Object, _respObject, _productObject);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task AddProductAsync_WhenProductDoesNotExist_ShouldAddNewProductToDB()
        {
            //Arrange
            var newProduct = new Products
            {
                ProductName = "TestProduct",
                StockAvailable = 10
            };

            // Act
            var response = await _productService.AddProductAsync(newProduct.ProductName, newProduct.StockAvailable);
            var productInDb = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == "TestProduct");

            
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(productInDb);
                Assert.That(productInDb.StockAvailable, Is.EqualTo(10));
                Assert.That(response.statusMessage, Is.EqualTo("New Product Added successfully"));
                Assert.That(response.productDetails.ProductName, Is.EqualTo("TestProduct"));
                Assert.That(response.productDetails.StockAvailable, Is.EqualTo(10));
                Assert.That(response.productDetails.ProductId, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetAllProductsAsync_WhenCalled_GetAllProductDetails()
        {
            // Act
            await AddDummyProductsForToTest();

            // Assert
            var products = await _productService.GetAllProductsAsync();

            // Assert
            
            Assert.Multiple(() =>
            {
                Assert.That(products.Count, Is.EqualTo(2));
                Assert.That(products[0].ProductName, Is.EqualTo("Product1"));
                Assert.That(products[1].ProductName, Is.EqualTo("Product2"));
            });
        }

        [Test]
        public async Task GetProductByIdAsync_WhenCalledWithProductId_GetOnlyThatProductDetails()
        {
            // Act
            await AddDummyProductsForToTest();

            // Assert
            var products = await _productService.GetProductByIdAsync(100001);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(products.ProductName, Is.EqualTo("Product1"));
                Assert.That(products.StockAvailable, Is.EqualTo(10));
            });
        }

        [Test]
        public async Task DeleteProductByIdAsync_WhenCalledWithProductId_ShouldDeleteThatProductIdDetailsFromDb()
        {
            // Act
            await AddDummyProductsForToTest();

            // Assert
            await _productService.DeleteProductByIdAsync(100002);
            var productInDb = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == 100002);

            // Assert
            Assert.IsNull(productInDb);
        }

        [Test]
        public async Task DeleteProductByIdAsync_WhenCalled_ShouldDeletePerticularRecordProductCountShouldGetReducedByOne()
        {
            // Act
            await AddDummyProductsForToTest();

            // Assert
            var beforeDelete_Products = await _productService.GetAllProductsAsync();
            await _productService.DeleteProductByIdAsync(100002);
            var afterDelete_Products = await _productService.GetAllProductsAsync();

            // Assert
            Assert.AreNotEqual(beforeDelete_Products.Count, afterDelete_Products.Count);
        }

        [Test]
        public async Task UpdateProductAsync_WhenCalledToUpdateProductDeatilById_ShouldUpdateProductDetailsAccordingToNewValue()
        {
            // Act
            await AddDummyProductsForToTest();
            ProductDto updatedProductDetails_FromBody = new ProductDto { ProductName = "Product x", StockAvailable = 100 };

            // Assert
            var afterUpdate_productDetails = await _productService.UpdateProductAsync(100002, updatedProductDetails_FromBody);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(afterUpdate_productDetails.productDetails.ProductName, Is.EqualTo("Product x"));
                Assert.That(afterUpdate_productDetails.productDetails.StockAvailable, Is.EqualTo(100));
                Assert.That(afterUpdate_productDetails.statusMessage, Is.EqualTo("Product updated successfully"));
            });
        }

        [Test]
        public async Task UpdateProductAsync_PriductIdCoundNotFound_ShouldReturnProdductNotFoundResponse()
        {
            // Act
            await AddDummyProductsForToTest();
            ProductDto updatedProductDetails_FromBody = new ProductDto { ProductName = "Product x", StockAvailable = 100 };

            // Assert
            var afterUpdate_productDetail = await _productService.UpdateProductAsync(100005, updatedProductDetails_FromBody);

            // Assert
            Assert.That(afterUpdate_productDetail.statusMessage, Is.EqualTo("Product not found"));
        }

        private ProductDbContext CreateInMemoryDbContext()
        {
            // Configure In-memory database
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ProductDbContext(options);
            return _dbContext;
        }

        private async Task AddDummyProductsForToTest()
        {
            var product1 = new Products { ProductId = 100001, ProductName = "Product1", StockAvailable = 10 };
            var product2 = new Products { ProductId = 100002, ProductName = "Product2", StockAvailable = 20 };
            await _dbContext.Products.AddAsync(product1);
            await _dbContext.Products.AddAsync(product2);
            await _dbContext.SaveChangesAsync();
        }
    }

}

