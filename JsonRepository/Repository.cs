using System.Text.Json;
using System.Text.Json.Nodes;
using Nito.AsyncEx;

namespace ManagementServer.Infrastructure.Persistence;

public abstract class Repository<TData>(FileSystemHelper fileSystemHelper) : IOnQuit, IInitialize, IDisposable
    where TData : class, IDeepCloneable<TData>, IMigratable, new()
{
    private readonly SemaphoreSlim saveLock = new(1, 1);
    private readonly JsonSerializerOptions jsonSerializerOptions = AppJsonSerializerOptions.Default;
    private Debouncer debouncer = null!;
    private TData data = null!;

    protected abstract JsonMigrator Migrator { get; }

    private static string FilePath => $"{typeof(TData).Name}.json";

    private AsyncContextThread Worker { get; } = new();

    public async Task Initialize()
    {
        try
        {
            await using FileStream reader = fileSystemHelper.OpenRead(FilePath);
            var node = await JsonNode.ParseAsync(reader)
                ?? throw new InvalidOperationException("Deserialized JSON is null");

            await Migrator.Migrate(node);

            data = node.Deserialize<TData>(jsonSerializerOptions)
                ?? throw new InvalidOperationException("Deserialized JSON is null");
        }
        catch (Exception exception) when (exception is FileNotFoundException or DirectoryNotFoundException)
        {
            data = new();
        }

        debouncer = new Debouncer
        {
            FirstDelay = TimeSpan.FromMilliseconds(500),
            SubsequentDelay = TimeSpan.FromMilliseconds(500),
            Action = Save,
        };
    }

    public Task<T> Read<T>(Func<TData, Task<T>> reader) =>
        Worker.Factory.Run(() => reader(data));

    public Task<T> Read<T>(Func<TData, T> reader) =>
        Worker.Factory.Run(() => reader(data));

    public Task Write(Func<TData, Task> modifier) =>
        Worker.Factory.Run(async () =>
        {
            var draft = data.DeepClone();
            await modifier(draft);
            data = draft;
            debouncer.MarkDirty();
        });

    public Task Write(Action<TData> modifier) =>
        Worker.Factory.Run(() =>
        {
            var draft = data.DeepClone();
            modifier(draft);
            data = draft;
            debouncer.MarkDirty();
        });

    public Task<T> Modify<T>(Func<TData, Task<T>> modifier) =>
        Worker.Factory.Run(async () =>
        {
            var draft = data.DeepClone();
            var result = await modifier(draft);
            data = draft;
            debouncer.MarkDirty();
            return result;
        });

    public Task<T> Modify<T>(Func<TData, T> modifier) =>
        Worker.Factory.Run(() =>
        {
            var draft = data.DeepClone();
            var result = modifier(draft);
            data = draft;
            debouncer.MarkDirty();
            return result;
        });

    public Task OnQuit() => Save();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Worker.Dispose();
            saveLock.Dispose();
        }
    }

    private async Task Save()
    {
        try
        {
            var data = await Read(x => x.DeepClone());

            using (await saveLock.LockAsync())
            {
                await using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, data, jsonSerializerOptions);
                stream.Position = 0;
                await fileSystemHelper.WriteToFile(FilePath, stream);
            }
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to save {FilePath}: {ex}");
        }
    }
}
