using Prism.Events;

namespace DockerHelper.Core.Events;

public class ExceptionEvent : PubSubEvent<Exception>
{
}
