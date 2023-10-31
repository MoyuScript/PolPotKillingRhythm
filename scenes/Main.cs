using Godot;
using System;

public partial class Main : Node2D
{
	bool _isPlaying;
	public bool IsPlaying { get => _isPlaying; }
	[Signal] public delegate void OnIsPlayingChangedEventHandler();

	bool _isContinuouslyNote;
	public bool IsContinuouslyNote { get => _isContinuouslyNote; }
	[Signal] public delegate void OnIsContinuouslyNoteChangedEventHandler();

	public override void _Process(double delta)
	{
		if (!_isPlaying && Input.IsActionJustPressed("key_space"))
		{
			StartGame();
		}
	}

	public void StartGame()
	{
		if (_isPlaying)
		{
			return;
		}

		_isPlaying = true;
		EmitSignal(SignalName.OnIsPlayingChanged);
		GetNode<TextureRect>("BackUI/TextStart").Visible = false;
		GetNode<AudioStreamPlayer>("BackgroundMusic").Play();
	}
	public void ResetGame()
	{
		GetNode<TextureRect>("BackUI/TextStart").Visible = true;
		AudioStreamPlayer audioStreamPlayer = GetNode<AudioStreamPlayer>("BackgroundMusic");
		audioStreamPlayer.Stop();
		audioStreamPlayer.Seek(0);

		_isPlaying = false;
		EmitSignal(SignalName.OnIsPlayingChanged);
		_isContinuouslyNote = false;
		EmitSignal(SignalName.OnIsContinuouslyNoteChanged);
	}
}

