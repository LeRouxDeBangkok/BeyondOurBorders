using Godot;
using System;
using Beyondourborders.Script;

public partial class InventoryButton : Button
{
    private Item inventoryItem;
    private TextureRect icon;
    private Label quantityLabel;
    private int index;
    public int ItemIndex { get; private set; }
    [Signal]
    public delegate void ConsumeRequestEventHandler(int index);
    public override void _Ready()
    {
        icon = GetNode<TextureRect>("TextureRect");
        quantityLabel = GetNode<Label>("Label");
		
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
	
    public override void _Pressed()
    {
        EmitSignal(SignalName.ConsumeRequest, ItemIndex);
    }

    public void UpdateItem(Item item, int index)
    {
        this.index = index;
        ItemIndex = index;
        inventoryItem = item;

        if (item != null)
        {
            icon.Texture = item.Icon;
            quantityLabel.Text = item.Quantity.ToString();
        }
        else
        {
            icon.Texture = null;
            quantityLabel.Text = "";
        }
    }
}