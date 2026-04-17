using CapstoneReview.Service.DTOs;
using CapstoneReview.Service.Exceptions;
using CapstoneReview.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneReview.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;

    public TopicController(ITopicService topicService)
    {
        _topicService = topicService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _topicService.GetAllTopicsAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _topicService.GetTopicByIdAsync(id);
        if (result == null)
            return NotFound(new { error = "Topic not found." });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTopicRequest request)
    {
        try
        {
            return Ok(await _topicService.CreateTopicAsync(request));
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTopicRequest request)
    {
        try
        {
            return Ok(await _topicService.UpdateTopicAsync(id, request));
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _topicService.DeleteTopicAsync(id);
            return Ok(new { message = "Topic deleted successfully." });
        }
        catch (BusinessRuleException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}