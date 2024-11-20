using Npgsql;

public class User
{
    public int Id;
    public string Username;
    public string Password;
    public decimal Balance;

    public static User? Login(string username, string password, string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = "SELECT * FROM users WHERE username = @username AND password = @password";
        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("password", password);

        using var reader = cmd.ExecuteReader();
        reader.Read();
        if (reader.HasRows)
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Balance = reader.GetDecimal(3)
            };
        }
        else
        {
            Console.WriteLine("Invalid username or password");
            return null;
        }
    }

    public static void Register(string username, string password, decimal balance, string connectionString)
    {
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = "INSERT INTO users (username, password, balance) VALUES (@username, @password, @balance)";
        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("password", password);
        cmd.Parameters.AddWithValue("balance", balance);

        cmd.ExecuteNonQuery();
    }
}
