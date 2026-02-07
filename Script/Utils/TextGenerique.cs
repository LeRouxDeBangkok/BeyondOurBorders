using Godot;

namespace Beyondourborders.Script.Utils;


public partial class TextGenerique : Control
{
    [Export]
    public int ScrollSpeed = 50;

    private RichTextLabel creditsLabel;
    private float initialY;
    private Sprite2D _bob;
    private Timer _timer;
    public bool Start = false;

    public TextGenerique()
    {
        
    }

    public override void _Ready()
    {
        creditsLabel = GetNode<RichTextLabel>("RichTextLabel");
        _bob = GetNode<Sprite2D>("BobDiscoHd");
        _timer = GetNode<Timer>("Timer");
        initialY = creditsLabel.Position.Y;
    }

    public override void _Process(double delta)
    {
        if(Start)
        {
            // Défilement automatique du texte des crédits
            var position = creditsLabel.Position;
            position.Y -= ScrollSpeed * (float)delta;
            creditsLabel.Position = position;

            if (_timer.IsStopped())
            {
                _timer.Start();
                _bob.FlipH = !_bob.FlipH;
            }
        }
    }
}