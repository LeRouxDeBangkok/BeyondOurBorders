using Beyondourborders.Script.ClientData;
using Godot;

namespace Beyondourborders.Script.Entities.Enemies;

public partial class EnemyGhettoSynchronizer : MultiplayerSynchronizer {
    public override void _Ready() {
        this.PublicVisibility = false;
        GhettoMobSynchronizerManager.Instance.AddSynchronizer(this);

        if (!Client.Instance.IsMultiplayer) {
            this.SetMultiplayerAuthority(Multiplayer.GetUniqueId()); // Should be 1.
            return;
        }
        
        if (Client.Instance.OtherPlayer.Zone == Client.Instance.CurrentPlayer.Zone) {
            int authorityId = Client.Instance.IsGuest ? 1 : MultiplayerManager.Instance.GuestId;
            this.SetMultiplayerAuthority(authorityId);
            GD.Print($"New Synchronizer ({Client.Instance.CurrentPlayer.Name}): set to other AuthorityID ({authorityId}) - isguest: {Client.Instance.IsGuest}");
            // this.PublicVisibility = true;
        } else {
            GD.Print($"New Synchronizer ({Client.Instance.CurrentPlayer.Name}): set to current AuthorityID ({Multiplayer.GetUniqueId()})");
            this.SetMultiplayerAuthority(Multiplayer.GetUniqueId());
            // this.PublicVisibility = false;
        }
        
    }
}