﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Dto;
using XBLMS.Models;
using XBLMS.Utils;
using XBLMS.Core.Utils;
using System.Collections.Generic;
using XBLMS.Enums;

namespace XBLMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsRoleAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> InsertRole([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Add))
            {
                return this.NoAuth();
            }

            var auth = await _authManager.GetAuthorityAuth();


            var role = new Role
            {
                RoleName = request.RoleName,
                MenuIds = new List<string>(),
                PermissionIds = new List<string>(),
                SelectIds = request.SelectIds,
                Description = request.Description,
                CompanyId = auth.CompanyId,
                DepartmentId = auth.DepartmentId,
                CreatorId = auth.AdminId
            };
            if (request.Menus != null && request.Menus.Count > 0)
            {
                foreach (var item in request.Menus)
                {
                    if (!item.IsPermission)
                    {
                        role.MenuIds.Add(item.Id);
                    }
                    else
                    {
                        role.PermissionIds.Add(item.Id);
                    }
                }
            }
            await _roleRepository.InsertRoleAsync(role);

            _cacheManager.Clear();

            await _authManager.AddAdminLogAsync("新增管理员角色", $"角色名称:{request.RoleName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
