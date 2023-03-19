using DockerHelper.Core.Records;
using Prism.Events;

namespace DockerHelper.Core.Events;

public class RunContainerEvent : PubSubEvent<ContainerState>
{
}
