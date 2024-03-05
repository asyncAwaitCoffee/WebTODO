using Microsoft.Extensions.Options;
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

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Get all items
            app.MapGet("/list-get", (IRepository repository) => Results.Json(repository.TodoList));

            // Edit item in the list
            app.MapPut("/list-edit/{id:int}", (HttpContext ctx, IRepository repository, int id) => {
                if (ctx.Request.HasFormContentType)
                {
                    TodoItem? itemToUpdate = repository.GetItem(id);

                    if (itemToUpdate != null)
                    {
                        itemToUpdate.Title = ctx.Request.Form["title"];
                        itemToUpdate.Description = ctx.Request.Form["description"];
                        if (DateOnly.TryParse(ctx.Request.Form["date"], out DateOnly date))
                        {
                            itemToUpdate.Date = date;
                        }

                        return Results.Json(new { result = true, id });
                    }
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
