using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players.Properties;
using Beyondourborders.Script.NPC.Types;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.NPC;

/* 
 * Techniquement cque jv mettre pr notre jeu cpas super important psk bon voila c un jeu vite f
 * Parcontre c a prendre en compte pr tt le reste:
 * Ici j'ai foutu un truc pr pouvoir exécuter des commandes depuis un input de texte
 * C'est une GIGA IDÉE DE CON, a ne JAMAIS refaire ds n'importe quoi de sérieux.
 * Meme si ici j'ai limité le truc en faisant en sorte que tu peux juste appeler des fonctions
 * a partir de leur nom de cette classe et sans paramètres, c'est à éviter
 */
public partial class NpcDialog : RichTextLabel {
    protected NpcBase Parent;
    
    // TODO: add to options
    private const int ProcessEveryXFrame = 2;
    private const int YOffsetForAnimation = 20;

    private int _processEveryFrameCounter = 0;
    
    private Vector2 _initialPosition;
    
    private Tween _positionTween = null;
    private Tween _opacityTween = null;
    
    private bool _isShown;

    // private bool _currentLineOverriden;
    protected string CurrentLine;
    
    protected string[] DialogLines;
    protected int DialogI;
    
    public override void _Ready() {
        _initialPosition = new Vector2(Position.X, Position.Y);

        DialogLines = Text.Split('\n');
        DialogI = 0;
        CurrentLine = DialogLines[0];
        Text = "";
        Modulate = new Color(255, 255, 255, 0);
        GlobalPosition = new Vector2(_initialPosition.X, _initialPosition.Y + YOffsetForAnimation);

        Parent = GetParent<NpcBase>();
    }

    public void FancyShow() {
        _isShown = true;
        
        _positionTween = TweenUtils.RemakeTransitionTween(CreateTween, _positionTween);
        _positionTween.TweenProperty(
            this, 
            "position", 
            new Vector2(_initialPosition.X, _initialPosition.Y),
            0.2f);
        
        _opacityTween = TweenUtils.RemakeTransitionTween(CreateTween, _opacityTween);
        _opacityTween.TweenProperty(
            this,
            "modulate",
            new Color(1, 1, 1, 1),
            0.2f);
    }

    public void FancyHide() {
        _isShown = false;
        
        _opacityTween = TweenUtils.RemakeTransitionTween(CreateTween, _opacityTween);
        _opacityTween.TweenProperty(
            this,
            "modulate",
            new Color(1, 1, 1, 0),
            0.2f);
        
        _positionTween = TweenUtils.RemakeTransitionTween(CreateTween, _positionTween);
        _positionTween.TweenProperty(
            this, 
            "position", 
            new Vector2(_initialPosition.X, _initialPosition.Y + YOffsetForAnimation),
            0.2f);
        
        _positionTween.Finished += () => SetTextIndexSafe(0);
    }

    private void SetTextIndexSafe(int newIndex) {
        // _currentLineOverriden = false;
        int tempDialogI = newIndex % DialogLines.Length;
        if (tempDialogI < 0)
        {
            tempDialogI += DialogLines.Length;
        }

        if (DialogLines[tempDialogI] == "") {
            CurrentLine = "";
            Text = "";
            return; // kinda shit way to have the characters stop answering if at empty line.
        }

        DialogI = tempDialogI;
        
        CurrentLine = DialogLines[DialogI];
        if (CurrentLine.StartsWith("[center]"))
            Text = "[center]";
        else
            Text = "";
    }

    public virtual void NextLine() {
        SetTextIndexSafe(DialogI+1);
        if (!CurrentLine.StartsWith("() => "))
            return;

        var funcName = CurrentLine.Replace("() => ", "").Trim();
        switch (funcName) {
            case "GiveBigJump":
                GameData.CurrentGameData.Abilities += (UnlockableAbility.BigJump.ToString());
                break;
            case "SaveGame":
                GD.Print("SaveGame called.");
                if (Client.Instance.IsGuest) {
                    SetCurrentLineFully("[center]Only the host can save.");
                    return;
                }

                GameData.CurrentGameData!.LastCheckpoint = ((CheckpointNpcDialog)this).CheckpointNpc.CheckpointName;
                SaveHelper.Instance.Save();
                Client.Instance.CurrentPlayer.CurrentHp = Client.Instance.CurrentPlayer.HPMax;
                Client.Instance.OtherPlayer.CurrentHp = Client.Instance.OtherPlayer.HPMax;
                Client.Instance.CurrentPlayer.HpBar.Value = Client.Instance.CurrentPlayer.CurrentHp;
                Client.Instance.OtherPlayer.HpBar.Value = Client.Instance.OtherPlayer.CurrentHp;
                break;
            case "GiveDash":
                GameData.CurrentGameData.Abilities += (UnlockableAbility.Dash.ToString());
                break;
            case "GiveDoubleJump":
                GameData.CurrentGameData.Abilities += (UnlockableAbility.DoubleJump.ToString());
                break;
            case "GiveSword":
                GameData.CurrentGameData.Weapon = 2;
                break;
            case "GiveShittySword":
                GameData.CurrentGameData.Weapon = 1;
                break;
            case "RemoveDoor":
                ((AskingDoorDialog)this).RemoveDoor();
                break;
            
        }
        
        NextLine();

        // var method = GetType().GetMethod(CurrentLine.Replace("() => ", ""));
        // if (method is null) {
        //     GD.PushWarning("Method name from text is null: " + CurrentLine);
        //     NextLine();
        //     return;
        // }
        //
        // var funcRes = method.Invoke(this, new object[] { });
        // if (funcRes is bool and true) {
        //     // If result is true, do not get a new line instantly.
        // } else {
        //     NextLine();
        // }
    }

    public void SetCurrentLineFully(string line) {
        CurrentLine = line;
        if (CurrentLine.StartsWith("[center]"))
            Text = "[center]";
        else
            Text = "";
    }
    
    public override void _PhysicsProcess(double delta) {
        if (!_isShown)
            return;
        
        _processEveryFrameCounter++;
        if (_processEveryFrameCounter < ProcessEveryXFrame)
            return;

        if (Text.Length < CurrentLine.Length)
            Text += CurrentLine[Text.Length];
    }
}