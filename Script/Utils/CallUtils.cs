using System;
using Godot;

namespace Beyondourborders.Script.Utils;

public static class CallUtils {
    public static void CallDeferred(Action action) {
        Callable.From(action).CallDeferred();
    }

    public static Action CallDeferredA(Action action) {
        return () => CallDeferred(action);
    }
}