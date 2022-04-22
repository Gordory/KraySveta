using System.Threading;
using System.Threading.Tasks;
using KraySveta.Api.DataLayer.Options;
using KraySveta.Api.Reports.Attendance;
using KraySveta.Core;
using KraySveta.External.ThatsMyBis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Serilog;

namespace KraySveta.Api.Controllers;

[ApiController]
[Route("test/mongo")]
public class TestMongoController : ControllerBase
{
    private readonly ILogger<TestMongoController> _logger;
    private readonly IOptions<MongoDbConfiguration> _mongoDbConfiguration;
    private readonly IMongoCollection<Model> _collection;

    public TestMongoController(
        IOptions<MongoDbConfiguration> mongoDbConfiguration, ILogger<TestMongoController> logger)
    {
        _mongoDbConfiguration = mongoDbConfiguration;
        _logger = logger;

        var connectionString = _mongoDbConfiguration.Value.ToString();
        var mongoDbClient = new MongoClient(connectionString);

        var database = mongoDbClient.GetDatabase(_mongoDbConfiguration.Value.Database);
        _collection = database.GetCollection<Model>("test");
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] Model model, CancellationToken token)
    {
        _logger.LogInformation("Received POST request");
            
        if (string.IsNullOrWhiteSpace(model.Name))
            return BadRequest("Model name should be not empty");

        await _collection.InsertOneAsync(model, cancellationToken: token);
        return Ok();
    }

    [HttpGet("{name}")]
    public async Task<ActionResult> Get(string name, CancellationToken token)
    {
        _logger.LogInformation("Received GET request");
            
        var filter = Builders<Model>.Filter.Eq(x => x.Name, name);
        var cursor = await _collection.FindAsync(filter, cancellationToken: token);
        var models = await cursor.ToListAsync(token);

        return Ok(models);
    }

    [BsonIgnoreExtraElements]
    public class Model
    {
        public string Name { get; set; }
            
        public int? Age { get; set; }
    }
}