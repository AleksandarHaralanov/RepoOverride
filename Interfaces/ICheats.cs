namespace RepoOverride.Interfaces
{
    public interface ICheats
    {
        bool GodModeEnabled { get; }
        bool MapEnemiesEnabled { get; }

        void Update();
        void ToggleGodMode();
        void ToggleMapEnemies();
        void MaxHeal();
        void MapEnemies();
    }
}
