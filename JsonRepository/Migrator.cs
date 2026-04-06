namespace JsonRepository;

using System.Text.Json.Nodes;

public class Migrator
{
    public required IList<Func<JsonNode, Task>> Migrations { get; init; }

    public async Task Migrate(JsonNode node)
    {
        if (
            node?.AsObject().TryGetPropertyValue("schemaVersion", out var versionNode) != true
            || versionNode?.GetValue<int>() is not int version
        )
        {
            throw new InvalidOperationException("JSON does not contain a valid schemaVersion");
        }

        if (version < 0 || version > Migrations.Count)
        {
            throw new JsonRepositoryException($"Invalid version number for migration: {version}");
        }

        foreach (var migration in Migrations.Skip(version))
        {
            await migration(node).ConfigureAwait(false);
        }
    }
}
