using AMS.Models;
using AMS.Requests;

namespace AMS.Interfaces;

public interface ITopicManager
{
    Topic CreateTopic(AddTopicRequest request);
    Topic UpdateTopic(string id, UpdateTopicRequest request);
    Topic GetTopic(string id);

    bool DeleteTopic(string id);

    List<Topic> GetTopics();
}