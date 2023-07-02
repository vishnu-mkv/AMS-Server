using AMS.Data;
using AMS.Interfaces;
using AMS.Models;
using AMS.Requests;

namespace AMS.Managers;

public class TopicManager : ITopicManager
{

    private readonly ApplicationDbContext context;
    private readonly IAuthManager authManager;

    public TopicManager(ApplicationDbContext context, IAuthManager authManager)
    {
        this.context = context;
        this.authManager = authManager;
    }

    public bool CheckIfTopicExists(string id, string organizationId)
    {
        return context.Topics.Any(t => t.Id == id && t.OrganizationId == organizationId);
    }

    public Topic CreateTopic(AddTopicRequest request)
    {
        var Topic = new Topic
        {
            Name = request.Name
        };

        if (request.Color != null)
        {
            Topic.Color = request.Color;
        }

        Topic.OrganizationId = authManager.GetUserOrganizationId();

        context.Topics.Add(Topic);
        context.SaveChanges();

        return Topic;
    }

    public bool DeleteTopic(string id)
    {
        var Topic = GetTopic(id);

        if (Topic == null)
        {
            throw new Exception("Topic not found");
        }

        context.Topics.Remove(Topic);
        context.SaveChanges();

        return true;
    }

    public Topic? GetTopic(string id)
    {
        return context.Topics.FirstOrDefault(t => t.Id == id && t.OrganizationId == authManager.GetUserOrganizationId());
    }

    public List<Topic> GetTopics()
    {
        return context.Topics.Where(t => t.OrganizationId == authManager.GetUserOrganizationId()).ToList();
    }

    public Topic UpdateTopic(string id, UpdateTopicRequest request)
    {
        var Topic = GetTopic(id);

        if (Topic == null)
        {
            throw new Exception("Topic not found");
        }

        Topic.Name = request.Name ?? Topic.Name;

        Topic.Color = request.Color ?? Topic.Color;
        context.SaveChanges();

        return Topic;
    }


}
