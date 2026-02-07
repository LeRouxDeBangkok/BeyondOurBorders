using Godot;

namespace Beyondourborders.Script.NPC.Types;

public partial class AskingDoorDialog : NpcDialog {
    // F vite f plus le temps
    private int[] Answers = new int[] { 0, 0, 1, 2 };

    private string[] Questions = {
        "", 
        "Que dit une théière devant un ascenseur? 1-'Je veux mon thé', 2-'Thé super beau', 3-'Thé g'", 
        "Que dit un mexicain en faisant sa lessive? 1-'Ay cabrón', 2-'C'est salsa', 3-'je sais pas je parle pas mexicain'",
        "La réponse de la vie, l’univers, et tout 1-'Jean-Luc Mélenchon', 2-'Une saucisse', 3-'42'"
    };
    
    
    private int questionNum = 0;
    public AskingDoor parent;
    
    public override void _Ready() {
        base._Ready();
        parent = GetParent<AskingDoor>();
    }

    public override void NextLine() {
        if (DialogI == 0 || DialogI == 2 || DialogI == 3) {
            base.NextLine();
        }
    }


    public override void _Process(double delta) {
        base._Process(delta);
        if (DialogI == 0)
            return;

        if (questionNum == 4) {
            return;
        }
        
        var answerNum = Answers[questionNum];
        if (Input.IsActionJustPressed("Reponse1")) {
            if (answerNum != 0) {
                Text = "";
                CurrentLine = "Hé non fraté hop hop hop on recommence.";
                DialogI = 0;
                questionNum = 0;
            } else {
                questionNum++;
                if (questionNum == 4) {
                    DialogI = 2;
                    NextLine();
                    return;
                }
                Text = "";
                CurrentLine = Questions[questionNum];
            }
        }
        if (Input.IsActionJustPressed("Reponse2")) {
            if (answerNum != 1) {
                Text = "";
                CurrentLine = "Hé non fraté hop hop hop on recommence.";
                DialogI = 0;
                questionNum = 0;
            } else {
                questionNum++;
                if (questionNum == 4) {
                    DialogI = 2;
                    NextLine();
                    return;
                }
                Text = "";
                CurrentLine = Questions[questionNum];
            }
        }
        if (Input.IsActionJustPressed("Reponse3")) {
            if (answerNum != 2) {
                Text = "";
                CurrentLine = "Hé non fraté hop hop hop on recommence.";
                DialogI = 0;
                questionNum = 0;
            } else {
                questionNum++;
                if (questionNum == 4) {
                    DialogI = 2;
                    NextLine();
                    return;
                }
                Text = "";
                CurrentLine = Questions[questionNum];
            }
        }
    }

    public void RemoveDoor() {
        parent.Collision.Disabled = true;
        parent.Hide();
    }
}