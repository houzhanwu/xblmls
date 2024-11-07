﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XBLMS.Core.Utils;
using XBLMS.Enums;
using XBLMS.Models;
using XBLMS.Services;

namespace XBLMS.Core.Services
{
    public partial class StudyManager
    {
        public async Task User_GetPlanInfo(StudyPlanUser planUser, bool isDetail = false)
        {
            var studyPlan = await _studyPlanRepository.GetAsync(planUser.PlanId);
            studyPlan.Set("PlanBeginDateTimeStr", studyPlan.PlanBeginDateTime.Value.ToString(DateUtils.FormatStringDateOnlyCN));
            studyPlan.Set("PlanEndDateTimeStr", studyPlan.PlanEndDateTime.Value.ToString(DateUtils.FormatStringDateOnlyCN));

            var planCourseTotal = await _studyPlanCourseRepository.CountAsync(studyPlan.Id, false);
            var planSelectCourseTotal = await _studyPlanCourseRepository.CountAsync(studyPlan.Id, false);

            studyPlan.Set("CourseTotal", planCourseTotal);
            studyPlan.Set("SelectCourseTotal", planSelectCourseTotal);


            var overCourseTotal = 0;
            var overSelectCourseTotal = 0;
            long overSelectCourseDurationTotal = 0;

            var courseList = new List<StudyCourse>();
            var courseSelectList = new List<StudyCourse>();

            var planUserCourseList = await _studyCourseUserRepository.GetListAsync(studyPlan.Id, planUser.UserId);

            if (planUserCourseList != null && planUserCourseList.Count > 0)
            {
                foreach (var planUserCourse in planUserCourseList)
                {

                    var planCourse = await _studyPlanCourseRepository.GetAsync(studyPlan.Id, planUserCourse.CourseId);

                    if (planUserCourse.State == StudyStatType.Yiwancheng || planUserCourse.State == StudyStatType.Yidabiao)
                    {
                        if (planCourse.IsSelectCourse)
                        {
                            overSelectCourseTotal++;
                        }
                        else
                        {
                            overCourseTotal++;
                        }
                    }
                    if (planCourse.IsSelectCourse)
                    {
                        overSelectCourseDurationTotal += planUserCourse.TotalDuration;
                    }

                }
            }

            if (isDetail)
            {
                var planCourseList = await _studyPlanCourseRepository.GetListAsync(false, studyPlan.Id);
                if (planCourseList != null && planCourseList.Count > 0)
                {
                    foreach (var planCourse in planCourseList)
                    {
                        var course = await _studyCourseRepository.GetAsync(planCourse.CourseId);
                        var courseUser = await _studyCourseUserRepository.GetAsync(planUser.UserId, planUser.PlanId, planCourse.CourseId);
                        await User_GetCourseInfoByCourseList(studyPlan.Id, course, courseUser);
                        course.Set("CourseType", "必修课");
                        courseList.Add(course);
                    }
                }
                var planSelectCourseList = await _studyPlanCourseRepository.GetListAsync(true, studyPlan.Id);
                if (planSelectCourseList != null && planSelectCourseList.Count > 0)
                {
                    foreach (var planCourse in planSelectCourseList)
                    {
                        var course = await _studyCourseRepository.GetAsync(planCourse.CourseId);
                        var courseUser = await _studyCourseUserRepository.GetAsync(planUser.UserId, planUser.PlanId, planCourse.CourseId);
                        await User_GetCourseInfoByCourseList(studyPlan.Id, course, courseUser);
                        course.Set("CourseType", "选修课");
                        courseSelectList.Add(course);
                    }
                }
            }


            if (overCourseTotal == planCourseTotal)
            {
                planUser.State = StudyStatType.Yiwancheng;
            }
            if (planUser.TotalCredit >= studyPlan.PlanCredit)
            {
                planUser.State = StudyStatType.Yidabiao;
            }

            planUser.Set("OverCourseTotal", overCourseTotal);
            planUser.Set("OverSelectCourseTotal", overSelectCourseTotal);
            planUser.Set("OverSelectCourseDurationTotal", overSelectCourseDurationTotal);
            planUser.Set("CourseList", courseList);
            planUser.Set("CourseSelectList", courseSelectList);

            var isStudy = true;
            if (studyPlan.PlanBeginDateTime.Value > DateTime.Now || studyPlan.PlanEndDateTime.Value < DateTime.Now)
            {
                isStudy = false;
            }
            planUser.Set("IsStudy", isStudy);
            planUser.Set("Plan", studyPlan);
        }
    }
}
