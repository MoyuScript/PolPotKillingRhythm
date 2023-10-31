using Godot;
using System;

public partial class ProgressBar : TextureProgressBar
{
	[Export] public AudioStreamPlayer Audio;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Value = 1.0 - (Audio.GetPlaybackPosition() / Audio.Stream.GetLength());
	}
}
