namespace OctDailyApi.Models
{
    public class Product
    {
        // Unique identifier for each product
        public int Id { get; set; }

        // Name of the product
        public string Name { get; set; }

        // Description of the product
        public string Description { get; set; }

        // Price of the product
        public decimal Price { get; set; }

        // Stock quantity available
        public int Quantity { get; set; }
    }

}
