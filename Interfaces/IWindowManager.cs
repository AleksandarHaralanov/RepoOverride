namespace RepoOverride.Interfaces
{
    public interface IWindowManager
    {
        bool IsVisible { get; }
        void Toggle();
        void OnGUI();
    }
}