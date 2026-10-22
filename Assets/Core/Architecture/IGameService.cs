namespace Core.Architecture
{
    /// <summary>
    /// Base contract interface for all decoupled enterprise services registered in the ServiceLocator.
    /// </summary>
    public interface IGameService
    {
        void Initialize();
        void Dispose();
    }
}
