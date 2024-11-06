﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using XBLMS.Dto;
using XBLMS.Models;

namespace XBLMS.Repositories
{
    public interface IStudyCourseEvaluationRepository : IRepository
    {
        Task<int> InsertAsync(StudyCourseEvaluation item);
        Task<bool> UpdateAsync(StudyCourseEvaluation item);
        Task<bool> DeleteAsync(int id);
        Task<StudyCourseEvaluation> GetAsync(int id);
        Task<int> MaxAsync();
        Task<(int total, List<StudyCourseEvaluation> list)> GetListAsync(string keyWords, int pageIndex, int pageSize);
    }
}
