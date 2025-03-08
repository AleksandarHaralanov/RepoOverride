namespace RepoOverride.Interfaces
{
    public interface IPlayerFinder
    {
        object PlayerHealthInstance { get; }
        object PlayerAvatarInstance { get; }
        bool FindPlayerInstances();
    }
}