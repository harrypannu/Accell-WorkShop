using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WiproPOC.DAL;
using WiproPOC.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WiproPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerDbContext _context;

        public CustomersController(CustomerDbContext context)
        {
            _context = context;
        }

        //[Authorize(Policy = "RequireAdminRole")]  //To enable role it required premium subscription
        [HttpGet]
        public async Task<IEnumerable<Customer>> Get(int pageNumber = 1,
                int pageSize = 10,
                string sortBy = "Id",
                bool isDescending = false)
        {
            var query = _context.Customers.AsQueryable();

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "firstname" => isDescending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),
                "lastname" => isDescending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),
                "email" => isDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                "city" => isDescending ? query.OrderByDescending(c => c.City) : query.OrderBy(c => c.City),
                "age" => isDescending ? query.OrderByDescending(c => c.Age) : query.OrderBy(c => c.Age),
                _ => isDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };

            var customers = await query
                              .Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();

            return customers;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<Customer>> GetFilteredCustomers(
           [FromRoute] int? id,
            string city,
            int? age_gt,
            int? age_lt,
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "Id",
            bool isDescending = false)
        {
            var query = _context.Customers.AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(c => c.Id == id.Value);
            }
            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(c => c.City == city);
            }
            if (age_gt.HasValue)
            {
                query = query.Where(c => c.Age > age_gt.Value);
            }
            if (age_lt.HasValue)
            {
                query = query.Where(c => c.Age < age_lt.Value);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "firstname" => isDescending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),
                "lastname" => isDescending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),
                "email" => isDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                "city" => isDescending ? query.OrderByDescending(c => c.City) : query.OrderBy(c => c.City),
                "age" => isDescending ? query.OrderByDescending(c => c.Age) : query.OrderBy(c => c.Age),
                _ => isDescending ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };

            var customers = await query.ToListAsync();

            return Ok(customers);

        }
    }
}
