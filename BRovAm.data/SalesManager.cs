using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRovAm.data
{
    public class SalesManager
    {
        private string _connectionString;
        public SalesManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddCustomer(Customer c)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Costumers VALUES(@fn,@ln,@ad,@pn);SELECT @@IDENTITY";
                command.Parameters.AddWithValue("@fn", c.FirstName);
                command.Parameters.AddWithValue("@ln", c.LastNmae);
                command.Parameters.AddWithValue("@ad", c.Address);
                command.Parameters.AddWithValue("@pn", c.PhoneNumber);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
            }
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Costumers";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Customer> customers = new List<Customer>();
                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        CustomerID = (int)reader["CustomerId"],
                        FirstName = (string)reader["FirstName"],
                        LastNmae = (string)reader["LastName"],
                        Address = (string)reader["Address"],
                        PhoneNumber = (string)reader["PhoneNumber"]
                    });
                }
                return customers;
            }
        }

        public int CreateOrder(int customerID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Orders(CustomerID,OrderDate,TotalCost,TotalAmountPaid,TotalQuantity) "
                    + "VALUES(@custId,@date,@cost,@paid,@quantity);SELECT @@IDENTITY";
                command.Parameters.AddWithValue("@custId", customerID);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                command.Parameters.AddWithValue("@cost", 0);
                command.Parameters.AddWithValue("@paid", 0);
                command.Parameters.AddWithValue("@quantity", 0);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
            }
        }

        public Order GetOrder(int oId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Orders WHERE OrderID = @oId";
                command.Parameters.AddWithValue("@oId", oId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Order
                {
                    OrderID = (int)reader["OrderID"],
                    CustomerID = (int)reader["CustomerID"],
                    OrderDate = (DateTime)reader["OrderDate"],
                    TotalCost = (decimal)reader["TotalCost"],
                    TotalAmountPaid = (decimal?)reader["TotalAmountPaid"],
                    TotalQuantity = (int?)reader["TotalQuantity"]
                };
            }
        }

        public OrderDetail GetOrderDetail(int odId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM OrderDetails WHERE OrderDetailID = @odId";
                command.Parameters.AddWithValue("@odId", odId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new OrderDetail
                {
                    OrderDetailID = (int)reader["OrderDetailID"],
                    OrderID = (int)reader["OrderID"],
                    ProductID = (int)reader["ProductID"],
                    SizeID = (int)reader["SizeID"],
                    ColorID = (int)reader["ColorID"],
                    Quantity = (int)reader["Quantity"],
                    Price = (decimal)reader["Price"]
                };
            }
        }

        public int CreateOrderDetail(int orderId, int productId, decimal price)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO OrderDetails(OrderID,ProductID,Quantity,Price) "
                    + "VALUES(@orderId,@productId,@qty,@price);SELECT @@IDENTITY";
                command.Parameters.AddWithValue("@orderId", orderId);
                command.Parameters.AddWithValue("@productId", productId);
                command.Parameters.AddWithValue("@qty", 1);
                command.Parameters.AddWithValue("@price", price);
                connection.Open();
                id = (int)(decimal)command.ExecuteScalar();
            }
            UpdateTotalCost(orderId);
            UpdateTotalQuantity(orderId);
            return id;
        }

        public void AddSizeToOrderDetail(int orderId, int sizeId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET SizeID = @sId WHERE OrderDetailID = @oId";
                command.Parameters.AddWithValue("@sId", sizeId);
                command.Parameters.AddWithValue("@oId", orderId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AddColorToOrderDetail(int orderId, int colorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET ColorID = @cId WHERE OrderDetailID = @oId";
                command.Parameters.AddWithValue("@cId", colorId);
                command.Parameters.AddWithValue("@oId", orderId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public decimal AddQuantityToOrderDetail(int orderdetailId, int qty, int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET Quantity = @qty WHERE OrderDetailID = @oId";
                command.Parameters.AddWithValue("@qty", qty);
                command.Parameters.AddWithValue("@oId", orderdetailId);
                connection.Open();
                command.ExecuteNonQuery();
            }
            OrderDetail od = GetOrderDetail(orderdetailId);
            Product product = new BRovAmManager(_connectionString).GetProductById(od.ProductID);
            decimal price = od.Quantity * product.Price;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET Price = @pr WHERE OrderDetailID = @oId";
                command.Parameters.AddWithValue("@pr", price);
                command.Parameters.AddWithValue("@oId", orderdetailId);
                connection.Open();
                command.ExecuteNonQuery();
            }
            UpdateTotalCost(orderId);
            UpdateTotalQuantity(orderId);
            return price;
        }

        public void AddMessageURL(string phoneNumber, string url)
        {
             using (SqlConnection connection = new SqlConnection(_connectionString))
             {
                 SqlCommand command = connection.CreateCommand();
                 command.CommandText = "INSERT INTO Messages(PhoneNumber,MessageURL) VALUES (@phNum,@url)";
                 command.Parameters.AddWithValue("@phNum", phoneNumber);
                 command.Parameters.AddWithValue("@url", url);
                 connection.Open();
                 command.ExecuteNonQuery();
             }
        }

        private void UpdateTotalCost(int orderId)
        {
            decimal price = 0;
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT SUM(Price) FROM OrderDetails WHERE OrderID = @oId";
                command.Parameters.AddWithValue("@oId", orderId);
                connection.Open();
                price = (decimal)command.ExecuteScalar();
            }
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Orders SET TotalCost = @price";
                command.Parameters.AddWithValue("@price", price);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void UpdateTotalQuantity(int orderId)
        {
            int quantity = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT SUM(Quantity) FROM OrderDetails WHERE OrderID = @oId";
                command.Parameters.AddWithValue("@oId", orderId);
                connection.Open();
                quantity = (int)command.ExecuteScalar();
            }
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Orders SET TotalQuantity = @quantity";
                command.Parameters.AddWithValue("@quantity", quantity);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
