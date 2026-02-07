using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Entities.Players;
using Godot;

namespace Beyondourborders.Script.Utils;

public partial class SimpleResizableArea : Area2D
{
    protected CollisionShape2D CollisionShape;
    protected RectangleShape2D RectangleShape;
    public SimpleResizableArea() { }

    public SimpleResizableArea(string name)
    {
        
    }

    protected bool IsFromThisClient(Node2D body) {
        if (body is not PlayerBase)
            return false;
        
        return !Client.Instance.IsMultiplayer || AreaUtils.IsRightAreaClient(body.Name, Multiplayer.GetUniqueId());
    }
    
    protected bool IsCurrentActivePlayer(Node2D body) {
        if (body is not PlayerBase)
            return false;
        
        if (Client.Instance.IsMultiplayer) {
            return AreaUtils.IsRightAreaClient(body.Name, Multiplayer.GetUniqueId());
        }
        return body.Name == Client.Instance.CurrentPlayer.Name;
    }

    public override void _Ready()
    {
        this.CollisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        this.RectangleShape = (RectangleShape2D)CollisionShape.GetShape();
    }
}