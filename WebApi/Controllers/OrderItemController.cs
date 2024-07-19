using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OrderItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetOrderItems")]
        public ActionResult<IEnumerable<OrderItem>> GetOrderItems()
        {
            var orderItems = _context.OrderItems
                .Include(s => s.Order)
                    .ThenInclude(o => o.Client)
                    .ThenInclude(o => o.Role)
                .Include(s => s.Order)
                    .ThenInclude(o => o.PickupPoint) 
                .Include(s => s.Shoe)
                    .ThenInclude(sh => sh.Brand) 
                .Include(s => s.Shoe)
                    .ThenInclude(sh => sh.Category)
                .ToList();
            
            if (!orderItems.Any()) return BadRequest("В списке нет объектов заказа");
            
            return Ok(orderItems);
        }

        [HttpPost("AddOrderItem")]
        public IActionResult AddOrderItem(long orderId, long shoeId, int quantity, bool isPickedUp)
        {
            var shoe = _context.Shoes.Find(shoeId);
            if (shoe == null) return BadRequest("Обувь не найдена");

            OrderItem orderItem = new OrderItem
            {
                OrderId = orderId,
                ShoeId = shoeId,
                Quantity = quantity,
                Price = shoe.Price * quantity,
                IsPickedUp = isPickedUp
            };

            var existingOrderItem = _context.OrderItems.FirstOrDefault(s => s.OrderId == orderItem.OrderId
                                                                      && s.ShoeId == orderItem.ShoeId
                                                                      && s.Quantity == orderItem.Quantity
                                                                      && s.Price == orderItem.Price
                                                                      && s.IsPickedUp == orderItem.IsPickedUp);

            if (existingOrderItem != null) return BadRequest($"Заказ уже существует");

            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();

            return Ok($"Заказ успешно добавлен, его id: {orderItem.Id}");
        }

        [HttpDelete("DeleteOrderItem")]
        public IActionResult DeleteOrderItem(long id)
        {
            var orderItem = _context.OrderItems.Find(id);

            if (orderItem == null) return BadRequest($"Нет заказа с id: {id}");

            if (orderItem.IsPickedUp) return BadRequest($"Не возможно удалить забранный товар");

            _context.OrderItems.Remove(orderItem);
            _context.SaveChanges();

            return Ok($"Заказ c id: {id} успешно удален");
        }

        [HttpPut("UpdateOrderItem")]
        public IActionResult UpdateOrderItem(long orderItemId, long? orderId = null, long? shoeId = null, int? quantity = null, bool? isPickedUp = null)
        {
            var findOrderItem = _context.OrderItems.Find(orderItemId);

            if (findOrderItem == null) return BadRequest($"Заказ с id: {orderItemId} не найден");

            if (orderId != null) findOrderItem.OrderId = orderId.Value;
            
            if (shoeId != null) findOrderItem.ShoeId = shoeId.Value;
            
            if (quantity != null) findOrderItem.Quantity = quantity.Value;
            
            if (shoeId != null || quantity != null)
            {
                var shoe = _context.Shoes.Find(findOrderItem.ShoeId);
                if (shoe != null)
                {
                    findOrderItem.Price = shoe.Price * findOrderItem.Quantity;
                }
            }
            if (isPickedUp != null)
            {
                findOrderItem.IsPickedUp = isPickedUp.Value;
            }

            _context.OrderItems.Update(findOrderItem);
            _context.SaveChanges();

            return Ok($"Заказ с id: {orderItemId} успешно обновлен");
        }
    }
}
