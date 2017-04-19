using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRovAm.data
{
    public class OrdersManager
    {
        private string _connectionString;

        public OrdersManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<OrderWithCustomer> AllOrders()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT o.OrderID, o.CustomerID, c.FirstName, c.LastName, c.PhoneNumber, o.OrderDate,o.TotalCost "
                   + "FROM Orders o JOIN Costumers c on o.CustomerID = c.CustomerId";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<OrderWithCustomer> orders = new List<OrderWithCustomer>();
                while (reader.Read())
                {
                    orders.Add(new OrderWithCustomer
                    {
                        OrderID = (int)reader["OrderID"],
                        CustomerID = (int)reader["CustomerID"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        PhoneNumber = (string)reader["PhoneNumber"],
                        OrderDate = (DateTime)reader["OrderDate"],
                        TotalCost = (decimal)reader["TotalCost"]
                    });
                }
                return orders;
            }
        }

        public IEnumerable<OrderDetailExpanded> GetOrderDetailsByOrderId(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT od.OrderDetailID, od.OrderID, p.ItemCode, p.Description, c.Color, s.Size, od.Quantity, od.Price "
                + "FROM OrderDetails od JOIN Products p ON od.ProductID = p.ProductId JOIN Colors c ON  od.ColorID = c.ColorId JOIN Sizes s ON od.SizeID = s.SizeId "
                + "WHERE OrderID = @oId";
                command.Parameters.AddWithValue("@oId", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<OrderDetailExpanded> orderDetails = new List<OrderDetailExpanded>();
                while (reader.Read())
                {
                    orderDetails.Add(new OrderDetailExpanded
                    {
                        OrderDetailID = (int)reader["OrderDetailID"],
                        OrderID = (int)reader["OrderID"],
                        ItemCode = (string)reader["ItemCode"],
                        Description = (string)reader["Description"],
                        Color = (string)reader["Color"],
                        Price = (decimal)reader["Price"],
                        Size = (string)reader["Size"],
                        Quantity = (int)reader["Quantity"]
                    });
                }
                return orderDetails;
            }
        }

        public Customer GetCustomerById(int cId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Costumers WHERE CustomerID = @cId";
                command.Parameters.AddWithValue("@cId", cId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Customer
                {
                    CustomerID = (int)reader["CustomerId"],
                    FirstName = (string)reader["FirstName"],
                    LastNmae = (string)reader["LastName"],
                    Address = (string)reader["Address"],
                    PhoneNumber = (string)reader["PhoneNumber"]
                };
            }
        }
    }
}
