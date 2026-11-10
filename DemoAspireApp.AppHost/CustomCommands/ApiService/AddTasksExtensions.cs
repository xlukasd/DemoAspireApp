using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace DemoAspireApp.AppHost.CustomCommands.ApiService;

internal static class AddTasksExtensions
{
    public static IResourceBuilder<ProjectResource> WithAddTasksHttpCommand(
        this IResourceBuilder<ProjectResource> builder)
    {
        //TODO Place your code here

        return builder;
    }
}
