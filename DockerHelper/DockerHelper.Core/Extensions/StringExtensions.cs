namespace DockerHelper.Core.Extensions;

public static class StringExtensions
{
    public static bool IsEmpty(this string @this)
    {
        return string.IsNullOrWhiteSpace(@this);
    }
}
