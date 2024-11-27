****Product Management API's****

**Motivation & Problem Solved:**
The goal of this project was to create a straightforward and scalable API for managing product data.
It addresses the need for efficient product management in applications, providing functionality for adding, updating, deleting,
retrieving products, and managing product stock levels. The project emphasizes best practices in validation, error handling, and response structure,
while also incorporating dependency injection and service-layer design for better maintainability.
By leveraging Entity Framework Core (EF Core), the project simplifies database interactions by mapping C# models to database tables.
This allows for seamless CRUD operations and eliminates the need for raw SQL. EF Core also supports migrations,
making it easy to update the database schema as the application evolves. This combination of tools helps keep product management organized and efficient.

The following technologies were used: 
**ASP.NET Core 9.0:** For building the RESTful API.
**Entity Framework Core (EF Core):** Simplifies database operations with ORM, enabling easy data access.
**Dependency Injection (DI):** Promotes loose coupling by injecting services, making the code more maintainable and testable.
**Serilog:** For error logging and monitoring. Swagger: For easy API testing and documentatio as well in Multiserver Arichtecture.
**C#:** The language for implementing business logic and API endpoints. Semaphore (Multithreading): Manages concurrent resource access, ensuring thread safety in multithreaded environments.
**Global Exception Handling:** Handles unhandled exceptions globally, ensuring smooth error management across the API.

Explored technologies over Developing:
**Redis:** Used for fast, in-memory data storage and caching to improve performance and scalability. 
**RabbitMQ:** A message broker used for managing communication between services in a distributed system, ensuring reliable messaging.
**Rowversion:** Used in database management to handle concurrency control, ensuring data integrity in multi-user environments.
**Custom Exception Handling through Middleware:** Allows centralized error handling and customization of error responses in the application pipeline.
Difference Between Normal Try-Catch and Global Exception Handling Multi-server Architecture: Involves distributing workloads across multiple servers to improve scalability, fault tolerance, and load balancing in distributed systems.

**Endpoints :**

Post: 
**../api/products Description:** Adds a new product to the inventory (tbl_Products DB). Input: json - Body { "ProductName": "Example Product", -->String "StockAvailable": 100 --> Int } Output: 200 OK if successfully added. 400 Bad Request if input validation fails. -->Input validation Fails when StockAvailable <=0

Get:
1> **../api/products** Description: Retrieves a list of all products from tbl_Products DB. Output: 200 OK with list of products. 404 Not Found if no products are available.

2> **../api/products/{id}** Description: Retrieves a product by its ID. Input: id (Product ID) Output: 200 OK with product details. 404 Not Found if product is not found.

DELETE **../api/products/{id**} Description: Deletes a product by its ID. Input: id (Product ID) Output: 200 OK if successfully deleted. 404 Not Found if product is not found.

PUT 1 > **../api/products/{id}** Description: Updates the product details. Input: json --Body { "ProductName": "Updated Product", "StockAvailable": 200 } Output: 200 OK if updated successfully. 404 Not Found if product not found --> If Product ID was not Exist

2> **../api/products/decrement-stock/{id}/{quantity}** Description: Decreases the stock of a product. Input: id (Product ID) quantity (Amount to decrement) Output: 200 OK : if Required Stock Quantity Exist : Stock Decremented successfully if Required Stock Quantity Not Exist : Insufficient stock available. Please select a quantity within the available stock 404 Not Found if product not found. -- If Product ID was not Exist

3> **../api/products/increment-stock/{id}/{quantity}** Description: Increases the stock of a product. Input: id (Product ID) quantity (Amount to increment) Output: 200 OK if Stock Incremented successfully. 404 Not Found if product not found. -->If Product ID was not Exist

**Improvisation Scope:**

**Message and Redis Adaptation:** Implement Redis to support multi-server hosting, ensuring high-volume load handling and preventing data inconsistency and race conditions during concurrent processing.
**Authentication and Authorization:** Integrate robust authentication and authorization mechanisms to secure communication and prevent unauthorized access.
**SSL Certificate for Secure Communication:** Adopt SSL certificates to encrypt communication and ensure data security during transmission.
**Modularization of the Project:** Split the project into multiple layered components to enhance consistency, enabling smoother collaboration among developers and reducing the risk of affecting other parts of the project during development.

**Steps to Access the Product Management API Application**
1. **Clone the Repository**
Open your terminal or Git Bash and clone the repository using the following command:
git clone https://github.com/SUMI3747/ProductsAPIEndpoints.git
2. **Update the Database Server Name**
Navigate to the project directory and update the database connection string in the appsettings.json file.
Replace your_server_name with your client machine's SQL Server instance name.
3. **Run Migration Commands**
update-database (Pacakage manager Console)
4. **Access the Rest API**
navigate to the Swagger UI for testing the endpoints

Thank You..


Regards,
Sumit L
Ph: 8123886338






