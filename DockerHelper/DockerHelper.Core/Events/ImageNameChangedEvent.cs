using Prism.Events;

namespace DockerHelper.Core.Events;

public class ImageNameChangedEvent : PubSubEvent<string>
{
}
