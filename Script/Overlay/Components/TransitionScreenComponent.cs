using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.Overlay.Components;

public partial class TransitionScreenComponent : ColorRect {
    private readonly float _duration = 0.2f;
    
    private Tween _opacityTween;
    private Label _text;
    private CharacterBody2D _walkingBob;
    private AnimatedSprite2D _walkingBobSprite;
    
    public bool IsDoneTransitioning { get; private set; }
    
    public override void _Ready()
    {
        this.Modulate = Color.Color8(255, 255, 255, 0);

        _text = GetNode<Label>("TransitionText");
        _walkingBob = GetNode<CharacterBody2D>("WalkingBob");
        _walkingBobSprite = _walkingBob.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _walkingBobSprite.Pause();
    }
    
    public void ShowScreen(string message) {
        _text.Text = message;

        Visible = true;
        
        _walkingBobSprite.Play();

        IsDoneTransitioning = false;
        _opacityTween = TweenUtils.RemakeTransitionTween(CreateTween, _opacityTween);
        _opacityTween.TweenProperty(
            this,
            "modulate",
            Color.Color8(255, 255, 255, 255),
            _duration);
        _opacityTween.Finished += () => IsDoneTransitioning = true;
    }

    public void RemoveScreen() {
        IsDoneTransitioning = false;
        _opacityTween = TweenUtils.RemakeTransitionTween(CreateTween, _opacityTween);
        _opacityTween.TweenProperty(
            this,
            "modulate",
            Color.Color8(255, 255, 255, 0),
            _duration);
        _opacityTween.Finished += () => IsDoneTransitioning = true;
        
        _opacityTween.Finished += DisableViewOnTweenDone;
    }

    public void SetMessage(string newMessage) {
        _text.Text = newMessage;
    }

    private void DisableViewOnTweenDone()
    {
        Visible = false;
        _walkingBobSprite.Pause();
        _opacityTween.Finished -= DisableViewOnTweenDone;
    }
}