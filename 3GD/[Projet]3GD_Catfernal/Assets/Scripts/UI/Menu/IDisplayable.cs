
namespace Game.UI
{
    /// <summary>
    /// An interface to allow an UI element to be hide/unhide from the screen.
    /// </summary>
    public interface IDisplayable
    {
        void SetVisibility(bool visible, bool canvasVisibility = true);
    }
}