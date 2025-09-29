using CompanyApi.DTOs;
using CompanyApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CompanyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        // GET: api/<BranchController>
        [HttpGet]
        public async Task<IEnumerable<BranchDto>> Get()
        {
            return await _branchService.GetAllBranchesAsync();
        }

        // GET api/<BranchController>/5
        [HttpGet("{id}")]
        public async Task<BranchDto?> Get(int id)
        {
            return await _branchService.GetBranchByIdAsync(id);
        }

        // POST api/<BranchController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BranchDto branch)
        {
            await _branchService.CreateBranchAsync(branch);
            return Ok("Branch Created Successfully");
        }

        // PUT api/<BranchController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromBody] BranchDto branch)
        {
            await _branchService.UpdateBranchAsync(id, branch);
            return Ok("Branch Updated Successfully");
        }

        // DELETE api/<BranchController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _branchService.DeleteBranchAsync(id);
            return Ok("Branch Deleted Successfully");
        }
    }
}
