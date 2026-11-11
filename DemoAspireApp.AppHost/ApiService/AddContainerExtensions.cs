using Microsoft.Extensions.DependencyInjection;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
namespace DemoAspireApp.AppHost.ApiService
{
    internal static class AddContainerExtensions
    {
        internal static IResourceBuilder<ProjectResource> WithAddContainerCommand(this IResourceBuilder<ProjectResource> resourceBuilder)
        {
            resourceBuilder.WithHttpCommand(
                path: "/containers",
                displayName: "Add container",
                commandName: "AddContainer",
                commandOptions: new HttpCommandOptions
                {
                    Method = HttpMethod.Post,
                    Description = "Add a new container to the Azure Blob Storage account.",
                    PrepareRequest = PrepareRequestAsync,
                    IconName = "Add",
                    IsHighlighted = true
                });

            return resourceBuilder;
        }

        private static async Task PrepareRequestAsync(HttpCommandRequestContext context)
        {
            var interactionService = context.Services.GetRequiredService<IInteractionService>();

            InteractionResult<InteractionInput> newContainerInput = await interactionService.PromptInputAsync(
                "New container",
                "Please enter values for the new container",
                "Name",
                "Name",
                new InputsDialogInteractionOptions()
                {
                    ValidationCallback = (input) =>
                    {
                        var nameInput = input.Inputs.Single();

                        string? name = input.Inputs.GetString("Name");

                        if (name == null)
                        {
                            input.AddValidationError(nameInput, "Name is required.");
                            return Task.CompletedTask;
                        }

                        var allLetersLowercase = name!.All(char.IsLower);

                        if (!allLetersLowercase)
                        {
                            input.AddValidationError(nameInput, "Name must be all lowercase letters.");
                            return Task.CompletedTask;
                        }

                        return Task.CompletedTask;
                    }
                },
                cancellationToken: context.CancellationToken);

            if (newContainerInput.Canceled)
            {
                throw new OperationCanceledException("Container addition was canceled by the user.");
            }

            Container container = new(newContainerInput.Data.Value!);

            var jsonContent = new StringContent(JsonSerializer.Serialize(container), Encoding.UTF8, MediaTypeNames.Application.Json);

            context.Request.Content = jsonContent;
        }

        record Container(string Name);
    }
}