using CompanyApi.DTOs;
using CompanyApi.Services.Interfaces;
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
        public async Task<IActionResult> Get(PaginationParameters paginationParameters)
        {
            var products = await _productService.GetAllProductsAsync(paginationParameters);
            return Ok(products);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult?> Get(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }

        // POST api/<ProductController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductDto branch)
        {
            await _productService.CreateProductAsync(branch);
            return Ok(await Result<ProductDto>.SuccessAsync("Product Created Successfully", 200));
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductDto branch)
        {
            await _productService.UpdateProductAsync(id, branch);
            return Ok(await Result<ProductDto>.SuccessAsync("Product Updated Successfully", 200));
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok(await Result<ProductDto>.SuccessAsync("Product Deleted Successfully", 200));
        }
    }
}
