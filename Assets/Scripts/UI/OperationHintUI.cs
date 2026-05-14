using UnityEngine;

public class OperationHintUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject hintPanel;

    [Header("Input")]
    public KeyCode toggleKey = KeyCode.I;

    [Header("Start State")]
    public bool showOnStart = true;

    private bool isVisible = true;

    private void Start()
    {
        SetVisible(showOnStart);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleHint();
        }
    }

    public void ToggleHint()
    {
        SetVisible(!isVisible);
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;

        if (hintPanel != null)
        {
            hintPanel.SetActive(isVisible);
        }
    }
}
