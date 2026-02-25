using UnityEngine;
using UnityEngine.UIElements;

public class LoadingBarView
{
    private VisualElement fill;
    private Label label;

    public LoadingBarView(VisualElement root)
    {
        fill = root.Q<VisualElement>("Fill");
        label = root.Q<Label>("ProgressLabel");
    }

    public void SetProgress(float normalizedValue)
    {
        normalizedValue = Mathf.Clamp01(normalizedValue);

        fill.style.width = Length.Percent(normalizedValue * 100f);
        label.text = $"{Mathf.RoundToInt(normalizedValue * 100f)}%";
    }
}
