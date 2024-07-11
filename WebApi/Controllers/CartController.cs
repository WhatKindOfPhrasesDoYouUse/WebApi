using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context) { _context = context; }

        [HttpGet("GetCarts")]
        public ActionResult<IEnumerable<Cart>> GetCarts()
        {
            var carts = _context.Carts.Include(s => s.Client).ToList();
            return Ok(carts);
        }

        [HttpPost("AddCart")]
        public ActionResult AddCart(long clientId)
        {
            Cart cart = new Cart();
            cart.ClientId = clientId;
            
            var existingCart = _context.Carts.FirstOrDefault(s => s.ClientId == clientId);

            if (existingCart != null) return BadRequest($"Корзина у пользователя {_context.Clients.Find(clientId)} уже существует");

            _context.Carts.Add(cart);
            _context.SaveChanges();

            return Ok($"Корзина успешно добавлена, ее id: {cart.Id}");
        }


        [HttpDelete("DeleteCart")]
        public IActionResult DeleteCart(long id)
        {
            var cart = _context.Carts.Find(id);

            if (cart == null) return BadRequest("Нет корзины с таким id");

            _context.Carts.Remove(cart);
            _context.SaveChanges();

            return Ok($"Корзина с id: {id} успешно удалена");
        }

        [HttpPut("UpdateCart")]
        public IActionResult UpdateCart(long cartId, long? clientId = null)
        {
            var findCart = _context.Carts.Find(cartId);

            if (findCart == null) return BadRequest("Корзина не найдена");

            if (clientId != null) findCart.ClientId = clientId.Value;

            _context.Carts.Update(findCart);
            _context.SaveChanges();

            return Ok($"Корзина с id: {cartId} успешно Обновлена");
        }

    }
}
