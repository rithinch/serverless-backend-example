using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace API.Repositories
{
    #region AirplaneRepository
    public class AirplaneRepository
    {
        #region Constants...

        private const string AIRPLANES_COLLECTION_NAME = "Airplanes";

        #endregion

        #region Class Variables...

        private readonly IMongoCollection<Airplane> _AirplanesCollection;

        #endregion

        #region Constructors...

        public AirplaneRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("MongoStorageConnectionString"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DBName"));
            _AirplanesCollection = database.GetCollection<Airplane>(AIRPLANES_COLLECTION_NAME);
            AddUniqueIndex("RegNo");
        }

        #endregion

        #region Methods...

        #region AddUniqueIndex
        private async void AddUniqueIndex(string property)
        {
            var options = new CreateIndexOptions() { Unique = true };
            var field = new StringFieldDefinition<Airplane>(property);
            var indexDefinition = new IndexKeysDefinitionBuilder<Airplane>().Ascending(field);
            CreateIndexModel<Airplane> _Index = new CreateIndexModel<Airplane>(indexDefinition, options);
            await _AirplanesCollection.Indexes.CreateOneAsync(_Index);
        }
        #endregion

        #region Create
        public async Task<Airplane> Create(Airplane airplane)
        {
            await _AirplanesCollection.InsertOneAsync(airplane);
            return airplane;
        }
        #endregion

        #region Get
        public async Task<List<Airplane>> Get()
        {
            IAsyncCursor<Airplane> _Results = await _AirplanesCollection.FindAsync(plane => true);
            return await _Results.ToListAsync();
        }
        #endregion

        #region Get(string regNo)
        public async Task<Airplane> Get(string regNo)
        {
            IAsyncCursor<Airplane> _Results = await _AirplanesCollection.FindAsync(plane => plane.RegNo == regNo);
            return await _Results.FirstOrDefaultAsync();
        }
        #endregion

        #region Remove(Airplane)
        public async Task Remove(Airplane airplane)
        {
            await _AirplanesCollection.DeleteOneAsync(plane => plane.Id == airplane.Id);
        }
        #endregion

        #region Remove(regNo)
        public async Task Remove(string regNo)
        {
            await _AirplanesCollection.DeleteOneAsync(plane => plane.RegNo == regNo);
        }
        #endregion

        #region Update
        public async Task Update(string regNo, Airplane airplane)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(airplane, serializerSettings)) } };
            await _AirplanesCollection.UpdateOneAsync(Builders<Airplane>.Filter.Eq(plane => plane.RegNo, regNo), bson);
        }
        #endregion

        #endregion

    }
    #endregion
}
