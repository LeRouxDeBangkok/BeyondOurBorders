extends Area2D

const MyScript = preload("res://Scene/Meta/node.gd")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

func _on_body_entered(body: Node2D) -> void:
	var my_script_instance = MyScript.new()
	my_script_instance._leavePuzzleAreaEvent()
	print("Ok")

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
