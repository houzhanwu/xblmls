﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseFilesController
    {
        [HttpGet, Route(RouteActionsDownload)]
        public async Task<ActionResult> ActionsDownload([FromQuery] IdRequest request)
        {
            var admin = await _authManager.GetAdminAsync();
            if (admin == null) return Unauthorized();

            var file = await _studyCourseFilesRepository.GetAsync(request.Id);
            if (file == null || string.IsNullOrEmpty(file.Url)) return NotFound();

            var filePath = PathUtils.Combine(_settingsManager.WebRootPath, file.Url);
            if (!FileUtils.IsFileExists(filePath)) return NotFound();

            await _authManager.AddAdminLogAsync("下载课件", file.FileName);

            return this.Download(filePath);
        }
    }
}
