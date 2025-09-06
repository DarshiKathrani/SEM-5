namespace GroceryMvc.Models
{
    public class CustomerModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }

        public string Password {  get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
