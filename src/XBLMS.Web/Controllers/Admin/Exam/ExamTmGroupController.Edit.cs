﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Utils;

namespace XBLMS.Web.Controllers.Admin.Exam
{
    public partial class ExamTmGroupController
    {
        [HttpGet, Route(RouteEditGet)]
        public async Task<ActionResult<GetEditResult>> GetEdit([FromQuery] IdRequest request)
        {
            var auth = await _authManager.GetAuthorityAuth();

            var group = new ExamTmGroup();
            var selectOrganIds = new List<string>();
            if (request.Id > 0)
            {
                group = await _examTmGroupRepository.GetAsync(request.Id);
            }
            var tmTree = await _examManager.GetExamTmTreeCascadesAsync(auth);
            var groupTypeSelects = ListUtils.GetSelects<TmGroupType>();
            var txList = await _examTxRepository.GetListAsync();

            if (txList == null || txList.Count == 0)
            {
                await _examTxRepository.ResetAsync();
                txList = await _examTxRepository.GetListAsync();
            }

            return new GetEditResult
            {
                Group = group,
                GroupTypeSelects = groupTypeSelects,
                TmTree = tmTree,
                TxList = txList
            };
        }


        [HttpPost, Route(RouteEditPost)]
        public async Task<ActionResult<BoolResult>> PostEdit([FromBody] GetEditRequest request)
        {

            if (request.Group.Id > 0)
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Update))
                {
                    return this.NoAuth();
                }
            }
            else
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Add))
                {
                    return this.NoAuth();
                }
            }

            var auth = await _authManager.GetAuthorityAuth();


            if (request.Group.Id > 0)
            {
                var group = await _examTmGroupRepository.GetAsync(request.Group.Id);

                await _examTmGroupRepository.UpdateAsync(request.Group);

                await _authManager.AddAdminLogAsync("修改题目组", group.GroupName);
                await _authManager.AddStatLogAsync(StatType.ExamTmGroupUpdate, "修改题目组", group.Id, group.GroupName);
            }
            else
            {
                request.Group.CreatorId = auth.AdminId;
                request.Group.CompanyId = auth.CompanyId;
                request.Group.DepartmentId = auth.DepartmentId;

                var groupId = await _examTmGroupRepository.InsertAsync(request.Group);
                await _authManager.AddAdminLogAsync("新增题目组", request.Group.GroupName);
                await _authManager.AddStatLogAsync(StatType.ExamTmGroupAdd, "新增题目组", groupId, request.Group.GroupName);
                await _authManager.AddStatCount(StatType.ExamTmGroupAdd);

            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
