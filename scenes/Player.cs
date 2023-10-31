using Godot;
using System;

public partial class Player : Node2D
{
	AnimationTree _playerTree;
	AnimationTree _swordLeftTree;
	AnimationTree _swordRightTree;

	public override void _Ready()
	{
		_playerTree = GetNode<AnimationTree>("Player/AnimationTree");
		_swordLeftTree = GetNode<AnimationTree>("SwordLeft/AnimationTree");
		_swordRightTree = GetNode<AnimationTree>("SwordRight/AnimationTree");
	}
	public override void _Process(double delta)
	{
		if (
			GetNode<Main>("/root/Main").IsContinuouslyNote
			&& (
				Input.IsActionJustPressed("key_space")
				|| Input.IsActionJustPressed("key_left")
				|| Input.IsActionJustPressed("key_right")
			)
		)
		{
			PlayHitBigAnimation();
		}
		else
		{
			if (Input.IsActionJustPressed("key_space"))
			{
				PlayHitCenterAnimation();
			}
			else
			{
				if (Input.IsActionJustPressed("key_left"))
				{
					PlayHitLeftAnimation();
				}
				if (Input.IsActionJustPressed("key_right"))
				{
					PlayHitRightAnimation();
				}
			}
			
		}
		
	}

	void PlayHitLeftAnimation()
	{
		_playerTree.Set("parameters/HitBlendBig/blend_amount", 0);
		_playerTree.Set("parameters/HitBlend/blend_amount", -1);
		_playerTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_swordLeftTree.Set("parameters/HitBlendBig/blend_amount", 0);
		_swordLeftTree.Set("parameters/HitBlend/blend_amount", 0);
		_swordLeftTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
	}

	void PlayHitRightAnimation()
	{
		_playerTree.Set("parameters/HitBlendBig/blend_amount", 0);
		_playerTree.Set("parameters/HitBlend/blend_amount", 1);
		_playerTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_swordRightTree.Set("parameters/HitBlendBig/blend_amount", 0);
		_swordRightTree.Set("parameters/HitBlend/blend_amount", 0);
		_swordRightTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
	}

	void PlayHitCenterAnimation()
	{
		_playerTree.Set("parameters/HitBlendBig/blend_amount", 0);
		_playerTree.Set("parameters/HitBlend/blend_amount", 0);
		_playerTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_swordLeftTree.Set("parameters/HitBlendBig/blend_amount", 0);
		_swordLeftTree.Set("parameters/HitBlend/blend_amount", 1);
		_swordLeftTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_swordRightTree.Set("parameters/HitBlendBig/blend_amount", 0);
		_swordRightTree.Set("parameters/HitBlend/blend_amount", 1);
		_swordRightTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
	}

	void PlayHitBigAnimation()
	{
		_playerTree.Set("parameters/HitBlendBig/blend_amount", 1);
		_playerTree.Set("parameters/HitBlend/blend_amount", 0);
		_playerTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_swordLeftTree.Set("parameters/HitBlendBig/blend_amount", 1);
		_swordLeftTree.Set("parameters/HitBlend/blend_amount", 0);
		_swordLeftTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_swordRightTree.Set("parameters/HitBlendBig/blend_amount", 1);
		_swordRightTree.Set("parameters/HitBlend/blend_amount", 0);
		_swordRightTree.Set("parameters/OneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
	}
}
