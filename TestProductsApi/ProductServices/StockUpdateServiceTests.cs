using Microsoft.EntityFrameworkCore;
using PrdouctsApi.Data;
using PrdouctsApi.Models.DTOs;
using PrdouctsApi.Models.Entities;
using ProductInventoryManagerAPI.ProductServices;


namespace ProductInventoryManagerAPI.Tests.ProductServices
{
    [TestFixture]
    public class StockUpdateServiceTests
    {
        private ProductDbContext _dbContext;
        private StockUpdateService _stockUpdateService;

        [SetUp]
        public void SetUp()
        {
            ProductDbContext _dbContext = CreateInMemoryDbContext();

            // Create Dummy Test Data
            _dbContext.Products.Add(new Products
            {
                ProductId = 1,
                ProductName = "Test Product",
                StockAvailable = 10
            });

            var response = new ProductRequestResponse
            {
                statusMessage = string.Empty,
                productDetails = null
            };
            _dbContext.SaveChanges();

            // Initialize the service
            _stockUpdateService = new StockUpdateService(_dbContext, response);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task ModifyStock_WhenProductNotFoundByProductId_ShouldReturnsErrorResponse()
        {
            // Act
            var response = await _stockUpdateService.ModifyStock(99, 5, true);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(response.statusMessage, Is.EqualTo("Product not found"));
                Assert.That(response.productDetails, Is.Null);
            });
        }

        [Test]
        public async Task ModifyStock_WhenIncrementStockAvailableQuantity_ShouldRetunSuccessfulStatusMessageAndUpdateProductDeatils()
        {
            // Act
            var response = await _stockUpdateService.ModifyStock(1, 5, true);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(response.statusMessage, Is.EqualTo("Stock Incremented successfully"));
                Assert.That(response.productDetails.StockAvailable, Is.EqualTo(15));
            });
        }


        [Test]
        public async Task ModifyStock_WhenDecrementStockCalledInsufficientStock_ShouldRetunSuccessfulStatusMessageAndDoNotDeductProductStockQuantity()
        {
            // Act
            var response = await _stockUpdateService.ModifyStock(1, 15, false);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(response.statusMessage, Is.EqualTo("Insufficient stock available. Please select a quantity within the available stock.")); // This Hardcode string Respose Should move to string varibale that need comapree but for readability purpose i am checking Direclty
                Assert.That(response.productDetails.StockAvailable, Is.EqualTo(10));
            });
        }

        [Test]
        public async Task ModifyStock_SufficientStockAvailable_ShouldRetunSuccessfulStatusResponseMessageAndUpdateProductDeatils()
        {
            // Act
            var response = await _stockUpdateService.ModifyStock(1, 10, false);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(response.statusMessage, Is.EqualTo("Stock Decremented successfully"));
                Assert.That(response.productDetails.StockAvailable, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task ModifyStock_WhenIncrementAndDecrementStockRequestHitAtSamatime_ShouldProcessRequestOneByOneUpdateFinalUpdateProductDeatils()
        {
            // Act
            await _stockUpdateService.ModifyStock(1, 10, true);
            var end_responce = await _stockUpdateService.ModifyStock(1, 15, false);

            Assert.Multiple(() =>
            {

                // Assert
                Assert.That(end_responce.statusMessage, Is.EqualTo("Stock Decremented successfully"));
                Assert.That(end_responce.productDetails.StockAvailable, Is.EqualTo(5));
            });
        }


        [Test]
        public async Task ModifyStock_WhenFourTasksAreCalledConcurrently_ShouldProcessRequestsOneByOneAndMaintainDataConsistency()
        {
            // Arrange
            var productId = 1;

            // Act: Simulate 4 concurrent requests
            var tasks = new List<Task>();
            tasks.Add(Task.Run(() => _stockUpdateService.ModifyStock(productId, 10, true)));
            tasks.Add(Task.Run(() => _stockUpdateService.ModifyStock(productId, 10, false)));
            tasks.Add(Task.Run(() => _stockUpdateService.ModifyStock(productId, 10, false)));
            tasks.Add(Task.Run(() => _stockUpdateService.ModifyStock(productId, 7, true)));
            Task.WaitAll([.. tasks]);

            var stockDetails = await _stockUpdateService.ModifyStock(1, 0, true);
           
            // Assert
            Assert.That(stockDetails.productDetails.StockAvailable, Is.EqualTo(7));
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
    }
}




