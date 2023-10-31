using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Main : Node2D
{
	[Export] public Texture2D NoteNormalTexture;
	[Export] public Texture2D NoteCenterTexture;
	[Export] public Array<Texture2D> NoteContinuouslyPressTextureList;

	[Export] public Texture2D HitPerfectTexture;
	[Export] public Texture2D HitGoodTexture;
	[Export] public Texture2D HitNormalTexture;
	[Export] public Texture2D HitMissTexture;

	[Export] public PackedScene HitNoteScene;
	[Export] public PackedScene HitScoreScene;
	[Export] public PackedScene BarLineScene;

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
	public int Combo
	{
		get => _combo;
		private set
		{
			_combo = value;
			EmitSignal(SignalName.OnComboChanged);
		}
	}
	[Signal] public delegate void OnComboChangedEventHandler();

	float _bpm = 126.0f;
	public float Bpm { get => _bpm; }


	float _pxPerBeat = 100.0f;
	public float PxPerBeat { get => _pxPerBeat; }

	/// <summary>
	/// px/seconds
	/// </summary>
	public float NoteSpeed
	{
		get
		{
			float beatsPerSeconds = _bpm / 60.0f;
			float secondsPerBeat = 1.0f / beatsPerSeconds;
			return _pxPerBeat / secondsPerBeat;
		}
	}

	(float Top, float Bottom) _hitArea = new(-100, 100);

	const int _hitPerfectError = 10;
	const int _hitPerfectScore = 300;
	const int _hitGoodError = 30;
	const int _hitGoodScore = 100;
	const int _hitNormalError = 100;
	const int _hitNormalScore = 50;


	Node2D _trackLeft;
	Node2D _trackRight;
	Node2D _trackCenter;
	Node2D _trackContinuouslyPress;
	Node2D _hitScoreNode;
	Node2D _barLinesNode;
	AudioStreamPlayer _backgroundMusicNode;

	public override void _Ready()
	{
		_trackLeft = GetNode<Node2D>("Game/TrackList/TrackLeft");
		_trackRight = GetNode<Node2D>("Game/TrackList/TrackRight");
		_trackCenter = GetNode<Node2D>("Game/TrackList/TrackCenter");
		_trackContinuouslyPress = GetNode<Node2D>("Game/TrackList/TrackContinuouslyPress");
		_hitScoreNode = GetNode<Node2D>("Game/HitScore");
		_barLinesNode = GetNode<Node2D>("Game/BarLines");
		_backgroundMusicNode = GetNode<AudioStreamPlayer>("BackgroundMusic");
	}

	public override void _Process(double delta)
	{
		if (!IsPlaying && Input.IsActionJustPressed("key_space"))
		{
			StartGame();
		}

		if (IsPlaying)
		{
			HandleHit();
			MoveNotes(delta);
			MoveBarLines(delta);
		}
	}

	void MoveBarLines(double delta)
	{
		_barLinesNode.Position = _barLinesNode.Position with
		{
			Y = _barLinesNode.Position.Y + NoteSpeed * (float)delta,
		};
	}

	void GenerateHitScoreSprite(Texture2D texture, Vector2 position)
	{
		Node2D hitScoreSprite = HitScoreScene.Instantiate<Node2D>();
		hitScoreSprite.GetNode<Sprite2D>("Sprite2D").Texture = texture;
		hitScoreSprite.Position = position;
		_hitScoreNode.AddChild(hitScoreSprite);
	}

	void HandleHitTrack(Node2D track, string actionName)
	{
		
		for (int i = 0; i < track.GetChildCount(); i++)
		{
			HitNote note = track.GetChild<HitNote>(i);
			if (note.IsQueuedForDeletion()) continue;
			if (note.IsDisappearing)
			{
				continue;
			}
			if (note.Position.Y >= _hitArea.Bottom)
			{
				// Miss
				Combo = 0;
				GenerateHitScoreSprite(HitMissTexture, note.GlobalPosition);
				note.PlayMissAnimation();
			}
			else
			{
				bool isInHitArea = note.Position.Y >= _hitArea.Top;
				if (isInHitArea && Input.IsActionJustPressed(actionName))
				{
					int error = (int)Mathf.Abs(note.Position.Y);
					int hitScore = error switch
					{
						<= _hitPerfectError => _hitPerfectScore,
						<= _hitGoodError => _hitGoodScore,
						<= _hitNormalError => _hitNormalScore,
						_ => _hitNormalScore,
					};

					note.PlayHitAnimation();

					Score += hitScore;
					Combo += 1;

					Texture2D hitScoreImage = hitScore switch
					{
						_hitPerfectScore => HitPerfectTexture,
						_hitGoodScore => HitGoodTexture,
						_hitNormalScore => HitNormalTexture,
						_ => HitNormalTexture
					};

					GenerateHitScoreSprite(hitScoreImage, note.GlobalPosition);
				}
			}
		}
	}

	void HandleHit()
	{
		HandleHitTrack(_trackLeft, "key_left");
		HandleHitTrack(_trackRight, "key_right");
		HandleHitTrack(_trackCenter, "key_space");
	}

	void MoveTrackNote(Node2D track, double delta)
	{
		for (int i = 0; i < track.GetChildCount(); i++)
		{
			HitNote note = track.GetChild<HitNote>(i);
			if (note.IsQueuedForDeletion()) continue;
			if (note.IsDisappearing) continue;
			note.Position = note.Position with
			{
				Y = note.Position.Y + NoteSpeed * (float)delta
			};
		}
	}

	void MoveNotes(double delta)
	{
		MoveTrackNote(_trackLeft, delta);
		MoveTrackNote(_trackRight, delta);
		MoveTrackNote(_trackCenter, delta);
	}

	void GenerateBarLines()
	{
		float songDuration = (float)_backgroundMusicNode.Stream.GetLength();
		int beats = (int)Mathf.Ceil(songDuration * (_bpm / 60.0f));
		int bars = beats / 4;

		for (int i = 0; i < bars; i++)
		{
			Sprite2D barLineNode = BarLineScene.Instantiate<Sprite2D>();
			barLineNode.Position = new Vector2(0, (-i - 2) * _pxPerBeat * 4);
			_barLinesNode.AddChild(barLineNode);
		}
	}

	public void StartGame()
	{
		if (IsPlaying)
		{
			return;
		}

		IsPlaying = true;

		GetNode<TextureRect>("BackUI/TextStart").Visible = false;
		_backgroundMusicNode.Play();

		GenerateBarLines();
	}
	public void ResetGame()
	{
		GetNode<TextureRect>("BackUI/TextStart").Visible = true;
		AudioStreamPlayer audioStreamPlayer = GetNode<AudioStreamPlayer>("BackgroundMusic");
		audioStreamPlayer.Stop();
		audioStreamPlayer.Seek(0);

		IsPlaying = false;
		IsContinuouslyNote = false;
		GetTree().CallGroup("playing_items", "queue_free");
	}
}

