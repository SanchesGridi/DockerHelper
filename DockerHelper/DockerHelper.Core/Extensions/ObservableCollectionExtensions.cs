using System.Collections.ObjectModel;

namespace DockerHelper.Core.Extensions;

public static class ObservableCollectionExtensions
{
    public static void AddRange<TAny>(this ObservableCollection<TAny> @this, IEnumerable<TAny> range)
    {
        if (@this == null)
        {
            throw new ArgumentNullException("@this");
        }
        if (range == null)
        {
            throw new ArgumentNullException(nameof(range));
        }
        foreach (var item in range)
        {
            @this.Add(item);
        }
    }
}
