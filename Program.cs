using Microsoft.Extensions.Options;
using WebTODO.Models;

namespace WebTODO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<TodoItem> todoList = [
                new() { Title = "Vacuum", Description = "Vacuum my room", Date = new DateOnly(2024,3,10)},
                new() { Title = "Dishes", Description = "Wash dishes", Date = new DateOnly(2024,3,7)},
                new() { Title = "Chores", Description = "Do chores", Date = new DateOnly(2024,3,9)},
                ];

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Get all items
            app.MapGet("/list-get", () => Results.Json(todoList));

            // Edit item in the list
            app.MapPut("/list-edit/{id:int}", (HttpContext ctx, int id) => {
                if (ctx.Request.HasFormContentType)
                {
                    TodoItem? itemToUpdate = todoList.Find(t => t.Id == id);

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
            app.MapPost("/list-add", (HttpContext ctx) =>
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

                        todoList.Add(newItem);

                        return Results.Json(new { result = true, newItem.Id });
                    }
                }
                return Results.Json(new { result = false });
            });
            

            // Remove item from the list
            app.MapDelete("/list-remove/{id:int}", (int id) => {
                TodoItem? itemToRemove = todoList.Find(t => t.Id == id);
                if (itemToRemove != null)
                {
                    todoList.Remove(itemToRemove);
                    Results.Json(new { remove = "ok", id });
                }
            });

            app.Run();
        }
    }
}
