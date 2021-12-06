namespace AevApp.Model
{
    public class Global
    {
        public static string Orientation = "Portrait";

        public static bool IsPortrait => Orientation == AevApp.Constants.Orientation.Portrait;
    }
}