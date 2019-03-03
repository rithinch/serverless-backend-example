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
    public static class MaintenanceChecks
    {
        #region Constants...
        private static MaintenanceChecksRepository _ChecksRepo;
        private static TasksRepository _TasksRepo;
        #endregion

        #region Function Definitions

        #region AddNewMaintenanceCheck
        [FunctionName("AddNewMaintenanceCheck")]
        public static async Task<IActionResult> AddNewMaintenanceCheck([HttpTrigger(AuthorizationLevel.Function, "post", Route = "maintenance/")]HttpRequest req, ILogger log)
        {
            log.LogInformation($"Add new check request");
            MaintenanceCheckEntity _Entity = null;
            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                _Entity = JsonConvert.DeserializeObject<MaintenanceCheckEntity>(_RequestBody);

                if (new AirplaneRepository().Get(_Entity.FlightRegNo) == null)
                {
                    return new BadRequestObjectResult("Please provide a valid flight.");
                }

                await ChecksRepo.Create(_Entity);
            }
            catch (Exception _Exception)
            {
                log.LogError("Error in Deserializing");
                log.LogError(_Exception.Message);
                return new BadRequestResult();
            }

            return new OkObjectResult(_Entity);
        }
        #endregion

        #region AddNewTaskToMaintenanceCheck
        [FunctionName("AddNewTaskToMaintenanceCheck")]
        public static async Task<IActionResult> AddNewTaskToMaintenanceCheck([HttpTrigger(AuthorizationLevel.Function, "post", Route = "maintenance/{id}/tasks/")]HttpRequest req, string id, ILogger log)
        {
            log.LogInformation($"Add new task to check request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                TaskEntity _Entity = JsonConvert.DeserializeObject<TaskEntity>(_RequestBody);
                await TasksRepo.Create(_Entity);
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

        #region GetSingleCheck
        [FunctionName("GetSingleCheck")]
        public static async Task<IActionResult> GetSingleCheck([HttpTrigger(AuthorizationLevel.Function, "get", Route = "maintenance/{id}/")]HttpRequest req, string id, ILogger log)
        {
            log.LogInformation($"GET single check {id}");

            MaintenanceCheckEntity _Result = null;

            try
            {
                _Result = await ChecksRepo.Get(id);
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

        #region GetAllTasksInCheck
        [FunctionName("GetAllTasksInCheck")]
        public static async Task<IActionResult> GetAllTasksInCheck([HttpTrigger(AuthorizationLevel.Function, "get", Route = "maintenance/{id}/tasks/")]HttpRequest req, string id, ILogger log)
        {
            log.LogInformation($"GET all tasks in check {id}");

            List<TaskEntity> _Result = null;

            try
            {
                _Result = await new TasksRepository().GetAllByMaitenanceID(id);

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

        #region RemoveCheck
        [FunctionName("RemoveCheck")]
        public static async Task<IActionResult> RemoveCheck([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "maintenance/{id}")]HttpRequest req, string id, ILogger log)
        {
            log.LogInformation($"Remove check request");

            try
            {
                if (await ChecksRepo.Get(id) != null)
                {
                    await ChecksRepo.Remove(id);
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

        #region RemoveTask
        [FunctionName("RemoveTask")]
        public static async Task<IActionResult> RemoveTask([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "maintenance/{id}/tasks/{taskID}")]HttpRequest req, string id, string taskID, ILogger log)
        {
            log.LogInformation($"Remove task in check request");

            try
            {
                if (await TasksRepo.Get(taskID) != null)
                {
                    await TasksRepo.Remove(taskID);
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

        #region UpdateCheck
        [FunctionName("UpdateCheck")]
        public static async Task<IActionResult> UpdateCheck([HttpTrigger(AuthorizationLevel.Function, "put", Route = "maintenance/{id}")]HttpRequest req, string id, ILogger log)
        {
            log.LogInformation($"Update existing engineer request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                MaintenanceCheckEntity _Entity = JsonConvert.DeserializeObject<MaintenanceCheckEntity>(_RequestBody);

                if (await ChecksRepo.Get(id) != null)
                {
                    await ChecksRepo.Update(id, _Entity);
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

        #region UpdateTask
        [FunctionName("UpdateTask")]
        public static async Task<IActionResult> UpdateTask([HttpTrigger(AuthorizationLevel.Function, "put", Route = "maintenance/{id}/tasks/{taskID}")]HttpRequest req, string id, string taskID, ILogger log)
        {
            log.LogInformation($"Update existing task in check request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                TaskEntity _Entity = JsonConvert.DeserializeObject<TaskEntity>(_RequestBody);

                if (await TasksRepo.Get(taskID) != null)
                {
                    await TasksRepo.Update(taskID, _Entity);
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

        #region ChecksRepo
        internal static MaintenanceChecksRepository ChecksRepo
        {
            get
            {
                if (_ChecksRepo == null)
                {
                    _ChecksRepo = new MaintenanceChecksRepository();

                }
                return _ChecksRepo;
            }
            set
            {
                _ChecksRepo = value;
            }
        }
        #endregion

        #region TasksRepo
        internal static TasksRepository TasksRepo
        {
            get
            {
                if (_TasksRepo == null)
                {
                    _TasksRepo = new TasksRepository();

                }
                return _TasksRepo;
            }
            set
            {
                _TasksRepo = value;
            }
        }
        #endregion

        #endregion
    }
    #endregion
}