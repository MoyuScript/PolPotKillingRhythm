using Godot;
using System;

public partial class Main : Node2D
{
	bool _isPlaying;
	public bool IsPlaying
	{
		get => _isPlaying;
		private set
		{
			_isPlaying = value;
			EmitSignal(SignalName.OnIsPlayingChanged);
		}
	}
	[Signal] public delegate void OnIsPlayingChangedEventHandler();

	bool _isContinuouslyNote;
	public bool IsContinuouslyNote
	{
		get => _isContinuouslyNote;
		private set
		{
			_isContinuouslyNote = value;
			EmitSignal(SignalName.OnIsContinuouslyNoteChanged);
		}
	}
	[Signal] public delegate void OnIsContinuouslyNoteChangedEventHandler();

	int _score;
	public int Score
	{
		get => _score;
		private set
		{
			_score = value;
			EmitSignal(SignalName.OnScoreChanged);
		}
	}
	[Signal] public delegate void OnScoreChangedEventHandler();

	int _combo;
	public int Combo {
		get => _combo;
		private set
		{
			_combo = value;
			EmitSignal(SignalName.OnComboChanged);
		}
	}
	[Signal] public delegate void OnComboChangedEventHandler();

	public override void _Process(double delta)
	{
		if (!IsPlaying && Input.IsActionJustPressed("key_space"))
		{
			StartGame();
		}
	}

	public void StartGame()
	{
		if (IsPlaying)
		{
			return;
		}

		IsPlaying = false;

		GetNode<TextureRect>("BackUI/TextStart").Visible = false;
		GetNode<AudioStreamPlayer>("BackgroundMusic").Play();
	}
	public void ResetGame()
	{
		GetNode<TextureRect>("BackUI/TextStart").Visible = true;
		AudioStreamPlayer audioStreamPlayer = GetNode<AudioStreamPlayer>("BackgroundMusic");
		audioStreamPlayer.Stop();
		audioStreamPlayer.Seek(0);

		IsPlaying = false;
		IsContinuouslyNote = false;
	}
}

