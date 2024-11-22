
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using SnaptagOwnKioskInternalBackend.DBContexts;
using SnaptagOwnKioskInternalBackend.Printer;
using SnaptagOwnKioskInternalBackend.Services;
using SnaptagOwnKioskInternalBackend.Setting;

namespace SnaptagOwnKioskInternalBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .Enrich.WithExceptionDetails()
              .WriteTo.File(
                  path: Path.Combine("Logs", DateTime.Now.ToString("yyyyMMdd"), "log.txt"),
                  rollingInterval: RollingInterval.Infinite
              )
              .CreateLogger();
            try
            {
                Log.Information("Started");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.WebHost.UseUrls("http://127.0.0.1:8798");
                builder.Services.AddDbContext<SnaptagKioskDBContext>(options => options.UseSqlite("Data Source=SnaptagKiosk.db").LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name },
                   LogLevel.Information).EnableSensitiveDataLogging());
                builder.Services.AddScoped<PaymentService>();
                builder.Services.AddScoped<PrinterService>();
                builder.Services.AddHttpClient<PaymentService>();
                var app = builder.Build();

                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseAuthorization();


                app.MapControllers();
                LUCASPrinter print = new LUCASPrinter();
                print.InitPrinter();
                var res = print.GetPrinerStatus();
                ProgramSettingManager manager = ProgramSettingManager.Instance;
                
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected Error occured, msg = {ex.Message},stacktrace= {ex.StackTrace}");
            }
        }
    }
}
