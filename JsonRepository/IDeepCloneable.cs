namespace JsonRepository;

// TODO: Can we add implementation for deepclone via attribute & sourcegen?
public interface IDeepCloneable<out T>
{
    /// <summary>
    /// Creates a deep clone of this object. All reference types contained in this object must also be deep cloned, so that the clone is completely independent of the original.
    /// </summary>
    T DeepClone();
}
