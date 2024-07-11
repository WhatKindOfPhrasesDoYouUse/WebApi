using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CartItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartItemController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetCartItems")]
        public ActionResult<IEnumerable<CartItem>> GetCartItems()
        {
            var cartItems = _context.CartItems.Include(s => s.Cart).Include(s => s.Shoe).ToList();
            return Ok(cartItems);
        }

        [HttpPost("AddCartItem")]
        public IActionResult AddCartItem(long cartId, long shoeId, int quantity)
        {
            CartItem cartItem = new CartItem();
            cartItem.CartId = cartId;
            cartItem.ShoeId = shoeId;
            cartItem.Quantity = quantity;

            

            var shoe = _context.Shoes.Find(shoeId);

            if (shoe.Quantity < quantity) return BadRequest("Обуви не достаточно");

            shoe.Quantity -= quantity;
            _context.CartItems.Add(cartItem);
            _context.SaveChanges();

            return Ok($"Обувь успешно добавлена в корзину, ее id: {shoeId} в количестве: {quantity}");
        }

        [HttpDelete("DeleteCartItem")]
        public IActionResult DeleteCartItem(long id)
        {
            var cartItem = _context.CartItems.Include(ci => ci.Shoe).FirstOrDefault(ci => ci.Id == id);

            if (cartItem == null) return BadRequest($"Нет записи в корзине с id: {id}");

            var shoe = cartItem.Shoe;
            shoe.Quantity += cartItem.Quantity;

            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return Ok($"Элемент корзины успешно удален. Возвращено {cartItem.Quantity} обуви.");
        }

        [HttpPut("UpdateCartItem")]
        public IActionResult UpdateCartItem(long cartItemId, long? cartId = null, long? shoeId = null, int? quantity = null)
        {
            var findCartItem = _context.CartItems.Include(ci => ci.Shoe).FirstOrDefault(ci => ci.Id == cartItemId);

            if (findCartItem == null) return BadRequest("Запись в корзине не найдена");

            if (quantity != null)
            {
                var shoe = findCartItem.Shoe;
                int oldQuantity = findCartItem.Quantity;
                int newQuantity = quantity.Value;
                int quantityDifference = newQuantity - oldQuantity;

                if (quantityDifference > 0 && shoe.Quantity < quantityDifference)
                {
                    return BadRequest("Обуви недостаточно на складе для увеличения количества");
                }

                shoe.Quantity -= quantityDifference;
                findCartItem.Quantity = newQuantity;
            }

            if (shoeId != null && findCartItem.ShoeId != shoeId.Value)
            {
                var oldShoe = findCartItem.Shoe;
                var newShoe = _context.Shoes.Find(shoeId.Value);

                if (newShoe == null) return BadRequest("Новая обувь не найдена");

                oldShoe.Quantity += findCartItem.Quantity;

                if (newShoe.Quantity < findCartItem.Quantity)
                {
                    return BadRequest("Новой обуви недостаточно на складе");
                }

                newShoe.Quantity -= findCartItem.Quantity;
                findCartItem.ShoeId = shoeId.Value;
                findCartItem.Shoe = newShoe;
            }

            if (cartId != null) findCartItem.CartId = cartId.Value;

            _context.CartItems.Update(findCartItem);
            _context.SaveChanges();

            return Ok($"Объект корзины с id: {cartItemId} успешно обновлен");
        }
    }
}
