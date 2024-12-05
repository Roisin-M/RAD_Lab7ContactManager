using ContactManager.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//add this using to set fallback Policy for authorization
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using ContactManager.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()// add role services to identity
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

//set the fallback authorisation policy that requires users to be authenticated
//Is applied to all requests that don't explicitly specify an authorization policy.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()//adds  DenyAnonymousAuthorizationRequirement to the current
                               //instance, which enforces that the current user is authenticated.
    .Build();
});
/* requires all users to be authenticated, except for Razor Pages, controllers, 
 * or action methods with an authorization attribute. For example, Razor Pages, 
 * controllers, or action methods with [AllowAnonymous] or 
 * [Authorize(PolicyName="MyPolicy")] use the applied authorization attribute 
 * rather than the fallback authorization policy.
 * ^^^
 */

//an alternative way for MVC controllers and Razor Pages to require all users to be authenticated
//is by adding an authorization filter

//builder.Services.AddControllers(config =>
//{
//    //The preceding code uses an authorization filter, setting the fallback policy uses
//    //endpoint routing. Setting the fallback policy is the preferred way to require all
//    //users be authenticated.
//    var policy = new AuthorizationPolicyBuilder()
//    .RequireAuthenticatedUser()
//    .Build();
//    config.Filters.Add(new AuthorizeFilter(policy));
//});

// Authorization handlers.
builder.Services.AddScoped<IAuthorizationHandler,
                      ContactIsOwnerAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      ContactAdministratorsAuthorizationHandler>();

builder.Services.AddSingleton<IAuthorizationHandler,
                      ContactManagerAuthorizationHandler>();

var app = builder.Build();

//seed database initialiser
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    //add in to test password
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
    // requires using Microsoft.Extensions.Configuration;
    // Set password with the Secret Manager tool.
    // dotnet user-secrets set SeedUserPW <pw>
    var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");

    await SeedData.Initialize(services, testUserPw);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
