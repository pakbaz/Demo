namespace OrderProduct
{
    public interface IProduct
    {
        int Id { get; set; }
        string Name { get; set; }
        decimal Price { get; set; }
        decimal ShippingCost { get; set; }
    }

    public interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        decimal Balance { get; set; }
        List<(IProduct product, int quantity)> Orders { get; set; }
    }


    public interface ICompany
    {
        List<(IProduct product, int quantity)> Products { get; set; }
        List<IUser> Users { get; set; }
        void MakeOrder(List<(IProduct product, int quantity)> products, IUser user);
        void AddProduct(IProduct product, int quantity);
        void AddUser(IUser user);
    }

    class Product : IProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal ShippingCost { get; set; }

        public Product(int id, string name, int price, int shippingCost)
        {
            Id = id;
            Name = name;
            Price = price;
            ShippingCost = shippingCost;
        }
    }

    class User : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public List<(IProduct product, int quantity)> Orders { get; set; }

        public User(int id, string name, decimal balance)
        {
            Id = id;
            Name = name;
            Balance = balance;
            Orders = new List<(IProduct product, int quantity)>();
        }
    }


    public class Company : ICompany
    {
        public Company()
        {
            Products = new();
            Users = new();
        }
        public List<(IProduct product, int quantity)> Products { get; set; }
        public List<IUser> Users { get; set; }
        public void MakeOrder(List<(IProduct product, int quantity)> orderItems, IUser user)
        {
            //If Quantity is in stock
            //total value must not exceed total account balance
            decimal totalCost = 0;
            decimal highestShippingCost = 0;

            foreach (var item in orderItems)
            {
                var productInfo = Products.Find(p => p.product.Id == item.product.Id);
                if (productInfo.product == null)
                {
                    throw new Exception($"product with ID {item.product.Id} not found");
                }
                if (productInfo.quantity < item.quantity)
                {
                    throw new Exception($"Not enough stock for product {item.product.Name}");
                }
                totalCost += item.product.Price * item.quantity;
                if (item.product.ShippingCost > highestShippingCost)
                {
                    highestShippingCost = item.product.ShippingCost;
                }
            }

            totalCost += highestShippingCost;

            if (user.Balance < totalCost)
            {
                throw new Exception("User does not have enough fund");
            }

            //Order taking place
            user.Balance -= totalCost;
            foreach (var orderItem in orderItems)
            {
                var index = Products.FindIndex(p => p.product.Id == orderItem.product.Id);
                var updatedProductInfo = (Products[index].product, Products[index].quantity - orderItem.quantity);
                Products[index] = updatedProductInfo;
            }

        }
        public void AddProduct(IProduct product, int quantity)
        {
            //TODO: Checks
            Products.Add((product,quantity));
        }
        public void AddUser(IUser user)
        {
            Users.Add(user);
        }
    }


    class Program
    {
        public static void Main()
        {
            var c = new Company();

            // add two products
            c.AddProduct(new Product(id: 1, name: "product1", price: 20, shippingCost: 2), 20);
            c.AddProduct(new Product(id: 2, name: "product2", price: 30, shippingCost: 1), 10);

            // add a user
            c.AddUser(new User(1, "user1", 500));
        
            List<(IProduct product, int quantity)> order = new();

            var p1 = c.Products.Find(p => p.product.Id == 1).product;
            var p2 = c.Products.Find(p => p.product.Id == 2).product;

            // create an order for two products
            order.Add((p1, 5));
            order.Add((p2, 2));


            // generate the order for a user
            var user1 = c.Users.Find(u => u.Id == 1);
            if (user1 != null) {
                c.MakeOrder(order, user1);
                
            }            
        }
    }
}