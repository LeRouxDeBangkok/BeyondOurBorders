using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Beyondourborders.Script.Utils;

public static class WaitUtils {
    public static void WaitFor(double seconds, Action callback) {
        WaitFor(TimeSpan.FromSeconds(seconds), callback);
    }
    public static void WaitFor(TimeSpan waitTime, Action callback) {
        Task.Run(() => {
            Thread.Sleep(waitTime);
            callback.Invoke();
        });
    }
    public static void WaitForSinceTime(double startTime, double seconds, Action callback) {
        WaitUntil(
            () => Time.GetUnixTimeFromSystem() < startTime + seconds,
            callback
        );
    }
    public static void WaitUntil(Func<bool> condition, Action callback, int pollingRate = 10) {
        Task.Run(() => {
            while (condition()) {
                Thread.Sleep(pollingRate);
            }
            callback.Invoke();
        });
    }
}