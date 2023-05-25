using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            using var connection = new MySqlConnection(connectionString);
            {
                //Query for creating a category
                var categoryInsert = @"INSERT INTO categories
                        (`categoryName`, `description`, `picture`)
                        VALUES
                        (@CategoryName, @Description, @Picture);";
                {
                    Category newCategory = new Category
                    {
                        CategoryName = "test4",
                        Description = "test4",
                        Picture = "test4"
                    };
                    var rowsAffected = connection.Execute(categoryInsert, newCategory);
                    Console.WriteLine($"{rowsAffected} row(s) inserted.");
                }
                var insertedCategories = connection.Query<Category>("SELECT * FROM categories").ToList();

                //Query for creating a product
                var productInsert = @"INSERT INTO products (productName, supplierId, categoryId,quantityPerUnit, unitPrice,
                                                            unitsInStock, unitsOnOrder, reorderLevel, discontinued, lastUserId, lastDateUpdated)
                                      VALUES (@ProductName, @SupplierId, @CategoryId,@QuantityPerUnit, @UnitPrice,
                                                            @UnitsInStock, @UnitsOnOrder, @ReorderLevel, @Discontinued, @LastUserId, @LastDateUpdated)";
                {
                    Product newProduct = new Product
                    {
                        ProductName = "Product 1",
                        SupplierId = 1,
                        CategoryId = 1,
                        QuantityPerUnit = "10 units per case",
                        UnitPrice = 9.99m,
                        UnitsInStock = 100,
                        UnitsOnOrder = 20,
                        ReorderLevel = 10,
                        Discontinued = 0,
                        LastUserId = 1,
                        LastDateUpdated = DateTime.Now
                    };
                    var rowsAffected = connection.Execute(productInsert, newProduct);
                    Console.WriteLine($"{rowsAffected} row(s) inserted.");
                }
                var insertedProducts = connection.Query<Product>("SELECT * FROM products").ToList();

                //Query for creating an order
                var orderInsert = @"INSERT INTO orders (customerId, employeeId, orderDate, requiredDate, shippedDate, shipVia, freight,
                                                        shipName, shipAddress, shipCity, shipRegion, shipPostalCode, shipCountry)
                                    VALUES (@CustomerId, @EmployeeId, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight,
                                                         @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry)";
                {
                    Order newOrder = new Order
                    {
                        CustomerId = 1,
                        EmployeeId = 1,
                        OrderDate = DateTime.Now.AddDays(-4),
                        RequiredDate = DateTime.Now.AddDays(7),
                        ShippedDate = DateTime.Now.AddDays(3),
                        ShipVia = 1,
                        Freight = 10.5m,
                        ShipName = "John Doe",
                        ShipAddress = "123 Main St",
                        ShipCity = "New York",
                        ShipRegion = "NY",
                        ShipPostalCode = "10001",
                        ShipCountry = "USA"
                    };
                    var rowsAffected = connection.Execute(orderInsert, newOrder);
                    Console.WriteLine($"{rowsAffected} row(s) inserted.");
                }
                var insertedOrders = connection.Query<Order>("SELECT * FROM orders").ToList();

                //Query for getting list of orders sorted by date
                string orderByDate = "SELECT * FROM orders ORDER BY orderDate";
                List<Order> orders = connection.Query<Order>(orderByDate).ToList();

                //Query for getting a list of all products sorted by most sold products
                string orderByMostSoldProducts = "SELECT p.productName, SUM(od.quantity) AS totalQuantitySold " +
                                                    "FROM products p " +
                                                    "INNER JOIN orderDetails od ON p.productId = od.productId " +
                                                    "GROUP BY p.productId " +
                                                    "ORDER BY totalQuantitySold DESC";
                List<MostSoldProduct> mostSoldProducts = connection.Query<MostSoldProduct>(orderByMostSoldProducts).ToList();

                //Query for getting a list of all categories sorted by most sold products
                string orderCategoryByMostSoldProducts = "SELECT c.categoryName, p.productName, SUM(p.UnitsOnOrder) AS QuantitySold " +
                                                        "FROM Categories c " +
                                                        "INNER JOIN Products p ON c.categoryId = p.categoryId " +
                                                        "GROUP BY 1,2 " +
                                                        "ORDER BY QuantitySold DESC ";
                List<CategoryMostSoldProduct> categoriesMostSoldProducts = connection.Query<CategoryMostSoldProduct>(orderCategoryByMostSoldProducts).ToList();
            }
        }
    }
}
