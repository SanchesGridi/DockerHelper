using Docker.DotNet.Models;
using Prism.Events;

namespace DockerHelper.Core.Events;

public class ListContainersEvent : PubSubEvent<IList<ContainerListResponse>>
{
}
