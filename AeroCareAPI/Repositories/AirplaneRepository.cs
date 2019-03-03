using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

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
        }

        #endregion

        #region Methods...

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
            await _AirplanesCollection.ReplaceOneAsync(plane => plane.RegNo == regNo, airplane);
        }
        #endregion

        #endregion

    }
    #endregion

}
