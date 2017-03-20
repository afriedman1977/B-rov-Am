using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRovAm.data
{
    public class BRovAmManager
    {
        private string _connectionString;
        public BRovAmManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Products";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Product> products = new List<Product>();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        StyleNumber = (string)reader["StyleNumber"],
                        Brand = (string)reader["Brand"],
                        Description = (string)reader["Description"],
                        Price = (decimal)reader["Price"],
                        CategoryId = (int)reader["CategoryId"],
                        ItemCode = (string)reader["ItemCode"]
                    });
                }
                return products;
            }
        }

        public IEnumerable<Color> GetAllColors()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Colors";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Color> colors = new List<Color>();
                while (reader.Read())
                {
                    colors.Add(new Color
                    {
                        ColorId = (int)reader["ColorId"],
                        ProductColor = (string)reader["Color"]
                    });
                }
                return colors;
            }
        }

        public IEnumerable<Size> GetAllSizes()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Sizes";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Size> sizes = new List<Size>();
                while (reader.Read())
                {
                    sizes.Add(new Size
                    {
                        SizeId = (int)reader["SizeId"],
                        ProductSize = (string)reader["Size"]
                    });
                }
                return sizes;
            }
        }

        public IEnumerable<Category> GetAllCategories()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Categories";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Category> categories = new List<Category>();
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryId = (int)reader["CategoryId"],
                        CategoryName = (string)reader["CategoryName"],
                        Description = (string)reader["Description"]
                    });
                }
                return categories;
            }
        }

        public IEnumerable<Color> GetColorForProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT c.Color ,c.ColorId FROM Colors c"
                   + " JOIN ProductsColorsSizes pcs"
                   + " ON pcs.ColorId = c.ColorId"
                   + " JOIN Products p"
                   + " ON p.ProductId = pcs.ProductId"
                   + " WHERE p.ProductId = @id"
                   + " GROUP BY c.Color,c.ColorId";
                command.Parameters.AddWithValue(@"id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Color> colors = new List<Color>();
                while (reader.Read())
                {
                    colors.Add(new Color
                    {
                        ColorId = (int)reader["ColorId"],
                        ProductColor = (string)reader["Color"]
                    });
                }
                return colors;
            }
        }

        public IEnumerable<Size> GetSizesForProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT s.Size ,s.SizeId  FROM Sizes s"
                    + " JOIN ProductsColorsSizes pcs"
                    + " ON pcs.SizeId = s.SizeId"
                    + " JOIN Products p"
                    + " ON p.ProductId = pcs.ProductId"
                    + " WHERE p.ProductId = @id"
                    + " GROUP BY s.Size,s.SizeId";
                command.Parameters.AddWithValue(@"id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<Size> sizes = new List<Size>();
                while (reader.Read())
                {
                    sizes.Add(new Size
                    {
                        SizeId = (int)reader["SizeId"],
                        ProductSize = (string)reader["Size"]
                    });
                }
                return sizes;
            }
        }

        public IEnumerable<ProductsColorsSizes> GetAllProductSizeColors()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM ProductsColorsSizes";
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<ProductsColorsSizes> psc = new List<ProductsColorsSizes>();
                while (reader.Read())
                {
                    psc.Add(new ProductsColorsSizes
                    {
                        ProductId = (int)reader["ProductId"],
                        SizeId = (int)reader["SizeId"],
                        ColorId = (int)reader["ColorId"]
                    });
                }
                return psc;
            }
        }

        public int AddProduct(string styleNumber, string brand, string description, decimal price, int categoryId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Products(StyleNumber,Brand,Description,Price,CategoryId) "
                    + "VALUES(@style,@brand,@description,@price,@cId);SELECT @@IDENTITY";
                command.Parameters.AddWithValue("@style", styleNumber);
                command.Parameters.AddWithValue("@brand", brand);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@price", price);
                command.Parameters.AddWithValue("@cId", categoryId);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
                //command.CommandText = "INSERT INTO ProductsColors(ProductId,ColorId) VALUES(@pId,@coId)";
                //command.Parameters.AddWithValue("@pId",productId);
                //command.Parameters.AddWithValue("@coId",colorId);
                //command.ExecuteNonQuery();
                //command.CommandText = "INSERT INTO ProductsSizes(ProductId,SizeId) VALUES(@prId,@sId)";
                //command.Parameters.AddWithValue("@prId", productId);
                //command.Parameters.AddWithValue("@sId", sizeId);
                //command.ExecuteNonQuery();
            }
        }

        public Product GetProductById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Products WHERE ProductID = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                return new Product
                {
                    ProductId = (int)reader["ProductID"],
                    StyleNumber = (string)reader["StyleNumber"],
                    Brand = (string)reader["Brand"],
                    Description = (string)reader["Description"],
                    Price = (decimal)reader["Price"],
                    CategoryId = (int)reader["CategoryId"]
                };
            }
        }

        public void AddColorSizes(List<ProductsColorsSizes> pcs)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                foreach (ProductsColorsSizes p in pcs)
                {
                    SqlCommand command = connection.CreateCommand();
                    if (p.Included)
                    {
                        if (CountOfProductColorSize(p) != 0)
                        {
                            command.CommandText = "UPDATE ProductsColorsSizes SET STOCK = @quantity WHERE ProductId = @pId AND ColorId = @cId AND SizeId = @sId";                            
                        }
                        else
                        {
                            command.CommandText = "INSERT INTO ProductsColorsSizes(ProductId,ColorId,SizeId,Stock) "
                                 + "VALUES(@pId,@cId,@sId,@quantity)"; 
                        }
                    }
                    else
                    {
                        command.CommandText = "DELETE FROM ProductsColorsSizes WHERE ProductId = @pId AND ColorId = @cId AND SizeId = @sId";
                    }
                    command.Parameters.AddWithValue("@pId", p.ProductId);
                    command.Parameters.AddWithValue("@cId", p.ColorId);
                    command.Parameters.AddWithValue("@sId", p.SizeId);
                    command.Parameters.AddWithValue("@quantity", p.Quantity);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EditProduct(Product p)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE Products SET StyleNumber = @style, Brand = @brand, Description = @description,"
                    + " Price = @price, CategoryId = @cid WHERE ProductId = @id";
                command.Parameters.AddWithValue("@style", p.StyleNumber);
                command.Parameters.AddWithValue("@brand", p.Brand);
                command.Parameters.AddWithValue("@description", p.Description);
                command.Parameters.AddWithValue("@price", p.Price);
                command.Parameters.AddWithValue("@cId", p.CategoryId);
                command.Parameters.AddWithValue("@id", p.Id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM ProductsColorsSizes WHERE ProductId = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Products WHERE ProductId = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                command.ExecuteNonQuery();
            } 
        }

        public IEnumerable<ProductsColorsSizes> GetColorsAndSizesForProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM ProductsColorsSizes WHERE ProductId = @id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                List<ProductsColorsSizes> pcs = new List<ProductsColorsSizes>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    pcs.Add(new ProductsColorsSizes
                    {
                        ProductId = (int)reader["ProductId"],
                        ColorId = (int)reader["ColorId"],
                        SizeId = (int)reader["SizeId"],
                        Quantity = (int)reader["Stock"],
                        Included = true
                    });
                }
                return pcs;
            }
        }

        public IEnumerable<ColorSizeQuantity> GetDetailsForProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT c.Color,s.Size,cs.Stock FROM Colors c JOIN ProductsColorsSizes cs ON cs.ColorId = c.ColorId"
                    + " JOIN Sizes s  ON s.SizeId = cs.SizeId WHERE cs.ProductId = @id";
                command.Parameters.AddWithValue("@id", id); 
                connection.Open();
                List<ColorSizeQuantity> csq = new List<ColorSizeQuantity>();
                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    csq.Add(new ColorSizeQuantity
                    {
                        Color = (string)reader["Color"],
                        Size = (string)reader["Size"],
                        Quantity = (int)reader["stock"]
                    });
                }
                return csq;
            } 
        }

        private int CountOfProductColorSize(ProductsColorsSizes p)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM ProductsColorsSizes WHERE ProductId = @id AND ColorId = @cid AND SizeId = @sid";
                command.Parameters.AddWithValue("@id", p.ProductId);
                command.Parameters.AddWithValue("@cid", p.ColorId);
                command.Parameters.AddWithValue("@sid", p.SizeId);
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
    }
}
