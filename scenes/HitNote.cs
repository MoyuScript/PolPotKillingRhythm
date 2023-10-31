using Godot;
using System;

public partial class HitNote : Node2D
{
	AnimationPlayer _player;

	public bool IsDisappearing
	{
		get; private set;
	}

	public override void _Ready()
	{
		_player = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void PlayHitAnimation()
	{
		IsDisappearing = true;
		_player.Play("hit");
	}

	public void PlayMissAnimation()
	{
		IsDisappearing = true;
		_player.Play("miss");
	}
}
