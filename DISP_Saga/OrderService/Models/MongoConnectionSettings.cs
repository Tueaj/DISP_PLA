namespace OrderService.Models
{
    public class MongoConnectionSettings
    {
        public const string Key = "MongoConnection";

        public string HostName { get; set; }

        public int Port { get; set; }

        public string DatabaseName { get; set; }
    }
}