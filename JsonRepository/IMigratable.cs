namespace JsonRepository;

public interface IMigratable
{
    int SchemaVersion { get; }
}
