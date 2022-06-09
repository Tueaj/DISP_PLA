namespace InventoryService.Models
{
    public class MongoConnectionSettings
    {
        public const string Key = "MongoConnection";

        public string HostName { get; set; }

        public int Port { get; set; }

        public string DatabaseName { get; set; }

        public MongoConnectionCredentials? Credentials { get; set; }
    }

    public class MongoConnectionCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}