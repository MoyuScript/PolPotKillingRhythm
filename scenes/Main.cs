using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Main : Node2D
{
	[Export] public Texture2D NoteNormalTexture;
	[Export] public Texture2D NoteCenterTexture;

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

	const float _bpm = 126.0f;
	const int _ppq = 96;
	public float Bpm { get => _bpm; }


	float _pxPerBeat = 300.0f;
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
	Node2D _hitScoreNode;
	Node2D _barLinesNode;
	Label _endScoreLabelNode;
	AudioStreamPlayer _backgroundMusicNode;

	public override void _Ready()
	{
		_trackLeft = GetNode<Node2D>("Game/TrackList/TrackLeft");
		_trackRight = GetNode<Node2D>("Game/TrackList/TrackRight");
		_trackCenter = GetNode<Node2D>("Game/TrackList/TrackCenter");
		_hitScoreNode = GetNode<Node2D>("Game/HitScore");
		_barLinesNode = GetNode<Node2D>("Game/BarLines");
		_backgroundMusicNode = GetNode<AudioStreamPlayer>("BackgroundMusic");
		_endScoreLabelNode = GetNode<Label>("FrontUI/StatBoard/EndScoreLabel");

		_backgroundMusicNode.Finished += OnGameEnd;
	}

	void OnGameEnd()
	{
		_endScoreLabelNode.Visible = true;
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
				GetNode<AudioStreamPlayer>("Miss").Play();
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

					Combo += 1;
					Score += hitScore * Combo;

					Texture2D hitScoreImage = hitScore switch
					{
						_hitPerfectScore => HitPerfectTexture,
						_hitGoodScore => HitGoodTexture,
						_hitNormalScore => HitNormalTexture,
						_ => HitNormalTexture
					};

					GenerateHitScoreSprite(hitScoreImage, note.GlobalPosition);
					break;
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

	void GenerateNotes()
	{
		foreach (string row in Constant.Map.Split("\n"))
		{
			var col = row.Split(",");
			int tick;

			try
			{
				tick = Convert.ToInt32(col[0].Trim());
			}
			catch
			{
				continue;
			}

			string type = col[1].Trim();
			Vector2 position = new Vector2(0, -((float)tick / _ppq) * _pxPerBeat);
			switch (type)
			{
				case "L":
					{
						
						HitNote note = HitNoteScene.Instantiate<HitNote>();
						note.GetNode<Sprite2D>("Sprite2D").Texture = NoteNormalTexture;
						note.Position = position;
						_trackLeft.AddChild(note);
						break;
					}
				case "R":
					{
						HitNote note = HitNoteScene.Instantiate<HitNote>();
						note.GetNode<Sprite2D>("Sprite2D").Texture = NoteNormalTexture;
						note.Position = position;
						_trackRight.AddChild(note);
						break;
					}
				case "S":
					{
						// center note
						HitNote note = HitNoteScene.Instantiate<HitNote>();
						note.GetNode<Sprite2D>("Sprite2D").Texture = NoteCenterTexture;
						note.Position = position;
						_trackCenter.AddChild(note);
						break;
					}
				default:
					break;
			}
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

		GenerateBarLines();
		GenerateNotes();
		_backgroundMusicNode.Play();
	}
	public void ResetGame()
	{
		GetNode<TextureRect>("BackUI/TextStart").Visible = true;
		AudioStreamPlayer audioStreamPlayer = GetNode<AudioStreamPlayer>("BackgroundMusic");
		audioStreamPlayer.Stop();
		audioStreamPlayer.Seek(0);

		IsPlaying = false;
		IsContinuouslyNote = false;
		_endScoreLabelNode.Visible = false;
		_barLinesNode.Position = new Vector2(940, 711);
		Score = 0;
		Combo = 0;
		GetTree().CallGroup("playing_items", "queue_free");
	}
}

