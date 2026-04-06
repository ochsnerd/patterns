namespace JsonRepository;

public interface IInitialize
{
    /// <summary>
    /// Called during application startup.
    /// </summary>
    Task Initialize();
}
