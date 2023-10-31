using Godot;
using System;

public partial class StatBoard : Node
{
	int _currentScore = 0;
	public override void _Ready()
	{
		var mainNode = GetNode<Main>("/root/Main");
		var scoreLabelNode = GetNode<Label>("ScoreLabel");
		var comboLabelNode = GetNode<Label>("ComboLabel");

		mainNode.OnScoreChanged += () =>
		{
			var tween = CreateTween();
			tween.TweenMethod(
				Callable.From(
					(int val) =>
					{
						scoreLabelNode.Text = $"分数：{val:D8}";
						_currentScore = val;
					}
				),
				_currentScore,
				mainNode.Score,
				0.5
			);

		};

		mainNode.OnComboChanged += () =>
		{
			comboLabelNode.Text = $"连击：{mainNode.Combo}";
		};
	}
}
