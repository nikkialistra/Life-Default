namespace Selecting
{
    public interface ISelectable
    {
        void Hover();

        void Flash();

        void Select();
        void Deselect();

        void StopDisplay();
    }
}
