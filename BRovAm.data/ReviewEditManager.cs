using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRovAm.data
{
    public class ReviewEditManager
    {
        private string _connectionString;

        public ReviewEditManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Order GetOrderByCustomerId(int cId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Orders WHERE CustomerID = @cId";
                command.Parameters.AddWithValue("@cId", cId);
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

        public IEnumerable<Order> GetAllOrders()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Orders";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Order> orders = new List<Order>();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        OrderID = (int)reader["OrderID"],
                        CustomerID = (int)reader["CustomerID"],
                        OrderDate = (DateTime)reader["OrderDate"],
                        TotalCost = (decimal)reader["TotalCost"]
                    });
                }
                return orders;
            }
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByOrderId(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM OrderDetails WHERE OrderID = @oId";
                command.Parameters.AddWithValue("@oId", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                while (reader.Read())
                {
                    orderDetails.Add(new OrderDetail
                    {
                        OrderDetailID = (int)reader["OrderDetailID"],
                        OrderID = (int)reader["OrderID"],
                        ProductID = (int)reader["ProductID"],
                        ColorID = (int)reader["ColorID"],
                        Price = (decimal)reader["Price"],
                        SizeID = (int)reader["SizeID"],
                        Quantity = (int)reader["Quantity"]
                    });
                }
                return orderDetails;
            }
        }

        public IEnumerable<OrderDetail> GetOrderDetailsByItemCode(int id, int pId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM OrderDetails WHERE OrderID = @oId AND ProductID = @pId";
                command.Parameters.AddWithValue("@oId", id);
                command.Parameters.AddWithValue("@pId", pId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                while (reader.Read())
                {
                    orderDetails.Add(new OrderDetail
                    {
                        OrderDetailID = (int)reader["OrderDetailID"],
                        OrderID = (int)reader["OrderID"],
                        ProductID = (int)reader["ProductID"],
                        ColorID = (int)reader["ColorID"],
                        Price = (decimal)reader["Price"],
                        SizeID = (int)reader["SizeID"],
                        Quantity = (int)reader["Quantity"]
                    });
                }
                return orderDetails;
            }
        }

        public OrderDetail GetOrderDetailById(int odId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM OrderDetails WHERE OrderDetailID = @oId";
                command.Parameters.AddWithValue("@oId", odId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new OrderDetail
                {
                    OrderDetailID = (int)reader["OrderDetailID"],
                    OrderID = (int)reader["OrderID"],
                    ProductID = (int)reader["ProductID"],
                    ColorID = (int)reader["ColorID"],
                    Price = (decimal)reader["Price"],
                    SizeID = (int)reader["SizeID"],
                    Quantity = (int)reader["Quantity"]
                };
            }
        }

        public void UpdateQuantity(int qty, int odId)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET Quantity = @qty WHERE OrderDetailID = @id";
                command.Parameters.AddWithValue("@qty", qty);
                command.Parameters.AddWithValue("@id", odId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateColor(int colorCode, int odId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET ColorID = @cId WHERE OrderDetailID = @id";
                command.Parameters.AddWithValue("@cId", colorCode);
                command.Parameters.AddWithValue("@id", odId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void UpdateSize(int sizeCode, int odId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE OrderDetails SET SizeID = @sId WHERE OrderDetailID = @id";
                command.Parameters.AddWithValue("@sId", sizeCode);
                command.Parameters.AddWithValue("@id", odId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteOrderDetail(int odId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM OrderDetails  WHERE OrderDetailID = @id";
                command.Parameters.AddWithValue("@id", odId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
