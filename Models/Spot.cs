namespace UpxApi.Models
{
    public class Spot
    {
        public int SpotId { get; set; }
        public string? Region { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Address { get; set; }
        public bool Occupied { get; set; }
    }
}
