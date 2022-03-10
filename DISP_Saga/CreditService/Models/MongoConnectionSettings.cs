namespace CreditService.Models;

public class MongoConnectionSettings
{
    public const string Key = "MongoConnection";
    
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }
}