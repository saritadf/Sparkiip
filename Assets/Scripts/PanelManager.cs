using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    /// <summary>
    /// Manages UI panels and responsive layout for the home screen
    /// </summary>
    public class PanelManager : MonoBehaviour
    {
        // UI Document for Home Screen
        [SerializeField]
        private UIDocument _homeScreen;

    private void OnEnable()
    {
        if (_homeScreen != null)
        {
            BindMainMenuScreen();
            AdjustLayoutOnScreenSizeChange();
            RegisterScreenSizeChangeCallback();
        }
        else
        {
            Debug.LogWarning("UIDocument for HomeScreen is not assigned");
        }
    }

    private void BindMainMenuScreen()
    {
        var root = _homeScreen.rootVisualElement;
        var button = root.Q<Button>("button");

        if (button != null)
        {
            button.clicked += () =>
            {
                Debug.Log("Button Clicked");
            };
        }
        else
        {
            Debug.LogWarning("Button not found in the UXML document.");
        }
    }

    private void RegisterScreenSizeChangeCallback()
    {
        var root = _homeScreen.rootVisualElement;
        root.RegisterCallback<GeometryChangedEvent>(_ =>
        {
            AdjustLayoutOnScreenSizeChange();
        });
    }

    private void AdjustLayoutOnScreenSizeChange()
    {
        var root = _homeScreen.rootVisualElement;
        float screenWidth = root.resolvedStyle.width;

        // Apply or remove the small-screen class based on the screen width
        if (screenWidth < 600)
        {
            root.AddToClassList("small-screen");
        }
        else
        {
            root.RemoveFromClassList("small-screen");
        }
    }
    }
}