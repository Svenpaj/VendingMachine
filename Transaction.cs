using Npgsql;
public class Transaction
{
    public int Id;
    public int BuyerId;
    public int ProductId;
    public int Quantity;
    public decimal TotalPrice;
    public DateTime TransactionDate;

    public static void AddTransaction(int buyerId, int productId, int quantity, decimal totalPrice, string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;

        cmd.CommandText = "SELECT quantity FROM products WHERE id = @product_id";
        cmd.Parameters.AddWithValue("product_id", productId);

        using var reader = cmd.ExecuteReader();
        reader.Read();
        if (reader.HasRows)
        {
            int productQuantity = reader.GetInt32(0);
            if (productQuantity < quantity)
            {
                Console.WriteLine("Not enough stock.");
                return;
            }
        }
        else
        {
            Console.WriteLine("Product not found.");
            return;
        }

        reader.Close();

        cmd.CommandText = "INSERT INTO transactions (userid, productid, quantity, totalprice, transactiondate) VALUES (@buyer_id, @product_id, @quantity, @total_price, @transaction_date)";
        cmd.Parameters.AddWithValue("buyer_id", buyerId);
        cmd.Parameters.AddWithValue("product_id", productId);
        cmd.Parameters.AddWithValue("quantity", quantity);
        cmd.Parameters.AddWithValue("total_price", totalPrice);
        cmd.Parameters.AddWithValue("transaction_date", DateTime.Now);
        cmd.ExecuteNonQuery();

        cmd.CommandText = "UPDATE products SET quantity = quantity - @quantity WHERE id = @product_id";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "UPDATE users SET balance = balance - @total_price WHERE id = @buyer_id";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SELECT seller_id FROM products WHERE id = @product_id";
        using var reader2 = cmd.ExecuteReader();
        reader2.Read();
        int sellerId = 0;
        if (reader2.HasRows)
        {
            sellerId = reader2.GetInt32(0);
        }
        reader2.Close();

        if (sellerId != 0)
        {
            cmd.CommandText = "UPDATE users SET balance = balance + @total_price WHERE id = @seller_id";
            cmd.Parameters.AddWithValue("seller_id", sellerId);
            cmd.ExecuteNonQuery();
        }
        else
        {
            Console.WriteLine("Transaction failed.");
        }
    }

    }