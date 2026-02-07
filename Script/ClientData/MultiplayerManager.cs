using System;
using Beyondourborders.Script.Entities.Players;
using Beyondourborders.Script.Overlay;
using Beyondourborders.Script.Utils;
using Godot;

namespace Beyondourborders.Script.ClientData;

public partial class MultiplayerManager : Node2D {
    public static MultiplayerManager Instance { get; private set; }
    public MultiplayerManager() {
        Instance = this;
        IsBroadcasting = false;
        GuestId = -1;
    }
    
    [Export] public int DefaultPort = 55405;
    
    public bool IsBroadcasting { get; private set; }
    private ENetMultiplayerPeer _peer;
    public int GuestId;

    public void StartLanBroadcasting(int port) {
        IsBroadcasting = true;
        Multiplayer.PeerConnected += PeerConnected;
        Multiplayer.PeerDisconnected += PeerDisconnected;
        
        _peer = new ENetMultiplayerPeer();
        var error = _peer.CreateServer(port, 2);
        if(error != Error.Ok){
            GD.PushError("error cannot host! :" + error);
            return;
        }
        _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = _peer;
        GD.Print("Lan broadcasting !");
        Client.Instance.CurrentPlayer!.HideSynchronizer();
        Client.Instance.OtherPlayer!.HideSynchronizer();
    }

    public void StopLanBroadcasting() {
        IsBroadcasting = false;
        GuestId = -1;
        Multiplayer.PeerConnected -= PeerConnected;
        Multiplayer.PeerDisconnected -= PeerDisconnected;
        Client.Instance.IsMultiplayer = false;
        WaitUtils.WaitFor(0.2, () => {
            var host = _peer.Host;
            _peer.Close();
            host.Destroy();
        });

    }
    
    private void PeerDisconnected(long id)
    {
        GuestId = -1;
        GD.Print("Player Disconnected: " + id.ToString());
        
        Client.Instance.IsMultiplayer = false;
        Client.Instance.CurrentPlayer!.HideSynchronizer();
        Client.Instance.OtherPlayer!.HideSynchronizer();
    }
    private void PeerConnected(long id) {
        GuestId = (int)id;
        GD.Print("Player Connected! " + id.ToString());
        
        Client.Instance.IsLoading = false;
        Hud.Instance.TransitionScreen.ShowScreen("Player is joining...");
        
        if (Client.Instance.CurrentPlayer is SmallBob)
            Client.Instance.SwitchPlayers(force: true);

        Client.Instance.IsMultiplayer = true;
        Client.Instance.OtherPlayer!.SetMultiplayerAuthority((int)id);
        Client.Instance.Rpc("FromHostSetSmallBobCoordinates", Client.Instance.OtherPlayer.GlobalPosition);
    }

    
    public void JoinGame(string ip, int port) {
        _peer = new ENetMultiplayerPeer();
        _peer.CreateClient(ip, port);
        _peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
        Multiplayer.MultiplayerPeer = _peer;
        
        Hud.Instance.TransitionScreen.ShowScreen("Connecting...");


        Multiplayer.ConnectionFailed += GuestConnectionFailed;
        Multiplayer.ConnectedToServer += GuestConnectedToServer;
    }

    private void GuestConnectionFailed() {
        GD.Print("Failed");
        Hud.Instance.TransitionScreen.RemoveScreen();
        Multiplayer.ConnectionFailed -= GuestConnectionFailed;
        Multiplayer.ConnectedToServer -= GuestConnectedToServer;
    }

    private void GuestConnectedToServer() {
        GD.Print("Connected");

        Client.Instance.IsHost = false;
        Client.Instance.IsMultiplayer = true;
        Client.Instance.LoadGameFromGameDataGuest();
        Multiplayer.ConnectionFailed -= GuestConnectionFailed;
        Multiplayer.ConnectedToServer -= GuestConnectedToServer;
        Multiplayer.PeerDisconnected += OnGuestDisconnect;
    }

    private void OnGuestDisconnect(long id) {
        GD.Print("GUEST: DISCONNECTED");
        Client.Instance.IsMultiplayer = false;
        Client.Instance.IsHost = true;
        DisconnectFromHost();
        Hud.Instance.PauseMenu.CloseMenuSafe();
        Client.Instance.LeaveGameToMenu();
        Multiplayer.PeerDisconnected -= OnGuestDisconnect;
    }

    public void DisconnectFromHost() {
        _peer.Close();
        GetTree().SetMultiplayer(MultiplayerApi.CreateDefaultInterface());
    }
}
