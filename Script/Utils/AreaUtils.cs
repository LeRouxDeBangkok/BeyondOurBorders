namespace Beyondourborders.Script.Utils;

public class AreaUtils {
    public static bool IsRightAreaClient(string nodeName, int id) {
        return (nodeName == "SmallBob" && id != 1) || (nodeName == "BigBob" && id == 1);
    }
}