using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace API.Repositories
{
    #region EngineeringTeamsRepository
    public class EngineeringTeamsRepository
    {
        #region Constants...

        private const string TEAMS_COLLECTION_NAME = "Teams";

        #endregion

        #region Class Variables...

        private readonly IMongoCollection<EngineeringTeamEntity> _TeamsCollection;

        #endregion

        #region Constructors...

        public EngineeringTeamsRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("MongoStorageConnectionString"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DBName"));
            _TeamsCollection = database.GetCollection<EngineeringTeamEntity>(TEAMS_COLLECTION_NAME);
            AddUniqueIndex("Name");
        }

        #endregion

        #region Methods...

        #region AddUniqueIndex
        private async void AddUniqueIndex(string property)
        {
            var options = new CreateIndexOptions() { Unique = true };
            var field = new StringFieldDefinition<EngineeringTeamEntity>(property);
            var indexDefinition = new IndexKeysDefinitionBuilder<EngineeringTeamEntity>().Ascending(field);
            CreateIndexModel<EngineeringTeamEntity> _Index = new CreateIndexModel<EngineeringTeamEntity>(indexDefinition, options);
            await _TeamsCollection.Indexes.CreateOneAsync(_Index);
        }
        #endregion

        #region Create
        public async Task<EngineeringTeamEntity> Create(EngineeringTeamEntity team)
        {
            await _TeamsCollection.InsertOneAsync(team);
            return team;
        }
        #endregion

        #region Get
        public async Task<List<EngineeringTeamEntity>> Get()
        {
            IAsyncCursor<EngineeringTeamEntity> _Results = await _TeamsCollection.FindAsync(team => true);
            return await _Results.ToListAsync();
        }
        #endregion

        #region Get(string name)
        public async Task<EngineeringTeamEntity> Get(string name)
        {
            IAsyncCursor<EngineeringTeamEntity> _Results = await _TeamsCollection.FindAsync(team => team.Name == name);
            return await _Results.FirstOrDefaultAsync();
        }
        #endregion

        #region Remove(team)
        public async Task Remove(EngineeringTeamEntity team)
        {
            await _TeamsCollection.DeleteOneAsync(t => t.Id == team.Id);
        }
        #endregion

        #region Remove(name)
        public async Task Remove(string name)
        {
            await _TeamsCollection.DeleteOneAsync(t => t.Name == name);
        }
        #endregion

        #region Update
        public async Task Update(string name, EngineeringTeamEntity team)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(team, serializerSettings)) } };
            await _TeamsCollection.UpdateOneAsync(Builders<EngineeringTeamEntity>.Filter.Eq(t => t.Name, name), bson);
        }
        #endregion

        #endregion
    }
    #endregion
}
