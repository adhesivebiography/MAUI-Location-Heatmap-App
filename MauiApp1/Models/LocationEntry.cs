using SQLite;

namespace MauiApp1.Models
{
    public class LocationEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Latitude of the GPS position
        public double Latitude { get; set; }

        // Longitude of the GPS position
        public double Longitude { get; set; }

        // When the position was recorded
        public DateTime Timestamp { get; set; }
    }
}
