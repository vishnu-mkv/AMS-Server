using AMS.Authorization;
using AMS.Interfaces;
using AMS.Providers;
using AMS.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AMS.Controllers;

[Controller]
[Route("api/[controller]")]
public class TopicsController : ControllerBase
{

    private readonly ITopicManager _topicManger;

    public TopicsController(ITopicManager topicManger)
    {
        _topicManger = topicManger;
    }

    [HttpGet]
    [Route("")]
    [HasPermission(PermissionEnum.ReadTopic)]
    public IActionResult GetTopics()
    {
        var topics = _topicManger.GetTopics();
        return Ok(topics);
    }

    [HttpGet]
    [Route("{topicId}")]
    public IActionResult GetTopic(string topicId)
    {
        var topic = _topicManger.GetTopic(topicId);
        if (topic == null)
        {
            return NotFound();
        }

        return Ok(topic);

    }

    [HttpPost]
    [Route("")]
    [HasPermission(PermissionEnum.AddTopic)]
    public IActionResult AddTopic([FromBody] AddTopicRequest topic)
    {
        if (topic.Name is null)
        {
            return BadRequest("Name is required");
        }


        return Ok(_topicManger.CreateTopic(topic));
    }

    [HttpPut]
    [Route("{topicId}")]
    [HasPermission(PermissionEnum.UpdateTopic)]
    public IActionResult UpdateTopic([FromBody] UpdateTopicRequest topic, [FromRoute] string topicId)
    {

        return Ok(_topicManger.UpdateTopic(topicId, topic));
    }

    [HttpDelete]
    [Route("{topicId}")]
    [HasPermission(PermissionEnum.DeleteTopic)]
    public IActionResult DeleteTopic(string topicId)
    {
        _topicManger.DeleteTopic(topicId);
        return Ok(true);
    }
}