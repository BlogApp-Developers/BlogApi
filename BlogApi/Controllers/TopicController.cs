using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Base;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers;
public class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;

    public TopicController(ITopicService topicService)
    {
        this._topicService = topicService;
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<IEnumerable<Topic>>> GetAllTopics()
    {
        try
        {
            var topics = await _topicService.GetAllTopicsAsync();

            if (topics == null || !topics.Any())
            {
                return NotFound("Blogs not found");
            }

            return Ok(topics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
