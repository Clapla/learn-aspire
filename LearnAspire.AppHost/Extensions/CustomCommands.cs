namespace LearnAspire.AppHost.Extensions;

public static class CustomCommands
{
    public static IResourceBuilder<ProjectResource>
        WithDeleteDatabaseCommand(this IResourceBuilder<ProjectResource> builder)
    {
        return builder.WithCommand("Delete databse", "Delete database",
            async context =>
            {
                return new ExecuteCommandResult { Success = true };
            });
    }
}
