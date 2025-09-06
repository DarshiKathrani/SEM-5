using FluentValidation;
using System.Diagnostics.Metrics;
using GroceryStoreManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GroceryStoreManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAPIController : ControllerBase
    {
        private readonly GroceryStoreManagementContext context;
        private readonly IValidator<Customer> validator;
        private readonly IConfiguration configuration;

        public CustomerAPIController(GroceryStoreManagementContext context, IValidator<Customer> validator, IConfiguration configuration)
        {
            this.context = context;
            this.validator = validator;
            this.configuration = configuration;
        }

        // 🔑 Generate Token with Role & Expiry from appsettings.json
        private string GenerateJwtToken(Customer customer)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, customer.CustomerName),
                new Claim(ClaimTypes.Role, customer.Role)
            };

            var expiryMinutes = Convert.ToDouble(jwtSettings["TokenExpiryMinutes"]);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ✅ LOGIN API
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginUser)
        {
            var user = await context.Customers
                .FirstOrDefaultAsync(u => u.CustomerName == loginUser.CustomerName && u.Password == loginUser.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new { user.CustomerId, user.Role }
            });
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok(new { message = "Welcome Authenticated User!" });
        }



        //[HttpGet("all-users")]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    return Ok(await _context.Users.ToListAsync());
        //}
        // 🛡️ Allow anonymous access (overrides controller-level authorize)
        [AllowAnonymous]
        [HttpGet("public-info")]
        public IActionResult PublicInfo()
        {
            return Ok(new { message = "This is public info, no login required." });
        }

        #region GetAllCustomers
        //[Authorize(Roles = "User")]
        [HttpGet("list/{id}")]
        public IActionResult GetCustomers(int id)
        {
            var current = context.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (current == null) return NotFound();

            if (current.Role == "Admin")
            {
               
                return Ok(context.Customers.Where(c => c.Role == "User").ToList());
            }
            else
            {
              
                return Ok(new List<Customer> { current });
            }
        }

        #endregion

        #region GetCustomerByID
        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = context.Customers.Find(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        #endregion

        #region DeleteCustomerById
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomerById(int id)
        {

            var customer = context.Customers.Find(id);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            try
            {
                context.Customers.Remove(customer);
                context.SaveChanges();
                return NoContent(); // 204 success
            }
            catch (DbUpdateException ex)
            {
                string error = "";

                if (ex.InnerException != null)
                    error = ex.InnerException.Message;
                else
                    error = ex.Message;

                // TEMP: Log it to console to debug
                Console.WriteLine("DELETE ERROR: " + error);

                return BadRequest("Delete failed: " + error);
            }
        }
        #endregion

        #region InsertCustomer
        [HttpPost]
        public async Task<IActionResult> InsertCustomer([FromBody] Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Password))
            {
                // Option 1: Do nothing, keep null (not used in management)
                // Option 2: Set default like "AlreadyRegistered" just for database non-null
                customer.Password = "AlreadyRegistered";
            }

            if (string.IsNullOrEmpty(customer.Role))
            {
                customer.Role = "User";
            }

            var validationResult = await validator.ValidateAsync(customer);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }
        #endregion

        #region UpdateCustomer
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId) 
            {
                return BadRequest();
            }
            var existingCustomer = context.Customers.Find(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }
            existingCustomer.CustomerName = customer.CustomerName;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Email = customer.Email;        
            existingCustomer.Address = customer.Address;
            existingCustomer.Orders = customer.Orders;
            context.Customers.Update(existingCustomer);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region SearchCustomer
        // Get customers by filters (CustomerId)
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Customer>>> Filter([FromQuery] int? customerId)
        {
            var query = context.Customers.AsQueryable();
            if (customerId.HasValue)
                query = query.Where(c => c.CustomerId == customerId);

            return await query.ToListAsync();
        }

        #endregion

        #region CustomerDropdown
        // Get all customers (for dropdown)
        [HttpGet("dropdown/customers")]
        public async Task<ActionResult<IEnumerable<object>>> GetCustomers()
        {
            return await context.Customers
                .Select(c => new { c.CustomerId, c.CustomerName })
                .ToListAsync();
        }
        #endregion

        #region TopCustomers
        // Get top 3 customers
        [HttpGet("Top3")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetTop3()
        {
            return await context.Customers
               
                .Take(3)
                .ToListAsync();
        }
        #endregion

        #region InsertCustomerByAdmin
        [HttpPost("add-by-admin")]
        public async Task<IActionResult> AddCustomerByAdmin([FromBody] GroceryStoreManagementSystem.Models.CustomerAdminDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid customer data.");

            // Auto-generate temporary password
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);

            var customer = new Customer
            {
                CustomerName = dto.CustomerName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Password = tempPassword,   // required for DB
                Role = "User"              // default role for new customers
            };

            // ✅ validate using your existing FluentValidation<Customer>
            var validationResult = await validator.ValidateAsync(customer);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Customer created successfully",
                CustomerId = customer.CustomerId,
                TemporaryPassword = tempPassword
            });
        }
        #endregion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(string? search, string? address)
        {
            var customers = this.context.Customers.AsQueryable();

            // Search by name
            if (!string.IsNullOrEmpty(search))
            {
                customers = customers.Where(c => c.CustomerName.Contains(search));
            }

            // Filter by address
            if (!string.IsNullOrEmpty(address))
            {
                customers = customers.Where(c => c.Address.Contains(address));
            }

            return await customers.ToListAsync();
        }

        [HttpGet("paged")]
        public IActionResult GetPagedCustomers(int page = 1, int pageSize = 5)
        {
            var query = this.context.Customers.AsQueryable();

            var totalCount = query.Count();
            var customers = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                data = customers,
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }




    }
}