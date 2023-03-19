namespace DockerHelper.Core.Utils;

public static class Consts
{
    public static class Keys
    {
        public const string EnvsKey = "envs";
        public const string EmptyKey = "empty";
        public const string TitleKey = "title";
        public const string PortsKey = "ports";
        public const string VolumesKey = "volumes";
        public const string DefaultKey = "default";
        public const string ContainerKey = "container";
        public const string DeletedImageKey = "deleted_image";
        public const string NotExistingIdKey = "not_existing_id";
        public const string NotExistingCmdKey = "not_existing_cmd";
    }

    public static class ViewNames
    {
        public const string HistoryConsole = "_history_console_";
        public const string ExceptionsConsole = "_exceptions_console_";
        public const string PortsConsole = "_ports_console_";
        public const string VolumesConsole = "_volumes_console_";
        public const string ContainersConsole = "_containers_console_";
        public const string EnvsConsole = "_envs_console_";
        public const string ImagesViewer = "_images_scroll_viewer_";
    }

    public static class Dialogs
    {
        public const string PortsDialog = "ports_dialog";
        public const string VolumesDialog = "volumes_dialog";
        public const string EnvironmentsDialog = "environments_dialog";
    }
}
