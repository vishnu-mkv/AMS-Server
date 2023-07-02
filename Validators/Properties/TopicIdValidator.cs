using AMS.Interfaces;
using FluentValidation;
using FluentValidation.Validators;

namespace AMS.Validators;

public class TopicIdValidator<T> : PropertyValidator<T, string>
{

    private readonly IAuthManager authManager;
    private readonly ITopicManager topicManager;

    public TopicIdValidator(IAuthManager authManager, ITopicManager topicManager)
    {
        this.authManager = authManager;
        this.topicManager = topicManager;
    }

    public override string Name => "ValidTopicValidator";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (value == null)
        {
            return true;
        }

        return topicManager.CheckIfTopicExists(value, authManager.GetUserOrganizationId());
    }
}