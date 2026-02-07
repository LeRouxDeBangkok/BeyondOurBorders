using System;
using Godot;

namespace Beyondourborders.Script.Utils;

public static class TweenUtils {
    public static Tween RemakeTransitionTween(Func<Tween> createFunc, Tween oldObject) {
        if (oldObject != null && oldObject.IsValid())
        {
            oldObject.Kill();
        }

        oldObject = createFunc();
        oldObject.SetTrans(Tween.TransitionType.Quad);
        oldObject.SetEase(Tween.EaseType.InOut);
        return oldObject;
    }
}