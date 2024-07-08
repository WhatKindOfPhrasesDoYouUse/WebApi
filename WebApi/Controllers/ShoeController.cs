using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ShoeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoeController(ApplicationDbContext context) => _context = context;

        [HttpGet("GetShoes")]
        public ActionResult<IEnumerable<Shoe>> GetShoes()
        {
            var shoes = _context.Shoes.ToList();
            return Ok(shoes);
        }

        [HttpPost("AddShoe")]
        public IActionResult AddShoe(string name, int price, int size, string color, int quantity)
        {
            Shoe shoe = new Shoe();
            shoe.Name = name;
            shoe.Price = price;
            shoe.Size = size;
            shoe.Color = color;
            shoe.Quantity = quantity;

            var existingShoe = _context.Shoes.FirstOrDefault(s => s.Name == shoe.Name 
                                                            && s.Price == shoe.Price
                                                            && s.Size == shoe.Size
                                                            && s.Color == shoe.Color
                                                            && s.Quantity == shoe.Quantity);

            if (existingShoe != null) 
            {
                return BadRequest("Такая обувь уже существует");
            }

            _context.Shoes.Add(shoe);
            _context.SaveChanges();

            return Ok("Обувь успешно добавлена");
        }

        [HttpDelete("DeleteShoe")]
        public IActionResult DeleteShoe(long id)
        {
            var shoe = _context.Shoes.Find(id);

            if (shoe == null)
            {
                return BadRequest("Нет обуви с таким id");
            }

            _context.Shoes.Remove(shoe);
            _context.SaveChanges();

            return Ok("Обувь успешно удалена");
        }

        [HttpPut("UpdateShoe")]
        public IActionResult UpdateShoe(long shoeId, string name = null, int? price = null, int? size = null, string color = null, int? quantity = null)
        {
            var findShoe = _context.Shoes.Find(shoeId);

            if (findShoe == null)
            {
                return BadRequest("Обувь не найдена");
            }

            // Обновляем только переданные поля
            if (name != null)
            {
                findShoe.Name = name;
            }
            if (price != null)
            {
                findShoe.Price = price.Value;
            }
            if (size != null)
            {
                findShoe.Size = size.Value;
            }
            if (color != null)
            {
                findShoe.Color = color;
            }
            if (quantity != null)
            {
                findShoe.Quantity = quantity.Value;
            }

            _context.Shoes.Update(findShoe);
            _context.SaveChanges();

            return Ok("Обувь успешно обновлена");
        }

    }
}
