﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace XBLMS.Web.Controllers.Admin.Settings.Block
{
    public partial class SettingsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var admin = await _authManager.GetAdminAsync();

            var rules = await _ruleRepository.GetAllAsync(0);

            return new GetResult
            {
                Rules = rules
            };
        }
    }
}
