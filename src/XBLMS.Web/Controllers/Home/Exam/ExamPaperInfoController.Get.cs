﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Configuration;
using XBLMS.Dto;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Home.Exam
{
    public partial class ExamPaperInfoController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var user = await _authManager.GetUserAsync();
            if (user == null) { return Unauthorized(); }

            var paper = await _examPaperRepository.GetAsync(request.Id);
            await _examManager.GetPaperInfo(paper, user, request.PlanId, request.CourseId, true);

            return new GetResult
            {
                Item = paper
            };
        }
    }
}
