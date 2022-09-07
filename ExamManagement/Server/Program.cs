using ExamManagement.Server.Data;
using ExamManagement.Server.Dependencies;
using ExamManagement.Server.GrpcPipelines;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContextPool<ApplicationContext>((o, x) =>
{
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    x.UseApplicationServiceProvider(o);
});

builder.Services.AddGrpc(config =>
{
    config.MaxReceiveMessageSize = 25 * 1024 * 1024; // 25 MB
    config.MaxSendMessageSize = 25 * 1024 * 1024; // 25 MB
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
{
    config.SignIn.RequireConfirmedAccount = false;
    config.SignIn.RequireConfirmedEmail = false;
    config.SignIn.RequireConfirmedPhoneNumber = false;

    config.User.RequireUniqueEmail = true;

    config.Lockout.AllowedForNewUsers = false;

    config.Password.RequireUppercase = true;
    config.Password.RequireLowercase = true;
    config.Password.RequiredUniqueChars = 0;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequiredLength = 8;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationContext>();


builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "EM";
    config.Cookie.MaxAge = System.TimeSpan.MaxValue;
    config.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
});

builder.Services.RegisterGrpcDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<AccountGrpcPipeline>().EnableGrpcWeb().RequireCors("AllowAll");
    endpoints.MapGrpcService<CourseGrpcPipeline>().EnableGrpcWeb().RequireCors("AllowAll");
    endpoints.MapGrpcService<FacultyGrpcPipeline>().EnableGrpcWeb().RequireCors("AllowAll");
    endpoints.MapGrpcService<ResultGrpcPipeline>().EnableGrpcWeb().RequireCors("AllowAll");
    endpoints.MapGrpcService<ExamFormGrpcPipeline>().EnableGrpcWeb().RequireCors("AllowAll");
    endpoints.MapRazorPages();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapFallbackToFile("index.html");
});

app.Run();
