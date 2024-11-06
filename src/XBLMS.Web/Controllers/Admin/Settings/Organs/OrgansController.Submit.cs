﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Dto;
using XBLMS.Core.Utils;
using XBLMS.Models;
using NPOI.Util;
using ICSharpCode.SharpZipLib.Core;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using XBLMS.Utils;
using XBLMS.Enums;
using System.Collections;
using System.ComponentModel.Design;

namespace XBLMS.Web.Controllers.Admin.Settings.Organs
{
    public partial class OrgansController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] GetSubmitRequest request)
        {
            if (request.Id > 0)
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Add))
                {
                    return this.NoAuth();
                }
            }
            else
            {
                if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Update))
                {
                    return this.NoAuth();
                }
            }
            var auth = await _authManager.GetAuthorityAuth();

            if (request.Type == "company")
            {
                if (request.Id > 0)
                {
                    var company = await _companyRepository.GetAsync(request.Id);
                    var oldName = company.Name;
                    company.Name = request.Name;
                    if (!StringUtils.Equals(oldName, request.Name))
                    {
                        await _companyRepository.UpdateAsync(company);
                        await _authManager.AddAdminLogAsync("修改单位", $"修改前:{oldName},修改后:{company.Name}");
                    }
                }
                else
                {
                    await AddOrgans(request.Name, request.ParentId, request.Type, request.ParentType, auth.AdminId);
                }
            }
            if (request.Type == "department")
            {
                if (request.Id > 0)
                {
                    var department = await _organDepartmentRepository.GetAsync(request.Id);
                    var oldName = department.Name;
                    department.Name = request.Name;
                    if (!StringUtils.Equals(oldName, request.Name))
                    {
                        await _organDepartmentRepository.UpdateAsync(department);
                        await _authManager.AddAdminLogAsync("修改部门", $"修改前:{oldName},修改后:{department.Name}");
                    }
                }
                else
                {
                    await AddOrgans(request.Name, request.ParentId, request.Type, request.ParentType, auth.AdminId);
                }
            }
            if (request.Type == "duty")
            {
                if (request.Id > 0)
                {
                    var duty = await _organDutyRepository.GetAsync(request.Id);
                    var oldName = duty.Name;
                    duty.Name = request.Name;
                    if (!StringUtils.Equals(oldName, request.Name))
                    {
                        await _organDutyRepository.UpdateAsync(duty);
                        await _authManager.AddAdminLogAsync("修改岗位", $"修改前:{oldName},修改后:{duty.Name}");
                    }
                }
                else
                {
                    await AddOrgans(request.Name, request.ParentId, request.Type, request.ParentType, auth.AdminId);
                }
            }
            return new BoolResult
            {
                Value = true
            };
        }

        private async Task AddOrgans(string organNames, int organParentId, string type, string parentType, int adminId)
        {
            var companyId = 0;
            var departmentId = 0;

            var insertedTreeIdHashtable = new Hashtable { [1] = organParentId };

            if (type == "department")
            {
                if (parentType == "company")
                {
                    companyId = organParentId;
                    insertedTreeIdHashtable = new Hashtable { [1] = 0 };
                }
                if (parentType == "department")
                {
                    var findDepartment = await _organDepartmentRepository.GetAsync(organParentId);
                    companyId = findDepartment.CompanyId;
                }
            }
            if (type == "duty")
            {
                if (parentType == "department")
                {
                    var findDepartment = await _organDepartmentRepository.GetAsync(organParentId);
                    companyId = findDepartment.CompanyId;
                    departmentId = findDepartment.Id;
                    insertedTreeIdHashtable = new Hashtable { [1] = 0 };
                }
                if (parentType == "duty")
                {
                    var findDuty = await _organDutyRepository.GetAsync(organParentId);
                    companyId = findDuty.CompanyId;
                    departmentId = findDuty.DepartmentId;
                }
            }

            var names = organNames.Split('\n');
            foreach (var item in names)
            {
                if (string.IsNullOrEmpty(item)) continue;

                var count = StringUtils.GetStartCount('－', item) == 0 ? StringUtils.GetStartCount('-', item) : StringUtils.GetStartCount('－', item);
                var name = item.Substring(count, item.Length - count);
                count++;

                if (!string.IsNullOrEmpty(name) && insertedTreeIdHashtable.Contains(count))
                {
                    if (name.Contains('(') && name.Contains(')'))
                    {
                        var length = name.IndexOf(')') - name.IndexOf('(');
                        if (length > 0)
                        {
                            name = name.Substring(0, name.IndexOf('('));
                        }
                    }
                    name = name.Trim();

                    var parentId = (int)insertedTreeIdHashtable[count];

                    var insertId = 0;
                    if (type == "company")
                    {
                        var company = new OrganCompany()
                        {
                            Name = name,
                            ParentId = parentId,
                            CreatorId = adminId
                        };
                        insertId = await _companyRepository.InsertAsync(company);
                        company.CompanyId = insertId;
                        await _companyRepository.UpdateAsync(company);

                        var adminUserName = $"admin_{StringUtils.PadZeroes(insertId, 8)}";
                        await _administratorRepository.InsertAsync(new Administrator
                        {
                            UserName = adminUserName,
                            DisplayName = $"单位管理员_{name}",
                            CompanyId = insertId,
                            Auth = AuthorityType.AdminCompany,
                            AuthCurManageOrganId = insertId,
                            CreatorId = adminId,
                        }, adminUserName);

                        await _authManager.AddAdminLogAsync("新增单位", $"{name}");
                    }
                    if (type == "department")
                    {
                        var department = new OrganDepartment()
                        {
                            ParentId = parentId,
                            Name = name,
                            CompanyId = companyId,
                            DepartmentId = 0,
                            CreatorId = adminId
                        };

                        insertId = await _organDepartmentRepository.InsertAsync(department);
                        department.DepartmentId = insertId;
                        await _organDepartmentRepository.UpdateAsync(department);

                        await _authManager.AddAdminLogAsync("新增部门", name);
                    }
                    if (type == "duty")
                    {
                        var duty = new OrganDuty()
                        {
                            ParentId = parentId,
                            Name = name,
                            CompanyId = companyId,
                            DepartmentId = departmentId,
                            CreatorId = adminId
                        };
                        insertId = await _organDutyRepository.InsertAsync(duty);
                        await _authManager.AddAdminLogAsync("新增岗位", name);
                    }

                    insertedTreeIdHashtable[count + 1] = insertId;
                }
            }
        }


    }
}
