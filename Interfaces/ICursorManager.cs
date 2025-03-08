namespace RepoOverride.Interfaces
{
    public interface ICursorManager
    {
        void Update(bool menuActive);
        void RestoreState();
    }
}