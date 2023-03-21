namespace DockerHelper.Modules.Docker.Utils;

public class DockerDesktop
{
    public static class Hints
    {
        public const string Run = "[Try running the (Docker Desktop) application]";
        public const string AlreadyStarted = "[The (Docker Desktop) application already started]";
        public const string Troubleshoot = "[The button on the specified path can help: (Docker Desktop) application => Troubleshoot => Reset to factory defaults]";
    }

    public static class Questions
    {
        public const string Start = "Do you want to start the (Docker Desktop) application?";
    }
}
