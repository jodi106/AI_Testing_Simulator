using Assets.Enums;

namespace Assets.Entities
{
    public class MenuOptions
    {
        public MenuOptions()
        {
            Visibility = 0.0;

            TimeDay = TimeDay.Morning;

            Date = "";
        }

        public double Visibility { get; set; }

        public TimeDay TimeDay { get; set; }

        public string Date { get; set; }
    }
}
