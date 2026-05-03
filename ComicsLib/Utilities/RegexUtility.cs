using System.Text.RegularExpressions;

namespace ComicsLib.Utilities
{
    public static partial class RegexUtility
    {
        [GeneratedRegex(@"^[0-9]+$")]
        public static partial Regex NumberRegex();

        [GeneratedRegex(@"\/([^\/?]+)\?")]
        public static partial Regex FilenameRegex();
        [GeneratedRegex("\\d+$")]
        public static partial Regex ChapterRegex();

        [GeneratedRegex(@"^(https?:\/\/)?([^\/]+)\/")]
        public static partial Regex HostRegex();

        [GeneratedRegex("\\d{10}")]
        public static partial Regex UUIDRegex();

        [GeneratedRegex("^\\d+ ")]
        public static partial Regex PageRegex();

        [GeneratedRegex(@"\d{2}-\d{2}-\d{2}$")]
        public static partial Regex DateRegex();
    }
}
