﻿using Prism.Events;

namespace DockerHelper.Core.Events;

public class ContainerPruneEvent : PubSubEvent<IList<string>>
{
}
