using ShoppingApp.Models;

namespace ShoppingApp.Data
{
    public class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            Console.WriteLine("Starting SeedData Initialization");
            try
            {
                if (context.Products.Any())
                {
                    return;
                }

                var categories = new Category[]
                {
                new Category{Name="Beauty & Health", Description="For taking care of yourself"},
                new Category{Name="Auto", Description="Vehicle maintenance and supplies"},
                new Category{Name="Apparel", Description="Clothes for every style"},
                new Category{Name="Outdoors", Description="Products for all outdoor hobbies"},
                new Category{Name="Electronics", Description="The latest gadgets"},
                new Category{Name="Home & Garden", Description="Tools for any DIY project"},
                new Category{Name="Books", Description="Literature from around the world"},
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();


                var products = new Product[]
                {
                new Product{Name="Shampoo", CategoryId=1, Price=9.99m, Description="Cleans hair", ImgUrl="/Content/Images/pexels-karolina-grabowska-4465121.jpg"},
                new Product{Name="Vitamin-D", CategoryId=1, Price=5.99m, Description="Important for skin and bone health", ImgUrl="/Content/Images/vitamin_bottle.jpg"},
                new Product{Name="Air Freshener", CategoryId=2, Price=4.99m, Description="Keeps car smelling fresh", ImgUrl="/Content/Images/air_freshener.jpg"},
                new Product{Name="Motor Oil", CategoryId=2, Price=16.49m, Description="Long-lasting, high performance oil", ImgUrl="/Content/Images/motor_oil.jpg"},
                new Product{Name="Beanie", CategoryId=3, Price=11.00m, Description="Keeps your head warm during the winter", ImgUrl="/Content/Images/beanie.jpg"},
                new Product{Name="T-Shirt", CategoryId=3, Price=7.00m, Description="Casual everyday shirt", ImgUrl="/Content/Images/t_shirt.jpg"},
                new Product{Name="Hiking Shoes", CategoryId=4, Price=60.00m, Description="Comfortable shoes for hiking", ImgUrl="/Content/Images/hiking_shoes.jpg"},
                new Product{Name="Frisbee", CategoryId=4, Price=10.50m, Description="Perfect fun for a sunny day", ImgUrl="/Content/Images/frisbee.jpg"},
                new Product{Name="Tablet", CategoryId=5, Price=90.00m, Description="Productivity on the go", ImgUrl="/Content/Images/tablet.jpg"},
                new Product{Name="Digital Camera", CategoryId=5, Price=70.00m, Description="High resolution pictures", ImgUrl="/Content/Images/digital_camera.jpg"},
                new Product{Name="Potting Soil", CategoryId=6, Price=9.99m, Description="Makes plants grow fast", ImgUrl="/Content/Images/potting_soil.jpg"},
                new Product{Name="Coffee Maker", CategoryId=6, Price=49.99m, Description="High quality and convenient", ImgUrl="/Content/Images/coffee_maker.jpg"},
                new Product{Name="Cat in the Hat", CategoryId=7, Price=5.29m, Description="By Doctor Seuss", ImgUrl="/Content/Images/The_Cat_in_the_Hat.png"},
                new Product{Name="The Stranger", CategoryId=7, Price=11.59m, Description="By Albert Camus", ImgUrl="/Content/Images/the_stranger.jpg"},
                };

                context.Products.AddRange(products);
                context.SaveChanges();
                Console.WriteLine("Seeded Data");
            }
            catch (Exception ex)
            {
                Console.WriteLine("SeedData Error: " + ex.Message);
            }

        }
    }
}
