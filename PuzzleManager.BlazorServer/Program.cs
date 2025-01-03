using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PuzzleManager.BlazorServer.Components.Account;
using PuzzleManager.BlazorServer.Components;
using PuzzleManager.BlazorServer.Data;
using PuzzleManager.Data;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------
// 1. Add services to the container.
// ----------------------------------

// Add support for Razor Components, and also enable interactive server components.
// "RazorComponents" is the new name in .NET 8/9 for hosting Razor-based UI.
// "AddInteractiveServerComponents()" configures Blazor Server–like real-time interaction.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Adds support for a cascading authentication state
// This is used so that child components in Razor can access the authentication state
// (i.e., info about the current logged-in user).
builder.Services.AddCascadingAuthenticationState();

// IdentityUserAccessor and IdentityRedirectManager are application-specific services
// that might help manage user sessions or redirects upon login/logout.
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

// "AuthenticationStateProvider" is the Blazor mechanism for telling components who is logged in.
// "IdentityRevalidatingAuthenticationStateProvider" implements revalidation logic
// to keep user state current without reloading the entire application.
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Configures the authentication scheme. 
// By default, it uses IdentityConstants.ApplicationScheme (the cookie for signed-in users),
// and IdentityConstants.ExternalScheme for sign-in operations (like external logins).
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = IdentityConstants.ApplicationScheme;
	options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
	.AddIdentityCookies(); // Adds cookie-based authentication for ASP.NET Core Identity.

// Retrieves the connection string named "DefaultConnection" from appsettings.json (or other config).
// If it isn't found, throws an exception to prevent silent failures.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// This registers an EF Core DbContext (ApplicationDbContext) that handles Identity tables 
// (like AspNetUsers, AspNetRoles). Points to our SQL Server-based database.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

// Adds a developer page filter that gives detailed EF Core exceptions in dev mode 
// (e.g., info about migrations and database errors).
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configures IdentityCore for your custom user type (ApplicationUser).
// Requires confirmed accounts by default (email confirmation, etc.).
// Adds EF-based stores so Identity can save data in the database, sign-in manager for handling logins,
// and default token providers for email confirmations, password resets, etc.
builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

// IEmailSender is used for sending confirmation or password reset emails.
// "IdentityNoOpEmailSender" presumably does nothing (a "no-operation" stub) for local dev/testing.
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// ----------------------------------
// 2. Add puzzle data services
// ----------------------------------

// Register your puzzle data DbContext (PuzzleManagerContext) using the same or a different
// connection string, depending on whether you want them in the same database or separate.

builder.Services.AddDbContext<PuzzleManagerContext>(options =>
	options.UseSqlServer(connectionString)); // We are reusing the same connection string here, since the puzzle data is in the same database.

// ----------------------------------
// 3. Build the application
// ----------------------------------
var app = builder.Build();

// ----------------------------------
// 4. Configure the HTTP request pipeline
// ----------------------------------

// If we're in Development environment, use the endpoint that shows migration status pages.
// Otherwise, use a production error handler at "/Error" with server-side error scopes.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts(); // Adds HTTP Strict-Transport-Security to enforce HTTPS in production.
}

// Ensures all HTTP requests are redirected to HTTPS if you have it configured.
app.UseHttpsRedirection();

// Enables Anti-forgery protection for form submissions and other requests 
// to help mitigate CSRF (cross-site request forgery) attacks.
app.UseAntiforgery();

// Maps static assets (wwwroot files, etc.) so they can be served by the app.
app.MapStaticAssets();

// Maps Razor components using the root component "App" and sets the render mode to interactive server.
// This is effectively the Blazor Server-like experience (SignalR-based interaction).
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

// Adds additional endpoints for Identity, specifically for the built-in /Account Razor pages 
// (Login, Register, Logout, etc.).
app.MapAdditionalIdentityEndpoints();

// ----------------------------------
// 4. Run the application
// ----------------------------------
app.Run();
