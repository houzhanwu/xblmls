﻿using Datory;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home.Study
{
    public partial class StudyPlanController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var resultList = new List<StudyPlanUser>();
            var (total, list) = await _studyPlanUserRepository.GetListAsync("", user.Id, request.PageIndex, request.PageSize);
            if (total > 0)
            {
                foreach (var item in list)
                {
                    await _studyManager.User_GetPlanInfo(item);
                    resultList.Add(item);
                }
            }
            return new GetResult
            {
                Total = total,
                List = resultList
            };
        }

        [HttpGet, Route(RouteItem)]
        public async Task<ActionResult<ItemResult<StudyPlanUser>>> GetItem([FromQuery] IdRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var planUser = await _studyPlanUserRepository.GetAsync(request.Id);
            await _studyManager.User_GetPlanInfo(planUser);

            return new ItemResult<StudyPlanUser>
            {
                Item = planUser
            };
        }
    }
}
