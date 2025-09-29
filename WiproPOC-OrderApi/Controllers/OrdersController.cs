using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WiproPOC_OrderApi.DAL;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WiproPOC_OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;

        public OrdersController(OrderDbContext context)
        {
            _context = context;
        }

        // GET: api/orders/customer/{customerId}
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomer(
            int customerId,
            string sortBy = "OrderDate",
            bool isDescending = false)
        {
            var query = _context.Orders.Where(o => o.CustomerId == customerId);

            // Apply sorting based on sortBy and isDescending
            query = sortBy.ToLower() switch
            {
                "orderdate" => isDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
                _ => isDescending ? query.OrderByDescending(o => o.CustomerId) : query.OrderBy(o => o.CustomerId),
            };

            var orders = await query.ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/sum/customer/{customerId}?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd
        [HttpGet("sum/customer/{customerId}")]
        public async Task<IActionResult> GetSumOfOrders(int customerId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var totalAmount = await _context.Orders
                .Where(o => o.CustomerId == customerId && o.OrderDate >= startDate && o.OrderDate <= endDate)
                .SumAsync(o => o.Amount);

            return Ok(new
            {
                CustomerId = customerId,
                StartDate = startDate,
                EndDate = endDate,
                TotalAmount = totalAmount
            });
        }
    }
}
