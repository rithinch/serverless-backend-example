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
    #region Airplanes
    public static class Airplanes
    {
        #region Constants...
        private static AirplaneRepository _AirplaneRepo;
        #endregion

        #region Function Definitions

        #region AddNewAirplane
        [FunctionName("AddNewAirplane")]
        public static async Task<IActionResult> AddNewAirplane([HttpTrigger(AuthorizationLevel.Function, "post", Route = "airplanes/")]HttpRequest req, ILogger log)
        {
            log.LogInformation($"Add new airplane request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                AirplaneEntity _Entity = JsonConvert.DeserializeObject<AirplaneEntity>(_RequestBody);
                await AirplaneRepo.Create(_Entity);
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

        #region GetAllAirplanes
        [FunctionName("GetAllAirplanes")]
        public static async Task<IActionResult> GetAllAirplanes([HttpTrigger(AuthorizationLevel.Function, "get", Route = "airplanes/")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Get All Airplanes Requested");

            List<AirplaneEntity> _Airplanes = await AirplaneRepo.Get();

            return new JsonResult(_Airplanes);
        }
        #endregion

        #region GetAllMaintenanceChecksForFlight
        [FunctionName("GetAllMaintenanceChecksForFlight")]
        public static async Task<IActionResult> GetAllMaintenanceChecksForFlight([HttpTrigger(AuthorizationLevel.Function, "get", Route = "airplanes/{regNo}/maintenance-history/")]HttpRequest req, string regNo, ILogger log)
        {
            log.LogInformation("Get All Maintenance Checks for a flight");

            List<MaintenanceCheckEntity> _Checks = await new MaintenanceChecksRepository().GetAllByFlightRegNo(regNo);

            return new JsonResult(_Checks);
        }
        #endregion

        #region GetSingleAirplane
        [FunctionName("GetSingleAirplane")]
        public static async Task<IActionResult> GetSingleAirplane([HttpTrigger(AuthorizationLevel.Function, "get", Route ="airplanes/{regNo}/")]HttpRequest req, string regNo, ILogger log)
        {
            log.LogInformation($"GET single airplane {regNo}");

            AirplaneEntity _Result = null;

            try
            {
                _Result = await AirplaneRepo.Get(regNo);
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

        #region RemoveAirplane
        [FunctionName("RemoveAirplane")]
        public static async Task<IActionResult> RemoveAirplane([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "airplanes/{regNo}")]HttpRequest req, string regNo, ILogger log)
        {
            log.LogInformation($"Remove airplane request");

            try
            {
                if (await AirplaneRepo.Get(regNo) != null)
                {
                    await AirplaneRepo.Remove(regNo);
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

        #region UpdateAirplane
        [FunctionName("UpdateAirplane")]
        public static async Task<IActionResult> UpdateAirplane([HttpTrigger(AuthorizationLevel.Function, "put", Route = "airplanes/{regNo}")]HttpRequest req, string regNo, ILogger log)
        {
            log.LogInformation($"Update existing airplane request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                AirplaneEntity _Entity = JsonConvert.DeserializeObject<AirplaneEntity>(_RequestBody);

                if (await AirplaneRepo.Get(regNo) != null)
                {
                    await AirplaneRepo.Update(regNo, _Entity);
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

        #region AirplaneRepo
        internal static AirplaneRepository AirplaneRepo
        {
            get
            {
                if (_AirplaneRepo == null)
                {
                    _AirplaneRepo = new AirplaneRepository();

                }
                return _AirplaneRepo;
            }
            set
            {
                _AirplaneRepo = value;
            }
        }
        #endregion

        #endregion
    }
    #endregion
}
