namespace Kernel.UI.MenuViews
{
    public interface IMenuView
    {
        bool Shown { get; }
        
        void ShowSelf();
        void HideSelf();
    }
}