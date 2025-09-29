using CompanyApi.DTOs;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Get()
        {
            var result = await _branchService.GetAllBranchesAsync();
            return Ok(result);
        }

        // GET api/<BranchController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var branchResult = await _branchService.GetBranchByIdAsync(id);

            if (branchResult == null || branchResult.Data == null)
                return NotFound(await Result<BranchDto>.FailureAsync("Branch not found", 404));

            return Ok(branchResult);
        }

        // POST api/<BranchController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BranchDto branch)
        {
            await _branchService.CreateBranchAsync(branch);
            return Ok(await Result<BranchDto>.SuccessAsync(branch, "Branch created successfully", 201));
        }

        // PUT api/<BranchController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BranchDto branch)
        {
            await _branchService.UpdateBranchAsync(id, branch);
            return Ok(await Result<BranchDto>.SuccessAsync(branch, "Branch updated successfully", 200));
        }

        // DELETE api/<BranchController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _branchService.DeleteBranchAsync(id);
            return Ok(await Result<string>.SuccessAsync(default, "Branch deleted successfully", 200));
        }
    }
}
