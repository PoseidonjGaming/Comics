using ModernDownloadComics.model;

namespace ModernDownloadComics.Models
{
    public class Options
    {
        private static readonly string[] OptionList = ["Host", "Confirms", "Paths"];
        public Comic? Comic { get; set; }
        public string[] Hosts { get; set; } = [];
        public string[] Confirms { get; set; } = [];
        public string[] Paths { get; set; } = [];
        public sbyte Period { get; set; } = 0;
        public bool IsPeriodEnabled { get; set; } = false;

        public Options(Comic comic, string[] hosts, string[] confirms, string[] paths, sbyte period)
        {
            Comic = comic;
            Hosts = hosts;
            Confirms = confirms;
            Paths = paths;
            Period = period;
        }


        public Options() { }

        /*public static void AddToCombo(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            foreach (var option in OptionList)
            {
                comboBox.Items.Add(option);
            }
        }*/

    }
}
