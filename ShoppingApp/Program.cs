using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
/*builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.CommandTimeout(180);
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        }
    ));*/
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
    sqlOptions =>
    {
        sqlOptions.CommandTimeout(180);
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        );
    }));

//builder.Services.AddHostedService<KeepAliveService>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddUserManager<UserManager<ApplicationUser>>();

builder.Services.AddControllersWithViews();



builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
    });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();


var app = builder.Build();

// For gracefull fallback when database is warming up
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (SqlException ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database warming up after idle...");

        context.Response.StatusCode = 503; // Service unavailable
        await context.Response.WriteAsync(@"
            <html><head><meta http-equiv='refresh' content='10'></head>
            <body style='font-family:sans-serif;text-align:center;margin-top:20vh;'>
                <h2> Waking up the database...</h2>
                <p>Please wait a few seconds and refresh the page.</p>
            </body></html>");
    }
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    const int maxRetries = 5;
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            SeedData.Initialize(context);
            break;
        }
        catch (SqlException)
        {
            Console.WriteLine("SqlException caught on retry " + i + 1);
            await Task.Delay(TimeSpan.FromSeconds(10));
        }
    }
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();



app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
