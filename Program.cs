using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using API.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowPortFromAngulrClient",
        builder => builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddSignalR();
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowPortFromAngulrClient");
app.UseAuthentication(); 
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior",true);
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<PresenceHub>("hubs/presence");
    endpoints.MapHub<MessageHub>("hubs/message");
    endpoints.MapFallbackToController("Index","Fallback");
});
AppDbInitialer.SeedData(app);
app.Run();
