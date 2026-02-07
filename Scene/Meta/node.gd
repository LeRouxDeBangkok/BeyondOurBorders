extends Node

@export var OnClickEvent:WwiseEvent
@export var rtpcVolume:WwiseRTPC
@export var rtpcCombat:WwiseRTPC

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _openPauseMenuEvent() -> void:
	Wwise.set_state("GameStatus", "InMenu")

func _closePauseMenuEvent() -> void:
	Wwise.set_state("GameStatus", "InGame")
	
func _exitPauseMenuEvent() -> void:
	Wwise.set_state("GameStatus", "InGame")
	Wwise.set_state("NPC", "NotTalking")
	Wwise.set_state("PlayerState", "Lobby")
	Wwise.set_state("Puzzle", "NotDoingPuzzle")

func _beginTalkToNpcEvent() -> void:
	Wwise.set_state("NPC", "Talking")
	
func _stopTalkToNpcEvent() -> void:
	Wwise.set_state("NPC", "NotTalking")
	
func _enterPuzzleAreaEvent() -> void:
	Wwise.set_state("Puzzle", "DoingPuzzle")

func _leavePuzzleAreaEvent() -> void:
	Wwise.set_state("Puzzle", "NotDoingPuzzle")

func _changeVolumeValueEvent(value: float) -> void:
	print(value)
	rtpcVolume.set_global_value(value)
	
func _changeCombatValueEvent(value: float) -> void:
	rtpcCombat.set_global_value(value)
	
func _onClickButtonEvent() -> void:
	OnClickEvent.post(self)

func _on_ak_event_2d_music_sync_beat(data: Dictionary) -> void:
	pass # Replace with function body.
