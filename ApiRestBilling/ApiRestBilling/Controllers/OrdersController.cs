using ApiRestBilling.Data;
using ApiRestBilling.Models;
using ApiRestBilling.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiRestBilling.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IPurchaseOrdersService _purchaseOrdersService;

        public OrdersController(ApplicationDbContext context, IPurchaseOrdersService purchaseOrdersService)
        {
            _context = context;
            _purchaseOrdersService = purchaseOrdersService;
        }
        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            return await _context.Orders.ToListAsync();
        }

        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var Order = await _context.Orders.FindAsync(id);

            if (Order is null)
            {
                return NotFound();
            }

            return Order;
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order Order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }

            foreach (var detalle in Order.OrderItems)
            {
                detalle.UnitPrice = await _purchaseOrdersService.CheckUnitPrice(detalle);

                detalle.Subtotal = await _purchaseOrdersService.CalculateSubtotalOrderItem(detalle);
            }

            Order.TotalAmount = _purchaseOrdersService.CalcularTotalOrderItems((List<OrderItem>)Order.OrderItems);

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = Order.Id }, Order);
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order Order)
        {

            if (id != Order.Id)
            {
                return BadRequest();
            }
            _context.Entry(Order).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            return NoContent();
        }

        private bool OrderExists(int id)
        {
            throw new NotImplementedException();
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_context.Orders is null)
            {
                return NotFound();
            }

            var Order = await _context.Orders.FirstOrDefaultAsync(s => s.Id == id);
            if (Order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(Order);
            await _context.SaveChangesAsync();
            return NoContent();

        }
    }
}
