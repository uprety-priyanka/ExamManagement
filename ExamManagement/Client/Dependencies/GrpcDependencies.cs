using Grpc.Net.Client.Web;
using Grpc.Protos;

namespace ExamManagement.Client.Dependencies
{
    public static class GrpcDependencies
    {
        public static IServiceCollection RegisterAllGrpcDependencies(this IServiceCollection services, string BaseAddress) 
        {

            services.AddGrpcClient<AccountGrpcService.AccountGrpcServiceClient>(options =>
            {
                options.Address = new Uri(BaseAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(
                () => new GrpcWebHandler(new HttpClientHandler()));

            services.AddGrpcClient<CourseGrpcService.CourseGrpcServiceClient>(options =>
            {
                options.Address = new Uri(BaseAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(
                () => new GrpcWebHandler(new HttpClientHandler()));

            services.AddGrpcClient<FacultyGrpcService.FacultyGrpcServiceClient>(options =>
            {
                options.Address = new Uri(BaseAddress);
            })
            .ConfigurePrimaryHttpMessageHandler(
                () => new GrpcWebHandler(new HttpClientHandler()));

            return services;
        }
    }
}
