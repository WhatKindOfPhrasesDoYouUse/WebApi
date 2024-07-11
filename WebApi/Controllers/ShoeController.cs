using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (!shoes.Any()) return BadRequest("В списке нет обуви");
            return Ok(shoes);
        }

        [HttpPost("AddShoe")]
        public IActionResult AddShoe(string name, int price, int size, string color, int quantity, long brandId, long categoryId)
        {
            var errors = new List<string>();

            var brandExists = _context.Brands.Any(b => b.Id == brandId);
            if (!brandExists) errors.Add($"Нет бренда с id: {brandId}");

            var categoryExists = _context.Categories.Any(c => c.Id == categoryId);
            if (!categoryExists) errors.Add($"Нет категории с id: {categoryId}");

            if (errors.Any()) return BadRequest(string.Join(" и ", errors));

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

            if (existingShoe != null) return BadRequest($"Обувь с иминем {name} уже существует");
            
            _context.Shoes.Add(shoe);
            _context.SaveChanges();

            return Ok($"Обувь успешно создана, ее id: {shoe.Id}");
        }

        [HttpDelete("DeleteShoe")]
        public IActionResult DeleteShoe(long id)
        {
            var shoe = _context.Shoes.Find(id);

            if (shoe == null) return BadRequest($"Нет обуви с id: {id}");

            try
            {
                _context.Shoes.Remove(shoe);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Невозможно удалить обувь с id: {id}, так как она связана с другими объектами");
            }

            return Ok($"Обувь с id: {id} успешно удалена");
        }

        [HttpPut("UpdateShoe")]
        public IActionResult UpdateShoe(long shoeId, string? name = null, int? price = null, int? size = null, string color =   null, int? quantity = null, 
            long? brandId = null, long? categoryId = null)
        {
            var findShoe = _context.Shoes.Find(shoeId);

            if (findShoe == null) return BadRequest($"Обувь c id: {shoeId} не найдена");
            
            if (name != null) findShoe.Name = name;
            if (price != null) findShoe.Price = price.Value;
            if (size != null) findShoe.Size = size.Value;
            if (color != null) findShoe.Color = color;
            if (quantity != null) findShoe.Quantity = quantity.Value;
            if (brandId != null)
            {
                var brandExists = _context.Brands.Any(b => b.Id == brandId);
                if (!brandExists) return BadRequest($"Нет бренда с id: {brandId}");
                else findShoe.BrandId = brandId.Value;
            }
            if (categoryId != null)
            {
                var categoryExists = _context.Categories.Any(c => c.Id == categoryId);
                if (!categoryExists) return BadRequest($"Нет категории с id: {categoryId}");
                else findShoe.CategoryId = categoryId.Value;

            }

            _context.Shoes.Update(findShoe);
            _context.SaveChanges();

            return Ok($"Обувь с id: {shoeId} успешно обновлена");
        }
    }
}
