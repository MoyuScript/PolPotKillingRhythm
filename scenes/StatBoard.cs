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
		var endScoreLabelNode = GetNode<Label>("EndScoreLabel");

		mainNode.OnScoreChanged += () =>
		{
			var tween = CreateTween();
			tween.TweenMethod(
				Callable.From(
					(int val) =>
					{
						if (val >= 2000000)
						{
							scoreLabelNode.Text = $"分数：{val:D8}（>=200w）";
						}
						else
						{
							scoreLabelNode.Text = $"分数：{val:D8}";
						}
						_currentScore = val;
					}
				),
				_currentScore,
				mainNode.Score,
				0.5
			);

			endScoreLabelNode.Text = mainNode.Score >= 2000000 ? "成功去城市化（分数≥200w）" : "去城市化失败了捏（分数＜200w）";
		};

		mainNode.OnComboChanged += () =>
		{
			comboLabelNode.Text = $"连击：{mainNode.Combo}";
		};
	}
}
