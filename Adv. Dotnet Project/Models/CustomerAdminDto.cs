using System.ComponentModel.DataAnnotations;

namespace GroceryStoreManagementSystem.Models
{
    public class CustomerAdminDto
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
