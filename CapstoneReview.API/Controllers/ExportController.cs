using CapstoneReview.Service.Exceptions;
using CapstoneReview.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneReview.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly ILecturerSchedulePdfService _lecturerSchedulePdfService;

    public ExportController(ILecturerSchedulePdfService lecturerSchedulePdfService)
    {
        _lecturerSchedulePdfService = lecturerSchedulePdfService;
    }

    [HttpGet("lecturer-schedule-pdf")]
    public async Task<IActionResult> ExportLecturerSchedulePdf([FromQuery] int lecturerId, [FromQuery] int reviewRound)
    {
        try
        {
            var pdfBytes = await _lecturerSchedulePdfService.ExportLecturerSchedulePdfAsync(lecturerId, reviewRound);

            var fileName = $"lecturer_{lecturerId}_round_{reviewRound}_schedule.pdf";
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}