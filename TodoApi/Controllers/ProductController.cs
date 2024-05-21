using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore; 
using TodoApi.DataAccess;
using TodoApi.Models;
namespace TodoApi.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController:ControllerBase{
        private readonly AppicationContext _context;
        public ProductController(AppicationContext context){
            _context =context;
        }
        [HttpGet("Products")]
        [Authorize(Roles ="admin,user")]
        public IActionResult Get(){
            var products =_context.Products.ToList();
            return Ok(products);
        }
        [HttpGet("Product/{id}")]
        [Authorize(Roles ="admin,user")]
        public IActionResult GetProduct([FromRoute] int id){
            var product = _context.Products.FirstOrDefault(x=>x.Id == id);
            if(product!=null){
                return Ok(product);
            }
            return NotFound();
            
        }
        [HttpPost("Products")]
        [Authorize(Roles ="admin")]
        public IActionResult Save([FromBody] Product product){
            _context.Products.Add(product);
            _context.SaveChanges(); //for save state
            return Ok(product);
        }
        [HttpPut("Products")]
        [Authorize(Roles ="admin")]
        public IActionResult UPdate([FromBody] Product product){
            var result =_context.Products.AsNoTracking().FirstOrDefault(x => x.Id == product.Id);
            if(result!=null){
                 _context.Products.Update(product);
                _context.SaveChanges();
                return Ok();
            }
            return NotFound();
           
        }

        [HttpDelete("Products")]
        [Authorize(Roles ="admin")]
        public IActionResult delete([FromQuery] int id){
            var deleteProduct =_context.Products.AsNoTracking().FirstOrDefault(x=>x.Id ==id);
            if(deleteProduct==null){
                return NotFound();
            }
            _context.Products.Entry(deleteProduct).State =EntityState.Deleted ; //method 1
            // _context.Products.Remove(deleteProduct); //method2
            _context.SaveChanges();
            return Ok();
        }
    }
}