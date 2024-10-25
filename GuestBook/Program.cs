using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using GuestBookApp.Data;
using System;
using GuestBookApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GuestBookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRepository<Message>, MessageRepository>(); 

// Configure session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".GuestBook.Session"; 
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); 
app.UseStaticFiles(); 

app.UseRouting(); 

app.UseAuthorization(); 
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GuestBook}/{action=Index}/{id?}");

app.Run();
