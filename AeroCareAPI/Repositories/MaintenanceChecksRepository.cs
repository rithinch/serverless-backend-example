using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace API.Repositories
{
    public class MaintenanceChecksRepository
    {
        #region Constants...

        private const string MAINTENANCE_COLLECTION_NAME = "MaintenanceChecks";

        #endregion

        #region Class Variables...

        private readonly IMongoCollection<MaintenanceCheckEntity> _MaintenanceChecksCollection;

        #endregion

        #region Constructors...

        public MaintenanceChecksRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("MongoStorageConnectionString"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DBName"));
            _MaintenanceChecksCollection = database.GetCollection<MaintenanceCheckEntity>(MAINTENANCE_COLLECTION_NAME);

        }

        #endregion

        #region Methods...

        #region Create
        public async Task<MaintenanceCheckEntity> Create(MaintenanceCheckEntity maintenanceCheck)
        {
            await _MaintenanceChecksCollection.InsertOneAsync(maintenanceCheck);
            return maintenanceCheck;
        }
        #endregion

        #region Get
        public async Task<List<MaintenanceCheckEntity>> Get()
        {
            IAsyncCursor<MaintenanceCheckEntity> _Results = await _MaintenanceChecksCollection.FindAsync(c => true);
            return await _Results.ToListAsync();
        }
        #endregion

        #region Get(string Id)
        public async Task<MaintenanceCheckEntity> Get(string id)
        {
            IAsyncCursor<MaintenanceCheckEntity> _Results = await _MaintenanceChecksCollection.FindAsync(c => c.Id == id);
            return await _Results.FirstOrDefaultAsync();
        }
        #endregion

        #region GetAllByFlightRegNo(string regNo)
        public async Task<List<MaintenanceCheckEntity>> GetAllByFlightRegNo(string regNo)
        {
            IAsyncCursor<MaintenanceCheckEntity> _Results = await _MaintenanceChecksCollection.FindAsync(c => c.FlightRegNo == regNo);
            return await _Results.ToListAsync();
        }
        #endregion

        #region Remove(check)
        public async Task Remove(MaintenanceCheckEntity check)
        {
            await Remove(check.Id);
        }
        #endregion

        #region Remove(id)
        public async Task Remove(string id)
        {
            await _MaintenanceChecksCollection.DeleteOneAsync(c => c.Id == id);
        }
        #endregion

        #region Update
        public async Task Update(string id, MaintenanceCheckEntity check)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(check, serializerSettings)) } };
            await _MaintenanceChecksCollection.UpdateOneAsync(Builders<MaintenanceCheckEntity>.Filter.Eq(c => c.Id, id), bson);
        }
        #endregion

        #endregion
    }
}

