using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API.Entities;
using API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Functions
{
    #region Engineers
    public static class Engineers
    {
        #region Constants...
        private static EngineersRepository _EngineerRepo;
        #endregion

        #region Function Definitions

        #region AddNewEngineer
        [FunctionName("AddNewEngineer")]
        public static async Task<IActionResult> AddNewEngineer([HttpTrigger(AuthorizationLevel.Function, "post", Route = "engineers/")]HttpRequest req, ILogger log)
        {
            log.LogInformation($"Add new engineer request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                EngineerEntity _Entity = JsonConvert.DeserializeObject<EngineerEntity>(_RequestBody);

                if (_Entity.TeamID.Length > 0 &&  new EngineeringTeamsRepository().Get(_Entity.TeamID) == null)
                {
                    throw new Exception();

                }

                await EngineerRepo.Create(_Entity);

            }
            catch (Exception _Exception)
            {
                log.LogError("Error in Deserializing");
                log.LogError(_Exception.Message);
                return new BadRequestResult();
            }

            return new OkResult();
        }
        #endregion

        #region GetAllEngineers
        [FunctionName("GetAllEngineers")]
        public static async Task<IActionResult> GetAllEngineers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "engineers/")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Get All Airplanes Requested");

            List<EngineerEntity> _Engineers = await EngineerRepo.Get();

            return new JsonResult(_Engineers);
        }
        #endregion

        #region GetSingleEngineer
        [FunctionName("GetSingleEngineer")]
        public static async Task<IActionResult> GetSingleEngineer([HttpTrigger(AuthorizationLevel.Function, "get", Route = "engineers/{customID}/")]HttpRequest req, string customID, ILogger log)
        {
            log.LogInformation($"GET single engineer {customID}");

            EngineerEntity _Result = null;

            try
            {
                _Result = await EngineerRepo.Get(customID);
            }
            catch (Exception _Exception)
            {
                log.LogError(_Exception.Message);
            }

            if (_Result == null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(_Result);
        }
        #endregion

        #region RemoveEngineer
        [FunctionName("RemoveEngineer")]
        public static async Task<IActionResult> RemoveEngineer([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "engineers/{customID}")]HttpRequest req, string customID, ILogger log)
        {
            log.LogInformation($"Remove airplane request");

            try
            {
                if (await EngineerRepo.Get(customID) != null)
                {
                    await EngineerRepo.Remove(customID);
                }
                else
                {
                    return new NotFoundResult();
                }

            }
            catch (Exception _Exception)
            {
                log.LogError(_Exception.Message);
                return new NotFoundResult();
            }

            return new OkResult();
        }
        #endregion

        #region UpdateEngineer
        [FunctionName("UpdateEngineer")]
        public static async Task<IActionResult> UpdateEngineer([HttpTrigger(AuthorizationLevel.Function, "put", Route = "engineers/{customID}")]HttpRequest req, string customID, ILogger log)
        {
            log.LogInformation($"Update existing engineer request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                EngineerEntity _Entity = JsonConvert.DeserializeObject<EngineerEntity>(_RequestBody);

                if (await EngineerRepo.Get(customID) != null)
                {
                    if (_Entity.TeamID.Length > 0 && new EngineeringTeamsRepository().Get(_Entity.TeamID) == null)
                    {
                        return new BadRequestObjectResult("Please provide a valid team.");
                    }

                    await EngineerRepo.Update(customID, _Entity);
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (Exception _Exception)
            {
                log.LogError(_Exception.Message);
                return new NotFoundResult();
            }

            return new OkResult();
        }
        #endregion

        #endregion

        #region Properties

        #region EngineerRepo
        internal static EngineersRepository EngineerRepo
        {
            get
            {
                if (_EngineerRepo == null)
                {
                    _EngineerRepo = new EngineersRepository();

                }
                return _EngineerRepo;
            }
            set
            {
                _EngineerRepo = value;
            }
        }
        #endregion

        #endregion
    }
    #endregion
}
