using CompanyApi.DTOs.DocumentDtos;
using CompanyApi.DTOs.ResponseDtos;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Get([FromQuery] PaginationParameters paginationParams)
        {
            var result = await _documentService.GetAllDocumentsAsync(paginationParams);
            return Ok(result);
        }

        // GET api/<DocumentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var documentResult = await _documentService.GetDocumentByIdAsync(id);

            if (documentResult == null || documentResult.Data == null)
                return NotFound(await Result<DocumentDto>.FailureAsync("Document not found", 404));

            return Ok(await Result<DocumentDto>.SuccessAsync(documentResult.Data, "Document retrieved successfully", 200));
        }

        // POST api/<DocumentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DocumentDto document)
        {
            await _documentService.CreateDocumentAsync(document);
            return Ok(await Result<DocumentDto>.SuccessAsync(document, "Document created successfully", 201));
        }

        // PUT api/<DocumentController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] DocumentDto document)
        {
            await _documentService.UpdateDocumentAsync(id, document);
            return Ok(await Result<DocumentDto>.SuccessAsync(document, "Document updated successfully", 200));
        }

        // DELETE api/<DocumentController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _documentService.DeleteDocumentAsync(id);
            return Ok(await Result<string>.SuccessAsync(default, "Document deleted successfully", 200));
        }
    }
}
