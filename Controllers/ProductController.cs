using CompanyApi.DTOs;
using CompanyApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CompanyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<IEnumerable<ProductDto>> Get()
        {
            return await _productService.GetAllProductsAsync();
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ProductDto?> Get(int id)
        {
            return await _productService.GetProductByIdAsync(id);
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDto branch)
        {
            await _productService.CreateProductAsync(branch);
            return Ok("Product Created Successfully");
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromBody] ProductDto branch)
        {
            await _productService.UpdateProductAsync(id, branch);
            return Ok("Product Updated Successfully");
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok("Product Deleted Successfully");
        }
    }
}
