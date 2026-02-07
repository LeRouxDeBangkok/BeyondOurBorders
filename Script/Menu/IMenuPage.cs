namespace Beyondourborders.Script.Menu;

public interface IMenuPage {
    // Screen size is 1280 x 720.
    // Format is x, y
    // Eg if you put (0, 720), the screen will scroll down to go to that page.
    public (int, int) GetButtonOffsetFromPreviousPage();

    public (int, int) GetBackgroundOffsetFromPreviousPage();
}