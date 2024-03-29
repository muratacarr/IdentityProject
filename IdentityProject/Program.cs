using IdentityProject.ClaimProviders;
using IdentityProject.Context;
using IdentityProject.Extensions;
using IdentityProject.OptionsModels;
using IdentityProject.Requirements;
using IdentityProject.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    //opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
    opt.UseSqlite(builder.Configuration.GetConnectionString("SqliteCon"));
});

builder.Services.Configure<SecurityStampValidatorOptions>(opt =>
{
    opt.ValidationInterval = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();
builder.Services.AddScoped<IAuthorizationHandler, ExchangeExpireRequirementHandler>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(nameof(EmailSettings)));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddIdentityWithExt();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DenizliPolicy", policy =>
    {
        policy.RequireClaim("city", "denizli");
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });
});

builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder();

    cookieBuilder.Name = "MuratIdentityCookie";
    opt.Cookie = cookieBuilder;
    opt.LoginPath = new PathString("/Home/SignIn");
    opt.LogoutPath = new PathString("/Member/LogOut");
    opt.ExpireTimeSpan = TimeSpan.FromDays(1);
    opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
    opt.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
