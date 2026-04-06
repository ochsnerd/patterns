namespace JsonRepository;

using System.Text.Json;
using System.Text.Json.Nodes;
using Nito.AsyncEx;

public abstract class Repository<TData> : IOnQuit, IInitialize, IDisposable
    where TData : class, IDeepCloneable<TData>, IMigratable, new()
{
    private readonly SemaphoreSlim saveLock = new(1, 1);
    private readonly JsonSerializerOptions jsonSerializerOptions = new();
    private Debouncer debouncer = null!;
    private TData data = null!;

    protected abstract Migrator Migrator { get; }

    private static string FilePath => $"{typeof(TData).Name}.json";

    private AsyncContextThread Worker { get; } = new();

    public async Task Initialize()
    {
        try
        {
            await using FileStream reader = new(FilePath, new FileStreamOptions { Options = FileOptions.Asynchronous });
            var node =
                await JsonNode.ParseAsync(reader).ConfigureAwait(false) ?? throw new InvalidOperationException("Deserialized JSON is null");

            await Migrator.Migrate(node).ConfigureAwait(false);

            data =
                node.Deserialize<TData>(jsonSerializerOptions)
                ?? throw new InvalidOperationException("Deserialized JSON is null");
        }

        // intentionally crash on JsonException to prevent overwriting unexpected data
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

    public Task<T> Read<T>(Func<TData, Task<T>> reader) => Worker.Factory.Run(() => reader(data));

    public Task<T> Read<T>(Func<TData, T> reader) => Worker.Factory.Run(() => reader(data));

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
        Dispose(disposing: true);
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
            var localData = await Read(x => x.DeepClone()).ConfigureAwait(false);

            // TODO: why do we lock here?
            using (await saveLock.LockAsync())
            {
                await using var stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, localData, jsonSerializerOptions);
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
