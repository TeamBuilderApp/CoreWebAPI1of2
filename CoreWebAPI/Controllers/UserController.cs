using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace CoreWebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[EnableCors("_myAllowSpecificOrigins")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<User> _users;

    public UserController(ILogger<UserController> logger, IMongoClient client, IConfiguration configuration)
    {
        _logger = logger;
        _client = client;
        var databaseName = configuration["MongoDB:DatabaseName"];
        var collectionName = configuration["MongoDB:CollectionName"];

        _database = client.GetDatabase(databaseName);
        _users = _database.GetCollection<User>(collectionName);
    }

    [HttpPost]
    public async Task CreateAsync(User newUser)
    {
        List<User> allUsers = await GetAsync();
        newUser.id = allUsers.Count + 40 + 1;
        await _users.InsertOneAsync(newUser); ;
    }
    
    [HttpGet]
    public async Task<List<User>> GetAsync() =>
            await _users.Find(_ => true).ToListAsync();

    [HttpGet("{id}")]
    public async Task<User?> GetAsync(int id) =>
        await _users.Find(x => x.id == id).FirstOrDefaultAsync();

    [HttpPut]
    public async Task UpdateAsync(int id, User updateUser) =>
        await _users.ReplaceOneAsync(x => x.id == id, updateUser);
}
