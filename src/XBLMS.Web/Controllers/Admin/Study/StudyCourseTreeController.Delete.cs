using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XBLMS.Dto;
using XBLMS.Core.Utils;
using XBLMS.Utils;
using XBLMS.Enums;

namespace XBLMS.Web.Controllers.Admin.Study
{
    public partial class StudyCourseTreeController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] IdRequest request)
        {
            if (!await _authManager.HasPermissionsAsync(MenuPermissionType.Delete))
            {
                return this.NoAuth();
            }

            var admin = await _authManager.GetAdminAsync();

            var item = await _studyCourseTreeRepository.GetAsync(request.Id);

            if (item == null) return this.NotFound();
            var ids = await _studyCourseTreeRepository.GetIdsAsync(request.Id);
            var tmCount =0;
            if (tmCount > 0) return this.Error($"�÷������������{tmCount}���ſγ̣���ʱ������ɾ��");
            await _studyCourseTreeRepository.DeleteAsync(ids);
            await _authManager.AddAdminLogAsync("ɾ���γ̷��༰�����¼�", $"{item.Name}");
            return new BoolResult
            {
                Value = true
            };
        }
    }
}
