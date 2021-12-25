namespace Kernel.UI.Menus
{
    public interface IMenuView
    {
        bool Shown { get; }
        
        void ShowSelf();
        void HideSelf();
    }
}