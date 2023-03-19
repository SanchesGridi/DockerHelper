using Prism.Events;

namespace DockerHelper.Core.Events;

public class ContainerMonitorEvent : PubSubEvent<(string Action, string Type, string Id, string Container, string Image)>
{
}
