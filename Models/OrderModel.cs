using System.ComponentModel.DataAnnotations;

namespace GroceryMvc.Models
{
    public class OrderModel
    {
        public int OrderID {  get; set; }

        [Required(ErrorMessage ="Customer  is required")]
        public  int CustomerID {  get; set; }

        [Required(ErrorMessage ="Order Date is required")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage ="Amount is required")]
        public Decimal TotalAmount {  get; set; }
        public string? CustomerName { get; set; }
        public List<CustomerDropDownModel>? CustomerList { get; set; }

    }

    public class CustomerDropDownModel
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}