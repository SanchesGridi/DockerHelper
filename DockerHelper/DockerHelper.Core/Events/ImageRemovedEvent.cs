using Prism.Events;

namespace DockerHelper.Core.Events;

public class ImageRemovedEvent : PubSubEvent<List<(string Tag, string Value)>>
{
}
