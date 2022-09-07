﻿using ExamManagement.Server.Services.Abstraction;
using ExamManagement.Server.Services.Implementation;

namespace ExamManagement.Server.Dependencies
{
    public static class GrpcDependencies
    {
        public static IServiceCollection RegisterGrpcDependencies(this IServiceCollection services) 
        {

            services.AddHttpContextAccessor();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IFacultyService, FacultyService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IExamFormService, ExamFormService>();

            return services;
        }
    }
}
