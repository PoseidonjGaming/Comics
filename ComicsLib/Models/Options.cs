namespace ComicsLib.Models
{
    public class Options
    {
        private static Comic FallBack = new(
                "https://exemple_url.com",
                "https://exemple_baseurl.com",
                "package name",
                "filename",
                "host.com",
                0,
                "author");

        public static readonly string[] OptionList = [
            "Settings_Host_Search",
            "Settings_Host_Confirm",
            "Settings_Path",
            "Settings_Excluded_Host"];
        public Comic? Comic { get; set; }
        public string[] Hosts { get; set; } = [];
        public string[] Confirms { get; set; } = [];
        public string[] Paths { get; set; } = [];
        public string[] ExcludedHosts { get; set; } = [];
        public sbyte Period { get; set; } = 0;
        private string Lang { get; set; } = "en";

        public Options(Comic comic, string[] hosts, string[] confirms, string[] paths,
            string[] excludedHosts, sbyte period, string lang)
        {
            Comic = comic;
            Hosts = hosts;
            Confirms = confirms;
            Paths = paths;
            Period = period;
            ExcludedHosts = excludedHosts;
            Lang = lang;
        }


        public Options() {
            Comic = FallBack;
        }
    }
}
