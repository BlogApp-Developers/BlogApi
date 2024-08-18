using BlogApi.Data;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace BlogApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TopicController : ControllerBase
{
    private readonly ITopicService _topicService;
    private readonly BlogDbContext _context;
    private readonly BlogApi.TokenValidation.TokenValidation tokenValidation;

    public TopicController(ITopicService topicService, BlogDbContext context, BlogApi.TokenValidation.TokenValidation tokenValidation)
    {
        this.tokenValidation = tokenValidation;
        this._topicService = topicService;
        this._context = context;
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<IEnumerable<Topic>>> GetAllTopics()
    {
        try
        {
            try
            {
                base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

                var tokenNew = headerValues.FirstOrDefault().Substring(7);
                this.tokenValidation.ValidateToken(tokenNew);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

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
    public async Task<ActionResult<IEnumerable<TopicDto>>> GetUserTopics(Guid userId)
    {
        try
        {
            try
            {
                base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

                var tokenNew = headerValues.FirstOrDefault().Substring(7);
                this.tokenValidation.ValidateToken(tokenNew);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }


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

            var topicDtos = topics.Select(t => new TopicDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();

            return Ok(topicDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("AssignTopicsToUser/{userId}")]
    public async Task<IActionResult> AssignTopicsToUser(Guid userId, List<int> topicIds)
    {
        try
        {
            base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

            var tokenNew = headerValues.FirstOrDefault().Substring(7);
            this.tokenValidation.ValidateToken(tokenNew);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
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



    [HttpGet("RecommendedTopics/{userId}")]
    public async Task<ActionResult<IEnumerable<Topic>>> GetRecommendedTopics(Guid userId)
    {
        try
        {
            try
            {
                base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

                var tokenNew = headerValues.FirstOrDefault().Substring(7);
                this.tokenValidation.ValidateToken(tokenNew);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
            var userTopics = await _context.UserTopics
                .Where(ut => ut.UserId == userId)
                .Select(ut => ut.TopicId)
                .ToListAsync();

            var recommendedTopics = await _context.Topics
                .Where(t => !userTopics.Contains(t.Id))
                .Take(10)
                .ToListAsync();

            return Ok(recommendedTopics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPost("AddTopicToUser/{userId}/{topicId}")]
    public async Task<IActionResult> AddTopicToUser(Guid userId, int topicId)
    {
        try
        {
            try
            {
                base.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerValues);

                var tokenNew = headerValues.FirstOrDefault().Substring(7);
                this.tokenValidation.ValidateToken(tokenNew);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
            {
                return NotFound("Topic not found");
            }

            var userTopicExists = await _context.UserTopics
                .AnyAsync(ut => ut.UserId == userId && ut.TopicId == topicId);

            if (userTopicExists)
            {
                return BadRequest("Topic is already assigned to the user");
            }

            var userTopic = new UserTopic
            {
                UserId = userId,
                TopicId = topicId
            };

            _context.UserTopics.Add(userTopic);
            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


}
