using CompanyApi.DTOs.TemporaryDocumentDto;
using CompanyApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TemporaryDocumentsController : ControllerBase
    {
        private readonly ITemporaryDocumentService _temporaryDocument;

        public TemporaryDocumentsController(ITemporaryDocumentService temporaryDocument)
        {
            _temporaryDocument = temporaryDocument;
        }

        [HttpPost("createTempDoc")]
        public async Task<IActionResult> CreateTempDocuments([FromBody] ReceiptGenerationDto request)
        {

            var result = await _temporaryDocument.GenerateReceiptsAsync(request);
            return Ok(result);

        }

        [HttpPost("ApproveTempDoc")]
        public async Task<IActionResult> ApproveTempDocument([FromBody] ApproveTempDocumentDto request)
        {
            var result = await _temporaryDocument.ApproveReceiptsAsync(request);
            return Ok(result);
        }

    }
}
