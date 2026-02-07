using System.Collections.Generic;
using Godot;

namespace Beyondourborders.Script.ClientData;

// A REVOIR,
// Je px aussi faire un truc ou chaque objet checkpoint tu px lui foutre les cos depuis Godot
// et que ca save ca ds le json.
// Pr l'instant en tt cas c ca.
#nullable enable
public class Checkpoint {
    public string IdName { get; private set; }
    public string SaveSelectorName { get; private set; }
    public string Level { get; private set; }
    public Vector2 BigBobSpawn { get; private set; }
    public Vector2 SmallBobSpawn { get; private set; }

    public Checkpoint(string idName, string saveSelectorName, string level, Vector2 bigBobSpawn, Vector2 smallBobSpawn) {
        this.IdName = idName;
        this.SaveSelectorName = saveSelectorName;
        this.Level = level;
        this.BigBobSpawn = bigBobSpawn;
        this.SmallBobSpawn = smallBobSpawn;
    }
}
public static class CheckpointList {
    private static readonly Dictionary<string, Checkpoint?> Checkpoints = new ();

    public static Checkpoint DefaultCheckpoint { get; } =
        new Checkpoint("default", "Level 1", "Level1", new Vector2(0, -100), new Vector2(20, -100));
        //new Checkpoint("default", "Level 1", "Level1", new Vector2(-952, 2299), new Vector2(20, -100));
    
    
    static CheckpointList() {
        Checkpoints.Add("default", DefaultCheckpoint);
        Checkpoints.Add("test", new Checkpoint("test", "testchk", "Level1", new Vector2(0, 0), new Vector2(20, 0)));
        Checkpoints.Add("debutLevel2", new Checkpoint("debutLevel2","Level 2", "Level2",new Vector2(2120, 0), new Vector2(2140, 0)));
        Checkpoints.Add("beforeBossLevel2", new Checkpoint("beforeBossLevel2", "Level 2", "Level2", new Vector2(12000,0), new Vector2(11980,0)));
        Checkpoints.Add("Level3", new Checkpoint("Level3", "Level 3","Level3", new Vector2(-450,-850), new Vector2(-470,-850)));
        Checkpoints.Add("Level4", new Checkpoint("Level4", "Level 4","Level4", new Vector2(-800,-1700), new Vector2(-820,-1700)));
    }

    public static Checkpoint? GetCheckpoint(string name) {
        Checkpoints.TryGetValue(name, out var res);
        return res;
    }
}