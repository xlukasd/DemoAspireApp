using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DemoAspireApp.AppHost.CustomCommands.SqlServer;

internal static class ClearTasksExtensions
{
    public static IResourceBuilder<SqlServerServerResource> WithClearTasksCommand(
        this IResourceBuilder<SqlServerServerResource> builder)
    {
        CommandOptions commandOptions = new()
        {
            IconName = "Broom",
            IconVariant = IconVariant.Filled,
            UpdateState = OnUpdateResourceState,
            ConfirmationMessage = "Are you sure you want to clear the Tasks table?",
            Description = "Clears all tasks from the SQL Server database. This action cannot be undone.",
            IsHighlighted = true
        };

        builder.WithCommand(
            name: "clearTasks",
            displayName: "Clear tasks",
            executeCommand: context => OnRunClearTasksCommandAsync(builder),
            commandOptions);

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnRunClearTasksCommandAsync(
        IResourceBuilder<SqlServerServerResource> builder)
    {
        var sqlServerConnectionString = await builder.Resource.GetConnectionStringAsync();

        try
        {
            using var connection = new SqlConnection(sqlServerConnectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("DELETE FROM [MyDemoDatabase].[dbo].[Tasks]", connection);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            return CommandResults.Failure(
                $"Failed to clear tasks from the database: {ex.Message}");
        }

        return CommandResults.Success();
    }

    private static ResourceCommandState OnUpdateResourceState(
        UpdateCommandStateContext context)
    {
        var logger = context.Services.GetRequiredService<ILogger<Program>>();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Updating resource state: {ResourceSnapshot}",
                context.ResourceSnapshot);
        }

        return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;
    }
}
