namespace EXAM_ASP_NET.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}