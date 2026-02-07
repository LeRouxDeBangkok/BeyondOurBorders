using Godot;
using System;
using System.Collections.Generic;
using Beyondourborders.Script;
using Beyondourborders.Script.ClientData;
using Beyondourborders.Script.Utils;
using Beyondourborders.Script.Item;
using Beyondourborders.Script.Overlay;

public partial class Inventory : Control
{
	private GridContainer gridContainer;
	private PackedScene inventoryButton;

	[Export]
	private string _itemButtonPath = "res://Scene/Inventory/inventory_button.tscn";

	[Export]
	private string _testItemPath = "res://Scene/Item/TestItem.tres";

	[Export] 
	public int Capacity { get; set; } = 24;

	private List<Item> items = new List<Item>();
	private List<InventoryButton> inventoryButtons = new List<InventoryButton>();

	private bool _isInitialized = false;

	private Tween _appearTween; // pr lanimation dapparition tkt bob

	public string Chat = "";
	
	public bool IsInitialized() => _isInitialized;
	
	
	
	public override void _Ready()
	{
		if (_isInitialized) return;
		_isInitialized = true;

		gridContainer = GetNode<GridContainer>("ScrollContainer/GridContainer");
		inventoryButton = ResourceLoader.Load<PackedScene>(_itemButtonPath);

		if (inventoryButton == null)
		{
			GD.PrintErr($"Failed to load inventory button scene at path: {_itemButtonPath}");
			return;
		}
		PopulateButtons();
	}

	private void PopulateButtons()
	{
		foreach (Button child in gridContainer.GetChildren())
		{
			child.QueueFree();
		}

		inventoryButtons.Clear();
		items.Clear();
		for (int i = 0; i < Capacity; i++)
		{
			InventoryButton btn = inventoryButton.Instantiate<InventoryButton>();
			btn.Connect(InventoryButton.SignalName.ConsumeRequest, new Callable(this, nameof(OnConsumeRequest)));
			gridContainer.AddChild(btn);
			inventoryButtons.Add(btn);
			items.Add(null);
		}
	}
	
	private void OnConsumeRequest(int index)
	{
		Consume(index);
	}

	public void Consume(int index)
	{
		if (index < 0 || index >= Capacity)
		{
			GD.PrintErr("Invalid index for consumption.");
			return;
		}

		Item item = items[index];
		if (item == null)
		{
			GD.Print("No item in this slot.");
			return;
		}
		
		var invchat = item.Use(Client.Instance.CurrentPlayer);
		Chat = invchat;

		if (Hud.Instance != null)
		{
			Hud.Instance.OnInventoryChatUpdated(Chat);
		}
		else
		{
			GD.PrintErr("HUD instance is null!");
		}
		
		GD.Print($"Consumed: {item.Name}");

		if (item.IsStackable)
		{
			item.Quantity -= 1;
			if (item.Quantity <= 0)
				items[index] = null;
		}
		else
		{
			items[index] = null;
		}

		UpdateButton(index);
		Chat = invchat;
		if (Hud.Instance != null)
		{
			Hud.Instance.OnInventoryChatUpdated(Chat);
		}
		else
		{
			GD.PrintErr("HUD instance is null!");
		}
	}


	public void Add(Item item)
	{
		if (!_isInitialized)
		{
			GD.PrintErr("[ERROR] Inventory.Add called before _Ready(). Deferring execution.");
			CallDeferred(nameof(Add), item); // Retry after _Ready() completes
			return;
		}
		
		Item toAdd = item.Duplicate(true) as Item;
		if (toAdd == null)
		{
			GD.PrintErr("Failed to duplicate item for adding.");
			return;
		}
	
		if (toAdd.IsStackable)
		{
			for (int i = 0; i < Capacity && toAdd.Quantity > 0; i++)
			{
				var existing = items[i];
				if (existing != null 
					&& existing.ID == toAdd.ID 
					&& existing.Quantity < existing.StackSize)
				{
					int spaceLeft = existing.StackSize - existing.Quantity;
					int transfer = Math.Min(spaceLeft, toAdd.Quantity);

					existing.Quantity += transfer;
					toAdd.Quantity   -= transfer;
					UpdateButton(i);
					if (toAdd.Quantity == 0)
						return;
				}
			}
		}
	
		for (int i = 0; i < Capacity && toAdd.Quantity > 0; i++)
		{
			if (items[i] == null)
			{
				int placing = toAdd.IsStackable
					? Math.Min(toAdd.Quantity, toAdd.StackSize)
					: 1;
				var newStack = (item.Duplicate(true) as Item);
				newStack.Quantity = placing;

				items[i] = newStack;
				UpdateButton(i);

				toAdd.Quantity -= placing;
			}
		}

		if (toAdd.Quantity > 0)
		{
			GD.Print("Not enough space to add all items; leftover: " + toAdd.Quantity);
		}
		
		Chat = $"Picked up: {item.Name}";
		if (Hud.Instance != null)
		{
			Hud.Instance.OnInventoryChatUpdated(Chat);
		}
		else
		{
			GD.PrintErr("HUD instance is null!");
		}
	}


	public void UpdateButton(int index)
	{
		if (index < 0 || index >= inventoryButtons.Count)
		{
			GD.PrintErr($"UpdateButton: Invalid index {index}, buttons count: {inventoryButtons.Count}");
			return;
		}
		inventoryButtons[index].UpdateItem(items[index], index);
	}


	public void _on_add_item_button_button_down()
	{
		var itemResource = ResourceLoader.Load("res://Scene/Item/MoelleOsseuse.tres");
		if (itemResource is Item item)
		{
			Add(item);
		}
		else
		{
			GD.PrintErr($"Failed to load item resource from path: {_testItemPath}");
		}
	}
	
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (Input.IsActionJustPressed("Inventory"))
		{
			if (Visible)
			{
				_appearTween = TweenUtils.RemakeTransitionTween(CreateTween, _appearTween);
				
				Modulate = new Color(1, 1, 1, 1);
				_appearTween.TweenProperty(
					this, 
					"modulate", 
					Color.Color8(1, 1, 1, 0),
					0.1f);
				_appearTween.Finished += Hide;
			}
			else
			{
				_appearTween = TweenUtils.RemakeTransitionTween(CreateTween, _appearTween);
				
				Show();
				Modulate = new Color(1, 1, 1, 0);
				_appearTween.TweenProperty(
					this, 
					"modulate", 
					Color.Color8(255, 255, 255, 255),
					0.1f);
			}
		}
	}
}

