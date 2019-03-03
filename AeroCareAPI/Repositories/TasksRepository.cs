using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace API.Repositories
{
    #region TasksRepository
    public class TasksRepository
    {
        #region Constants...

        private const string TASKS_COLLECTION_NAME = "Tasks";

        #endregion

        #region Class Variables...

        private readonly IMongoCollection<TaskEntity> _TasksCollection;

        #endregion

        #region Constructors...

        public TasksRepository()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("MongoStorageConnectionString"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DBName"));
            _TasksCollection = database.GetCollection<TaskEntity>(TASKS_COLLECTION_NAME);
        }

        #endregion

        #region Methods...

        #region Create
        public async Task<TaskEntity> Create(TaskEntity task)
        {
            await _TasksCollection.InsertOneAsync(task);
            return task;
        }
        #endregion

        #region Get
        public async Task<List<TaskEntity>> Get()
        {
            IAsyncCursor<TaskEntity> _Results = await _TasksCollection.FindAsync(t => true);
            return await _Results.ToListAsync();
        }
        #endregion

        #region GetAllByMaintenanceID
        public async Task<List<TaskEntity>> GetAllByMaitenanceID(string id)
        {
            IAsyncCursor<TaskEntity> _Results = await _TasksCollection.FindAsync(t => t.MaintenanceCheckID == id);
            return await _Results.ToListAsync();
        }
        #endregion

        #region Get(string id)
        public async Task<TaskEntity> Get(string id)
        {
            IAsyncCursor<TaskEntity> _Results = await _TasksCollection.FindAsync(t => t.Id == id);
            return await _Results.FirstOrDefaultAsync();
        }
        #endregion

        #region Remove(task)
        public async Task Remove(TaskEntity task)
        {
            await Remove(task.Id);
        }
        #endregion

        #region Remove(id)
        public async Task Remove(string id)
        {
            await _TasksCollection.DeleteOneAsync(t => t.Id == id);
        }
        #endregion

        #region Update
        public async Task Update(string id, TaskEntity task)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(task, serializerSettings)) } };
            await _TasksCollection.UpdateOneAsync(Builders<TaskEntity>.Filter.Eq(t => t.Id, id), bson);
        }
        #endregion

        #endregion
    }
    #endregion
}

