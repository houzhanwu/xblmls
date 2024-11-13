using Datory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Core.Repositories
{
    public partial class ExamQuestionnaireUserRepository : IExamQuestionnaireUserRepository
    {
        private readonly Repository<ExamQuestionnaireUser> _repository;
        private readonly string _cacheKey;

        public ExamQuestionnaireUserRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ExamQuestionnaireUser>(settingsManager.Database, settingsManager.Redis);
            _cacheKey = CacheUtils.GetEntityKey(TableName);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;


        public async Task<int> InsertAsync(ExamQuestionnaireUser item)
        {
            return await _repository.InsertAsync(item);
        }
        public async Task<bool> UpdateAsync(ExamQuestionnaireUser item)
        {
            return await _repository.UpdateAsync(item);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task ClearByUserAsync(int userId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamQuestionnaireUser.UserId), userId));
        }
        public async Task ClearByPaperAsync(int paperId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId));
        }

        public async Task ClearByPaperAndUserAsync(int paperId, int userId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId).Where(nameof(ExamQuestionnaireUser.UserId), userId));
        }
        public async Task<bool> ExistsAsync(int paperId, int userId)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId).Where(nameof(ExamQuestionnaireUser.UserId), userId));
        }

        public async Task<(int total, List<ExamQuestionnaireUser> list)> GetListAsync(int userId, string keyWords, int pageIndex, int pageSize)
        {
            var query = Q.
                WhereNullOrFalse(nameof(ExamQuestionnaireUser.Locked)).
                Where(nameof(ExamQuestionnaireUser.UserId), userId);

            if (!string.IsNullOrWhiteSpace(keyWords))
            {
                keyWords = $"%{keyWords}%";
                query.WhereLike(nameof(ExamQuestionnaireUser.KeyWords), keyWords);
            }
            query.OrderByDesc(nameof(ExamQuestionnaireUser.Id));

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }

        public async Task<int> GetSubmitUserCountAsync(int planId, int courseId, int paperId)
        {
            var query = Q.
                Where(nameof(ExamQuestionnaireUser.SubmitType), SubmitType.Submit.GetValue()).
                Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId).
                Where(nameof(ExamQuestionnaireUser.PlanId), planId).
                Where(nameof(ExamQuestionnaireUser.CourseId), courseId);

            var total = await _repository.CountAsync(query);
            return total;
        }

        public async Task<(int total, List<ExamQuestionnaireUser> list)> GetListAsync(int paperId, string isSubmit, string keyWords, int pageIndex, int pageSize)
        {
            var query = Q.
                WhereNullOrFalse(nameof(ExamQuestionnaireUser.Locked)).
                Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId);

            if (!string.IsNullOrWhiteSpace(isSubmit))
            {
                if (isSubmit == "1")
                {
                    query.Where(nameof(ExamQuestionnaireUser.SubmitType), SubmitType.Submit.GetValue());
                }
                if (isSubmit == "0")
                {
                    query.WhereNot(nameof(ExamQuestionnaireUser.SubmitType), SubmitType.Submit.GetValue());
                }
            }
            if (!string.IsNullOrWhiteSpace(keyWords))
            {
                keyWords = $"%{keyWords}%";
                query.WhereLike(nameof(ExamQuestionnaireUser.KeyWordsAdmin), keyWords);
            }
            query.OrderByDesc(nameof(ExamQuestionnaireUser.Id));

            var total = await _repository.CountAsync(query);
            var list = await _repository.GetAllAsync(query.ForPage(pageIndex, pageSize));
            return (total, list);
        }
        public async Task<ExamQuestionnaireUser> GetAsync(int paperId, int userId, int planId, int courseId)
        {
            return await _repository.GetAsync(Q.
                Where(nameof(ExamQuestionnaireUser.PlanId), planId).
                Where(nameof(ExamQuestionnaireUser.CourseId), courseId).
                Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId).
                Where(nameof(ExamQuestionnaireUser.UserId), userId));
        }
        public async Task<ExamQuestionnaireUser> GetAsync(int paperId, int userId)
        {
            return await _repository.GetAsync(Q.
                Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId).
                Where(nameof(ExamQuestionnaireUser.UserId), userId));
        }
        public async Task<ExamQuestionnaireUser> GetOnlyOneAsync(int userId)
        {
            var query = Q.
                Where(nameof(ExamQuestionnaireUser.UserId), userId);

            return await _repository.GetAsync(query.
                OrderByDesc(nameof(ExamQuestionnaireUser.Id)).
                Limit(1));
        }


        public async Task<List<int>> GetPaperIdsAsync(int userId)
        {
            var query = Q.
                Select(nameof(ExamQuestionnaireUser.ExamPaperId)).
                Where(nameof(ExamQuestionnaireUser.UserId), userId).
                WhereNullOrFalse(nameof(ExamQuestionnaireUser.Locked)).
                Where(nameof(ExamQuestionnaireUser.ExamEndDateTime), ">", DateTime.Now).
                Where(nameof(ExamQuestionnaireUser.SubmitType), SubmitType.Save.GetValue());

            return await _repository.GetAllAsync<int>(query);
        }

        public async Task UpdateLockedAsync(int paperId, bool locked)
        {
            await _repository.UpdateAsync(Q.
                Set(nameof(ExamQuestionnaireUser.Locked), locked).
                Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId));
        }
        public async Task UpdateKeyWordsAsync(int paperId, string keyWords)
        {
            await _repository.UpdateAsync(Q.
                Set(nameof(ExamQuestionnaireUser.KeyWords), keyWords).
                Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId));
        }
        public async Task UpdateKeyWordsAdminAsync(int id, string keyWords)
        {
            await _repository.UpdateAsync(Q.
                Set(nameof(ExamQuestionnaireUser.KeyWordsAdmin), keyWords).
                Where(nameof(ExamQuestionnaireUser.Id), id));
        }
        public async Task UpdateExamDateTimeAsync(int paperId, DateTime beginDateTime, DateTime endDateTime)
        {
            await _repository.UpdateAsync(Q.
                Set(nameof(ExamQuestionnaireUser.ExamBeginDateTime), beginDateTime).
                Set(nameof(ExamQuestionnaireUser.ExamEndDateTime), endDateTime).
                Where(nameof(ExamQuestionnaireUser.ExamPaperId), paperId));
        }
    }
}
