using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace DemoAspireApp.AppHost.CustomCommands.SqlServer;

internal static class AddTasksExtensions
{
    public static IResourceBuilder<SqlServerServerResource> WithAddTasksCommand(
        this IResourceBuilder<SqlServerServerResource> builder)
    {
        CommandOptions commandOptions = new()
        {
            IconName = "Add",
            IconVariant = IconVariant.Filled,
            UpdateState = OnUpdateResourceState,
            ConfirmationMessage = "Are you sure you want to add tasks to the database?",
            Description = "Adds new tasks to the SQL Server database.",
            IsHighlighted = true
        };

        builder.WithCommand(
            name: "addTasks",
            displayName: "Add tasks",
            executeCommand: context => OnRunAddTasksCommandAsync(builder, context),
            commandOptions);

        return builder;
    }

    private static async Task<ExecuteCommandResult> OnRunAddTasksCommandAsync(
        IResourceBuilder<SqlServerServerResource> builder,
        ExecuteCommandContext context)
    {
        var interactionService = context.Services.GetRequiredService<IInteractionService>();

        InteractionResult<InteractionInput> newTaskInput = await interactionService.PromptInputAsync(
            "New task",
            "Please enter values for the new task",
            "Description",
            "Description",
            cancellationToken: context.CancellationToken);

        if (newTaskInput.Canceled)
        {
            return CommandResults.Failure("Task addition was canceled by the user.");
        }

        var sqlServerConnectionString = await builder.Resource.GetConnectionStringAsync();

        try
        {
            using var connection = new SqlConnection(sqlServerConnectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand($"INSERT INTO [MyDemoDatabase].[dbo].[Tasks] (Description, Deadline) VALUES ('{newTaskInput.Data.Value}', GETDATE())", connection);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            return CommandResults.Failure(
                $"Failed to add tasks to the database: {ex.Message}");
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
