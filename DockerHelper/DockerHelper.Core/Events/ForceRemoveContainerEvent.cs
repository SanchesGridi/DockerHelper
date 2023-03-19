using Prism.Events;

namespace DockerHelper.Core.Events;

public class ForceRemoveContainerEvent : PubSubEvent<string>
{
}
