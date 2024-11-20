using Npgsql;
public class Product
{
    public int Id;
    public string Name;
    public decimal Price;
    public int Quantity;
    public int SellerId;

    public static IEnumerable<Product> ViewProducts(string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = "SELECT * FROM products";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Quantity = reader.GetInt32(3),
                SellerId = reader.GetInt32(4)
            };
        }
    }

    public static void AddProduct(string name, decimal price, int quantity, int seller_id, string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = "INSERT INTO products (name, price, quantity, seller_id) VALUES (@name, @price, @quantity, @seller_id)";
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("price", price);
        cmd.Parameters.AddWithValue("quantity", quantity);
        cmd.Parameters.AddWithValue("seller_id", seller_id);
        cmd.ExecuteNonQuery();
    }
}
