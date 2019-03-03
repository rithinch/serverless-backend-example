using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace API.Repositories
{
    public class EngineersRepository
    {
        #region Constants...

        private const string ENGINEERS_COLLECTION_NAME = "Engineers";

        #endregion

        #region Class Variables...

        private readonly IMongoCollection<EngineerEntity> _EngineersCollection;

        #endregion

        #region Constructors...

        public EngineersRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("MongoStorageConnectionString"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DBName"));
            _EngineersCollection = database.GetCollection<EngineerEntity>(ENGINEERS_COLLECTION_NAME);
            AddUniqueIndex("CustomID");
        }

        #endregion

        #region Methods...

        #region AddUniqueIndex
        private void AddUniqueIndex(string property)
        {
            var options = new CreateIndexOptions() { Unique = true };
            var field = new StringFieldDefinition<EngineerEntity>(property);
            var indexDefinition = new IndexKeysDefinitionBuilder<EngineerEntity>().Ascending(field);
            CreateIndexModel<EngineerEntity> _Index = new CreateIndexModel<EngineerEntity>(indexDefinition, options);
            _EngineersCollection.Indexes.CreateOne(_Index);
        }
        #endregion

        #region Create
        public async Task<EngineerEntity> Create(EngineerEntity engineer)
        {
            await _EngineersCollection.InsertOneAsync(engineer);
            return engineer;
        }
        #endregion

        #region Get
        public async Task<List<EngineerEntity>> Get()
        {
            IAsyncCursor<EngineerEntity> _Results = await _EngineersCollection.FindAsync(eng => true);
            return await _Results.ToListAsync();
        }
        #endregion

        #region Get(string customID)
        public async Task<EngineerEntity> Get(string customID)
        {
            IAsyncCursor<EngineerEntity> _Results = await _EngineersCollection.FindAsync(eng => eng.CustomID == customID);
            return await _Results.FirstOrDefaultAsync();
        }
        #endregion

        #region GetByTeam(string name)
        public async Task<List<EngineerEntity>> GetByTeam(string name)
        {
            IAsyncCursor<EngineerEntity> _Results = await _EngineersCollection.FindAsync(eng => eng.TeamID == name);
            return await _Results.ToListAsync();
        }
        #endregion

        #region Remove(engineer)
        public async Task Remove(EngineerEntity engineer)
        {
            await _EngineersCollection.DeleteOneAsync(eng => eng.Id == engineer.Id);
        }
        #endregion

        #region Remove(customID)
        public async Task Remove(string customID)
        {
            await _EngineersCollection.DeleteOneAsync(eng => eng.CustomID == customID);
        }
        #endregion

        #region Update
        public async Task Update(string customID, EngineerEntity engineer)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(engineer, serializerSettings)) } };
            await _EngineersCollection.UpdateOneAsync(Builders<EngineerEntity>.Filter.Eq(eng => eng.CustomID, customID), bson);
        }
        #endregion

        #endregion
    }
}
