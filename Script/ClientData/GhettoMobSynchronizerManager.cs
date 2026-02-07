using System.Collections.Generic;
using Beyondourborders.Script.Entities.Enemies;
using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.ClientData;

public partial class GhettoMobSynchronizerManager : Node2D {
    public static GhettoMobSynchronizerManager Instance;

    public GhettoMobSynchronizerManager() {
        Instance = this;
        Synchronizers = new List<EnemyGhettoSynchronizer>();
    }

    private List<EnemyGhettoSynchronizer> Synchronizers;

    public void AddSynchronizer(EnemyGhettoSynchronizer synchronizer) {
        this.Synchronizers.Add(synchronizer);
    }
    
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RightBeforePlayerChangeZone(PlayerBase p, string newZone) {
        GD.Print("RightBeforePlayerChangeZone CALLED !");
        if (p == Client.Instance.CurrentPlayer) {
            this.Synchronizers.Clear();
        } else {
            if (newZone == Client.Instance.OtherPlayer.Zone) {
                int authorityId = Client.Instance.IsGuest ? 1 : MultiplayerManager.Instance.GuestId;
                foreach (var enemyGhettoSynchronizer in Synchronizers) {
                    enemyGhettoSynchronizer.SetMultiplayerAuthority(authorityId);
                }
            }
            else {
                foreach (var enemyGhettoSynchronizer in Synchronizers) {
                    enemyGhettoSynchronizer.SetMultiplayerAuthority(Multiplayer.GetUniqueId());
                }
            }

        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RightAfterPlayerDoneChangingZoneRpc(PlayerBase p) {
        RightAfterPlayerDoneChangingZone(p);
    }
    public void RightAfterPlayerDoneChangingZone(PlayerBase p) {
        if (Client.Instance.CurrentPlayer.Zone == Client.Instance.OtherPlayer.Zone) {
            foreach (var enemyGhettoSynchronizer in Synchronizers) {
                GD.Print("Oe? " + enemyGhettoSynchronizer.GetMultiplayerAuthority() + " "  + Client.Instance.CurrentPlayer.Name);
                enemyGhettoSynchronizer.PublicVisibility = true;
            }
        } else {
            
        }
    }

    public void OnGuestLeaveMultiplayer() {
        foreach (var s in Synchronizers) {
            s.PublicVisibility = false;
            s.SetMultiplayerAuthority(Multiplayer.GetUniqueId());
        }
    }
}