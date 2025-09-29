using CompanyApi.DTOs;
using CompanyApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CompanyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        // GET: api/<DocumentController>
        [HttpGet]
        public async Task<IEnumerable<DocumentDto>> Get()
        {
            return await _documentService.GetAllDocumentsAsync();
        }

        // GET api/<DocumentController>/5
        [HttpGet("{id}")]
        public async Task<DocumentDto?> Get(int id)
        {
            return await _documentService.GetDocumentByIdAsync(id);
        }

        // POST api/<DocumentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DocumentDto document)
        {
            await _documentService.CreateDocumentAsync(document);
            return Ok("Document Created Successfully");
        }

        // PUT api/<DocumentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromBody] DocumentDto document)
        {
            await _documentService.UpdateDocumentAsync(id,document);
            return Ok("Document Updated Successfully");
        }

        // DELETE api/<DocumentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return Ok("Document Deleted Successfully");
        }
    }
}
