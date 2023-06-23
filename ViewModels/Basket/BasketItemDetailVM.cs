namespace ETIT.ViewModels.Basket
{
    public class BasketItemDetailVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public double Price { get; set; }
        public string CompanyName { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
        public int Count { get; set; }
    }
}
