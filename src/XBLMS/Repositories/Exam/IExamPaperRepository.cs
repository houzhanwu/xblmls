using Datory;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public partial interface IExamPaperRepository : IRepository
    {
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountAsync(List<int> treeIds);
        Task<ExamPaper> GetAsync(int id);
        Task<int> InsertAsync(ExamPaper item);
        Task<bool> UpdateAsync(ExamPaper item);
        Task<(int total, List<ExamPaper> list)> GetListAsync(AuthorityAuth auth, List<int> treeIds, string keyword, int pageIndex, int pageSize);
        Task<bool> DeleteAsync(int Id);
        Task<int> MaxAsync();
        Task<List<int>> GetIdsAsync(List<int> ids, string keyword);
        Task<(int allCount, int addCount, int deleteCount, int lockedCount, int unLockedCount)> GetDataCount(AuthorityAuth auth);
    }
}
