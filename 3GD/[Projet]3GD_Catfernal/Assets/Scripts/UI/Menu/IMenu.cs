
namespace Game.UI
{
    public interface IMenu
    {
        int MenuIndex { get; }
        int LastMenuIndex { get; set; }

        void OnMenuSelected();
    }
}