using ERPClinic.Components;
using ERPClinic.Components.Classes;

using Microsoft.EntityFrameworkCore;

namespace ERPClinic;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("cloudConnection")));


        // Register IHttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        // If it should be one instance per HTTP request
        builder.Services.AddScoped<ClientsMDBClass>();
        builder.Services.AddScoped<CalendarMDBClass>();
        builder.Services.AddScoped<AppointmentMDBClass>();
        builder.Services.AddScoped<ExpensesMDBClass>();
        builder.Services.AddScoped<DBSelection>();


        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapControllers();

        app.Run();
    }
}
