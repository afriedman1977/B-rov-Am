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
                command.CommandText = "INSERT INTO Orders(CustomerID,OrderDate,TotalCost) "
                    + "VALUES(@custId,@date,@cost);SELECT @@IDENTITY";
                command.Parameters.AddWithValue("@custId", customerID);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                command.Parameters.AddWithValue("@cost", 0);
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
                    TotalCost = (decimal)reader["TotalCost"]
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
                return (int)(decimal)command.ExecuteScalar();
            }
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

        public decimal AddQuantityToOrderDetail(int orderId, int qty)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET Quantity = @qty WHERE OrderDetailID = @oId";
                command.Parameters.AddWithValue("@qty", qty);
                command.Parameters.AddWithValue("@oId", orderId);
                connection.Open();
                command.ExecuteNonQuery();
            }
            OrderDetail od = GetOrderDetail(orderId);
            decimal price = od.Quantity * od.Price;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET Price = @pr WHERE OrderDetailID = @oId";
                command.Parameters.AddWithValue("@pr", price);
                command.Parameters.AddWithValue("@oId", orderId);
                connection.Open();
                command.ExecuteNonQuery();
            }
            return price;
        }
    }
}
