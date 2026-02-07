using System;
using Godot;

namespace Beyondourborders.Script.Entities.Players.Properties;

public enum Dashing {
    Left,
    Right,
    //Up,
    //Down,
    Not
}

public class DashHandler { //TODO heuuuu le dash quand tu bouges pas il marche un peu trop bien
    private long _lastDashTime = 0; // Unix time of last dash (cooldown)

    private readonly Timer _dashTimer;
    private readonly float _dashVelocity;
    private readonly long _dashDelayMs;
    private readonly bool _unlimitedAirDashes;

    public Dashing State { get; private set; }
    private bool _canDashAir;

    public DashHandler(Timer node, float dashVelocity, long dashDelayMs, bool unlimitedAirDashes) {
        this._dashTimer = node;
        this._dashVelocity = dashVelocity;
        this._dashDelayMs = dashDelayMs;
        this._unlimitedAirDashes = unlimitedAirDashes;

        this.State = Dashing.Not;
        this._canDashAir = false;
        
        this._dashTimer.Timeout += OnDashTimerTimeout;
    }
    
    public Vector2 GetDashVelocityVector() {
        //TODO: Handle vertical dash (if ever implemented)
        return new Vector2(
            x: State == Dashing.Right ? _dashVelocity : -_dashVelocity,
            y: 0
        );
    }
    
    public (bool can, long checkTime) CanDash(bool onFloor) {
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (currentTime - _lastDashTime <= _dashDelayMs)
            return (false, 0);

        if (onFloor || _unlimitedAirDashes)
            return (true, currentTime);

        if (!_canDashAir)
            return (false, 0);

        _canDashAir = false;
        return (true, currentTime);
    }
    
    // Unsure as to if this func should check CanDash
    // or if it should JUST perform the dash and assume the check's been done somewhere else.
    // For now going with the latter.
    public void PerformDash(long time, bool playerFlipped) {
        _dashTimer.Start();
        _lastDashTime = time;
        State = playerFlipped ? Dashing.Right : Dashing.Left;
    }
    
    // Not sure if should be called OnFloor(=when it should be called) or ResetAirStatus (=what it does)
    public void ResetAirStatus() {
        if (!_canDashAir)
            _canDashAir = true;
    }
    
    private void OnDashTimerTimeout() {
        State = Dashing.Not;
    }
}