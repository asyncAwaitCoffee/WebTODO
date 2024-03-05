using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebTODO.Models;
using WebTODO.Repositories;

namespace WebTODO
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IRepository, MemRepository>();
            builder.Services.AddSingleton<IUsersRepository, MemUsersRepository>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(options => options.LoginPath = "/sign");
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", async (HttpContext ctx) =>
            {
                ctx.Response.ContentType = "text/html";
                await ctx.Response.SendFileAsync("html/home.html");
            });

            app.MapGet("/list", [Authorize] async (HttpContext ctx) =>
            {
                ctx.Response.ContentType = "text/html";
                await ctx.Response.SendFileAsync("html/list.html");
            });

            app.MapGet("/sign", async (HttpContext ctx) =>
            {
                ctx.Response.ContentType = "text/html";
                await ctx.Response.SendFileAsync("html/signin.html");
            });

            app.MapPost("/signin", async (HttpContext ctx, IUsersRepository usersRepo) =>
            {
                if (ctx.Request.HasFormContentType)
                {
                    string? name = ctx.Request.Form["name"];
                    string? password = ctx.Request.Form["password"];

                    if (name is not null && password is not null)
                    {
                        User? user = usersRepo.GetUser(name, password);

                        if (user != null)
                        {
                            List<Claim> claims = [new Claim(ClaimTypes.NameIdentifier, name)];
                            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await ctx.SignInAsync(claimsPrincipal);
                        }
                    }
                }
                ctx.Response.ContentType = "text/html";
                await ctx.Response.SendFileAsync("html/signin.html");
            });

            // Get all items
            app.MapGet("/list-get", (IRepository repository) => Results.Json(repository.TodoList));

            // Edit item in the list
            app.MapPut("/list-edit/{id:int}", (HttpContext ctx, IRepository repository, int id) => {
                if (ctx.Request.HasFormContentType)
                {
                    repository.UpdateItem(id, ctx.Request.Form);
                    return Results.Json(new { result = true, id });
                }

                return Results.Json(new { result = false, id });
            });

            // Add new item to the list
            app.MapPost("/list-add", (HttpContext ctx, IRepository repository) =>
            {
                if (ctx.Request.HasFormContentType)
                {
                    if (DateOnly.TryParse(ctx.Request.Form["date"], out DateOnly date))
                    {
                        TodoItem newItem = new()
                        {
                            Title = ctx.Request.Form["title"],
                            Description = ctx.Request.Form["description"],
                            Date = date
                        };

                        repository.AddItem(newItem);

                        return Results.Json(new { result = true, newItem.Id });
                    }
                }
                return Results.Json(new { result = false });
            });
            

            // Remove item from the list
            app.MapDelete("/list-remove/{id:int}", (IRepository repository, int id) => {
                TodoItem? itemToRemove = repository.GetItem(id);
                if (itemToRemove != null)
                {
                    repository.RemoveItem(itemToRemove);
                    Results.Json(new { remove = "ok", id });
                }
            });

            app.Run();
        }
    }
}
