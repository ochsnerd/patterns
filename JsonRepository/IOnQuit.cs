namespace JsonRepository;

public interface IOnQuit
{
    /// <summary>
    /// This method will be called when the application is quitting.
    /// </summary>
    Task OnQuit();
}
