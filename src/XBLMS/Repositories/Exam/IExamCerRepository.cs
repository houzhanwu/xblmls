using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IExamCerRepository : IRepository
    {
        Task<bool> IsExistsAsync(string name);

        Task<ExamCer> GetAsync(int id);
        Task<List<ExamCer>> GetListAsync(AuthorityAuth auth);
        Task<List<ExamCer>> GetListAsync();

        Task<int> InsertAsync(ExamCer item);

        Task UpdateAsync(ExamCer item);

        Task DeleteAsync(int id);

    }
}
