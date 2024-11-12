﻿using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Collections.Generic;
using XBLMS.Configuration;
using XBLMS.Models;
using XBLMS.Repositories;
using XBLMS.Services;

namespace XBLMS.Web.Controllers.Admin.Study
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class StudyCourseManagerController : ControllerBase
    {
        private const string Route = "study/studyCourseManager";
        private const string RouteCourse = Route + "/course";
        private const string RouteCourseExport = RouteCourse + "/export";
        private const string RouteUser = Route + "/user";
        private const string RouteUserExport = RouteUser + "/export";
        private const string RouteScore = Route + "/socre";
        private const string RouteScoreExport = RouteScore + "/export";

        private readonly IAuthManager _authManager;
        private readonly IStudyManager _studyManager;
        private readonly IPathManager _pathManager;
        private readonly IUploadManager _uploadManager;
        private readonly IOrganManager _organManager;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IStudyPlanRepository _studyPlanRepository;
        private readonly IStudyPlanCourseRepository _studyPlanCourseRepository;
        private readonly IStudyCourseRepository _studyCourseRepository;
        private readonly IStudyCourseTreeRepository _studyCourseTreeRepository;
        private readonly IStudyCourseFilesRepository _studyCourseFilesRepository;
        private readonly IStudyCourseWareRepository _studyCourseWareRepository;
        private readonly IStudyPlanUserRepository _studyPlanUserRepository;
        private readonly IStudyCourseUserRepository _studyCourseUserRepository;
        private readonly IExamPaperStartRepository _examPaperStartRepository;
        private readonly IExamPaperRepository _examPaperRepository;
        private readonly IExamCerRepository _examCerRepository;

        public StudyCourseManagerController(IAuthManager authManager,
            IPathManager pathManager,
            IUploadManager uploadManager,
            IStudyManager studyManager,
            IUserGroupRepository userGroupRepository,
            IStudyPlanRepository studyPlanRepository,
            IStudyPlanCourseRepository studyPlanCourseRepository,
            IStudyCourseRepository studyCourseRepository,
            IStudyCourseFilesRepository studyCourseFilesRepository,
            IStudyCourseTreeRepository studyCourseTreeRepository,
            IStudyCourseWareRepository studyCourseWareRepository,
            IStudyPlanUserRepository studyPlanUserRepository,
            IStudyCourseUserRepository studyCourseUserRepository,
            IOrganManager organManager,
            IExamPaperStartRepository examPaperStartRepository,
            IExamPaperRepository examPaperRepository,
            IExamCerRepository examCerRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _uploadManager = uploadManager;
            _studyManager = studyManager;
            _userGroupRepository = userGroupRepository;
            _studyPlanRepository = studyPlanRepository;
            _studyPlanCourseRepository = studyPlanCourseRepository;
            _studyCourseRepository = studyCourseRepository;
            _studyCourseTreeRepository = studyCourseTreeRepository;
            _studyCourseFilesRepository = studyCourseFilesRepository;
            _studyCourseWareRepository = studyCourseWareRepository;
            _studyPlanUserRepository = studyPlanUserRepository;
            _studyCourseUserRepository = studyCourseUserRepository;
            _organManager = organManager;
            _examPaperStartRepository = examPaperStartRepository;
            _examPaperRepository = examPaperRepository;
            _examCerRepository = examCerRepository;
        }
        public class GetRequest
        {
            public int Id { get; set; }
            public int PlanId { get; set; }
        }
        public class GetResult
        {
            public StudyCourse Item { get; set; }
        }
        public class GetUserRequest
        {
            public int Id { get; set; }
            public string KeyWords { get; set; }
            public string State { get; set; }
            public int PageIndex { get; set; }
            public int PageSize { get; set; }

        }
        public class GetUserResult
        {
            public int Total { get; set; }
            public List<StudyPlanUser> List { get; set; }
        }
        public class GetCourseRequest
        {
            public int Id { get; set; }
            public string KeyWords { get; set; }
        }
        public class GetCourseResult
        {
            public List<StudyPlanCourse> List { get; set; }
        }


    }
}

