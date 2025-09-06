namespace GroceryMvc.Models
{
    public class OrderDetailModel
    {
        public int OrderDetailsID {  get; set; }
        public int OrderID { get; set; }
        public int ProductID {  get; set; } 
        public  int Quantity {  get; set; }
        public string Status {  get; set; }
    }
}
