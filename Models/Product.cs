namespace productCatalogAPI.Models
{
    public class Product
    {
        public int Id{get; set; }
        //required was added so deserializer itself rejects a missing Name before the controller even runs
        public  required string Name{get; set; }
        public string? Description{get; set; }
        public double Price{get; set; }
    }
}