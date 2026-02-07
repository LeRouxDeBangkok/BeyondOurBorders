extends AkEvent2D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

@export var inMenuState: WwiseState
@export var inGameState: WwiseState

func _openPauseMenuEvent() -> void:
	inMenuState.set_value()

func _closePauseMenuEvent() -> void:
	inGameState.set_value()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
