using Godot;

public partial class ProgressIcon : TextureRect
{
	[Export] public float TargetProgress = 0.0f;
	[Export] public TextureProgressBar ProgressBar;
	
	const float _targetOpacity = 0.5f;
	const float _targetScale = 0.9f;
	Color _currentModulate;

	public override void _Ready()
	{
		_currentModulate = Modulate;
	}

	public override void _Process(double delta)
	{
		bool isGteProgress = ProgressBar.Value >= TargetProgress;
		Color nextModulate = _currentModulate with
		{
			A = isGteProgress ? 1 : _targetOpacity
		};

		if (nextModulate != _currentModulate)
		{
			Vector2 scale = isGteProgress ? Vector2.One : new Vector2(_targetScale, _targetScale);
			Tween tween = CreateTween();
			tween.Parallel().TweenProperty(this, "modulate", nextModulate, 0.1);
			tween.Parallel().TweenProperty(this, "scale", scale, 0.1);
			_currentModulate = nextModulate;
		}
	}
}
