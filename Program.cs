public class VendingMachine
{
    private static readonly string connectionString = "Host=localhost;Username=;Password=;Database=vendingmachinedb";
    public static void Main()
    {   int currentUserId = 0;
        User currentUser = null;
        Console.WriteLine("Welcome to the vending machine!");

        while (true)
        {
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. View Products");
            Console.WriteLine("4. Add Product");
            Console.WriteLine("5. Buy Product");
            Console.WriteLine("6. Exit");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter username:");
                    string? username = Console.ReadLine();
                    Console.WriteLine("Enter password:");
                    string? password = Console.ReadLine();
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        User? user = User.Login(username, password, connectionString);
                        if (user != null)
                        {
                            Console.WriteLine("Login successful!");
                            currentUserId = user.Id;
                            currentUser = user;
                        }

                        Console.WriteLine(user?.Username);
                        Console.WriteLine(user?.Id);
                    }
                    else
                    {
                        Console.WriteLine("Username and password cannot be empty.");
                    }
                    break;

                case "2":
                    Console.WriteLine("Enter username:");
                    string? newUsername = Console.ReadLine();
                    Console.WriteLine("Enter password:");
                    string? newPassword = Console.ReadLine();
                    Console.WriteLine("Enter balance:");
                    string? newBalance = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newUsername) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(newBalance))
                    {
                        if (decimal.TryParse(newBalance, out decimal balance))
                        {
                            User.Register(newUsername, newPassword, balance, connectionString);
                            Console.WriteLine("User registered!");
                        }
                        else
                        {
                            Console.WriteLine("Balance must be a number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Username and password cannot be empty.");
                    }
                    break;

                case "3":
                    foreach (Product product in Product.ViewProducts(connectionString))
                    {
                        Console.WriteLine($"{product.Id} {product.Name} {product.Price} {product.Quantity}");
                    }
                    break;

                case "4":
                    Console.WriteLine("Enter product name:");
                    string? productName = Console.ReadLine();
                    Console.WriteLine("Enter product price:");
                    string? productPrice = Console.ReadLine();
                    Console.WriteLine("Enter product quantity:");
                    string? productQuantity = Console.ReadLine();
                    if (!string.IsNullOrEmpty(productName) && !string.IsNullOrEmpty(productPrice) && !string.IsNullOrEmpty(productQuantity))
                    {
                        if (decimal.TryParse(productPrice, out decimal price) && int.TryParse(productQuantity, out int quantity))
                        {
                            if (currentUserId == 0)
                            {
                                Console.WriteLine("You must be logged in to add a product.");
                                break;
                            }
                            Product.AddProduct(productName, price, quantity, currentUserId, connectionString);
                            Console.WriteLine("Product added!");
                        }
                        else
                        {
                            Console.WriteLine("Price and quantity must be numbers.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Name, price and quantity cannot be empty.");
                    }
                    break;
                case "5":
                    Console.WriteLine("Enter product id:");
                    string? productId = Console.ReadLine();
                    Console.WriteLine("Enter quantity:");
                    string? productQuantityToBuy = Console.ReadLine();
                    if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(productQuantityToBuy))
                    {
                        if (int.TryParse(productId, out int id) && int.TryParse(productQuantityToBuy, out int quantity))
                        {
                            if (currentUserId == 0)
                            {
                                Console.WriteLine("You must be logged in to buy a product.");
                                break;
                            }
                            Product product = Product.ViewProducts(connectionString).FirstOrDefault(product => product.Id == id);
                            if (product != null)
                            {
                                decimal totalPrice = product.Price * quantity;
                                if (currentUser.Balance < totalPrice)
                                {
                                    Console.WriteLine("Not enough balance.");
                                    break;
                                }
                                Transaction.AddTransaction(currentUserId, id, quantity, totalPrice, connectionString);
                                Console.WriteLine("Transaction successful!");
                            }
                            else
                            {
                                Console.WriteLine("Product not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Product id and quantity must be numbers.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Product id and quantity cannot be empty.");
                    }
                    break;
            }
        }
    }
}


