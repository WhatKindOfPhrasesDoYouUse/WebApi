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
            var shoes = _context.Shoes.Include(s => s.Brand).Include(s => s.Category).ToList();
            return Ok(shoes);
        }

        [HttpPost("AddShoe")]
        public IActionResult AddShoe(string name, int price, int size, string color, int quantity, long brandId, long categoryId)
        {
            Shoe shoe = new Shoe();
            shoe.Name = name;
            shoe.Price = price;
            shoe.Size = size;
            shoe.Color = color;
            shoe.Quantity = quantity;
            shoe.BrandId = brandId;
            shoe.CategoryId = categoryId;

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

            return Ok($"Обувь c id: {id} успешно удалена");
        }

        [HttpPut("UpdateShoe")]
        public IActionResult UpdateShoe(long shoeId, string? name = null, int? price = null, int? size = null, string color = null, int? quantity = null, 
            long? brandId = null, long? categoryId = null)
        {
            var findShoe = _context.Shoes.Find(shoeId);

            if (findShoe == null)
            {
                return BadRequest("Обувь не найдена");
            }

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
            if (brandId != null)
            {
                findShoe.BrandId = brandId.Value;
            }
            if (categoryId != null)
            {
                findShoe.CategoryId = categoryId.Value;
            }

            _context.Shoes.Update(findShoe);
            _context.SaveChanges();

            return Ok($"Обувь с id: {shoeId} успешно обновлена");
        }
    }
}
