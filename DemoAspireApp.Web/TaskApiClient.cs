namespace DemoAspireApp.Web;

public class TaskApiClient(HttpClient httpClient)
{
    public async Task<TodoTask[]> GetTasksAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<TodoTask>? tasks = null;

        await foreach (var task in httpClient.GetFromJsonAsAsyncEnumerable<TodoTask>("/tasks", cancellationToken))
        {
            if (tasks?.Count >= maxItems)
            {
                break;
            }
            if (task is not null)
            {
                tasks ??= [];
                tasks.Add(task);
            }
        }

        return tasks?.ToArray() ?? [];
    }
}

public record TodoTask(string Description, DateOnly Deadline);
