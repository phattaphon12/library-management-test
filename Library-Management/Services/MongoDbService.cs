using MongoDB.Driver;
public class MongoDbService
{
    private readonly IMongoDatabase _database;
    public MongoDbService(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    // ดึง Collection สำหรับ Books
    public IMongoCollection<Book> GetBooksCollection()
    {
        return _database.GetCollection<Book>("Books"); // ชื่อ Collection สำหรับ Books
    }

    // ดึง Collection สำหรับ Users
    public IMongoCollection<User> GetUsersCollection()
    {
        return _database.GetCollection<User>("Users"); // ชื่อ Collection สำหรับ Users
    }

    public IMongoCollection<Loan> GetLoansCollection()
    {
        return _database.GetCollection<Loan>("Loans");
    }
}