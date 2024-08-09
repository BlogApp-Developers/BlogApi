using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;
    private readonly BlogDbContext _context;

    public TopicController(ITopicService topicService, BlogDbContext context)
    {
        this._topicService = topicService;
        this._context = context;
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<IEnumerable<Topic>>> GetAllTopics()
    {
        try
        {
            var topics = await _topicService.GetAllTopicsAsync();

            if (topics == null || !topics.Any())
            {
                return NotFound("Topics not found");
            }

            return Ok(topics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpGet("GetUserTopics/{userId}")]
    public async Task<ActionResult<IEnumerable<Topic>>> GetUserTopics(Guid userId)
    {
        try
        {
            
            var userTopics = await _context.UserTopics
                .Where(ut => ut.UserId == userId)
                .ToListAsync();

            if (userTopics == null || !userTopics.Any())
            {
                return NotFound("No topics found for this user.");
            }

        
            var topicIds = userTopics.Select(ut => ut.TopicId).ToList();

            
            var topics = await _context.Topics
                .Where(t => topicIds.Contains(t.Id))
                .ToListAsync();

            if (topics == null || !topics.Any())
            {
                return NotFound("Topics not found.");
            }

            return Ok(topics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }




    [HttpPost("AssignTopicsToUser/{userId}")]
    public async Task<IActionResult> AssignTopicsToUser(Guid userId, List<int> topicIds)
    {
        if (topicIds.Count < 3)
        {
            return BadRequest("You must select at least 3 topics.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var existingTopics = await _context.Topics.Where(t => topicIds.Contains(t.Id)).ToListAsync();

        var userTopics = existingTopics.Select(t => new UserTopic
        {
            UserId = userId,
            TopicId = t.Id
        }).ToList();

        _context.UserTopics.AddRange(userTopics);
        await _context.SaveChangesAsync();

        return Ok();
    }

}
