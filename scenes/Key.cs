using Godot;
using System;

public partial class Key : TextureButton
{
	[Export] public string ActionName;

	public override void _Ready()
	{
		var mainNode = GetNode<Main>("/root/Main");
		mainNode.OnIsContinuouslyNoteChanged += () => {
			if (mainNode.IsContinuouslyNote)
			{
				StartEmphasis();
			}
			else
			{
				StopEmphasis();
			}
		};
	}

	public override void _Process(double delta)
	{
		ButtonPressed = Input.IsActionPressed(ActionName);
	}

	void StartEmphasis()
	{
		var player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.Play("key_emphasize");
	}

	void StopEmphasis()
	{
		var player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.Stop();
	}
}
