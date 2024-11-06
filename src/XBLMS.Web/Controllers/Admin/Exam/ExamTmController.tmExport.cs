﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Configuration;
using XBLMS.Core.Utils;
using XBLMS.Core.Utils.Office;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamTmController
    {
        [HttpGet, Route(RouteExportExcel)]
        public async Task<ActionResult<StringResult>> Export([FromQuery] GetSearchRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Export))
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();

            var treeIds = new List<int>();
            if (request.TreeId > 0)
            {
                if (request.TreeIsChildren)
                {
                    treeIds = await _examTmTreeRepository.GetIdsAsync(request.TreeId);
                }
                else
                {
                    treeIds.Add(request.TreeId);
                }
            }
            var group = await _examTmGroupRepository.GetAsync(request.TmGroupId);

            var (total, list) = await _examTmRepository.Group_ListAsync(auth, group, treeIds, request.TxId, request.Nandu, request.Keyword, request.Order, request.OrderType, request.IsStop, 1, int.MaxValue);

            var fileName = "题目列表.xlsx";
            var filePath = _pathManager.GetDownloadFilesPath(fileName);

            var excelObject = new ExcelObject(_databaseManager, _pathManager, _organManager, _examManager);
            await excelObject.CreateExcelFileForTmAsync(list, filePath);

            var downloadUrl = _pathManager.GetRootUrlByPath(filePath);

            return new StringResult
            {
                Value = downloadUrl
            };
        }



    }
}
