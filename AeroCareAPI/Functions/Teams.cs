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
    #region Teams
    public static class Teams
    {
        #region Constants...
        private static EngineeringTeamsRepository _TeamsRepo;
        #endregion

        #region Function Definitions

        #region AddNewTeam
        [FunctionName("AddNewTeam")]
        public static async Task<IActionResult> AddNewTeam([HttpTrigger(AuthorizationLevel.Function, "post", Route = "teams/")]HttpRequest req, ILogger log)
        {
            log.LogInformation($"Add new Team request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                EngineeringTeamEntity _Entity = JsonConvert.DeserializeObject<EngineeringTeamEntity>(_RequestBody);
                await TeamsRepo.Create(_Entity);
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

        #region GetAllEngineersInTeam
        [FunctionName("GetAllEngineersInTeam")]
        public static async Task<IActionResult> GetAllEngineersInTeam([HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams/{name}/engineers")]HttpRequest req, string name, ILogger log)
        {
            log.LogInformation("Get All Engineers in Team Requested");

            List<EngineerEntity> _Engineers = null;

            if (await TeamsRepo.Get(name) != null)
            {
                _Engineers = await new EngineersRepository().GetByTeam(name);
            }
            else
            {
                return new NotFoundResult();
            }
             
            return new JsonResult(_Engineers);
        }
        #endregion

        #region GetAllTeams
        [FunctionName("GetAllTeams")]
        public static async Task<IActionResult> GetAllEngineers([HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams/")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Get All Teams Requested");

            List<EngineeringTeamEntity> _Teams = await TeamsRepo.Get();

            return new JsonResult(_Teams);
        }
        #endregion

        #region GetSingleTeam
        [FunctionName("GetSingleTeam")]
        public static async Task<IActionResult> GetSingleTeam([HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams/{name}/")]HttpRequest req, string name, ILogger log)
        {
            log.LogInformation($"GET single team {name}");

            EngineeringTeamEntity _Result = null;

            try
            {
                _Result = await TeamsRepo.Get(name);
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

        #region RemoveTeam
        [FunctionName("RemoveTeam")]
        public static async Task<IActionResult> RemoveTeam([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "teams/{name}")]HttpRequest req, string name, ILogger log)
        {
            log.LogInformation($"Remove team request");

            try
            {
                if (await TeamsRepo.Get(name) != null)
                {
                    await TeamsRepo.Remove(name);
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

        #region UpdateTeam
        [FunctionName("UpdateTeam")]
        public static async Task<IActionResult> UpdateTeam([HttpTrigger(AuthorizationLevel.Function, "put", Route = "teams/{name}")]HttpRequest req, string name, ILogger log)
        {
            log.LogInformation($"Update existing team request");

            try
            {
                string _RequestBody = await new StreamReader(req.Body).ReadToEndAsync();
                EngineeringTeamEntity _Entity = JsonConvert.DeserializeObject<EngineeringTeamEntity>(_RequestBody);

                if (await TeamsRepo.Get(name) != null)
                {
                    await TeamsRepo.Update(name, _Entity);
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

        #region TeamsRepo
        internal static EngineeringTeamsRepository TeamsRepo
        {
            get
            {
                if (_TeamsRepo == null)
                {
                    _TeamsRepo = new EngineeringTeamsRepository();

                }
                return _TeamsRepo;
            }
            set
            {
                _TeamsRepo = value;
            }
        }
        #endregion

        #endregion
    }
    #endregion
}
