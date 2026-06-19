using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerInputHandler.OnA_ButtonPressed += HandleA_ButtonPressed;
       PlayerInputHandler.OnB_ButtonPressed += HandleB_ButtonPressed;
       PlayerInputHandler.OnC_ButtonPressed += HandleC_ButtonPressed;
       PlayerInputHandler.OnD_ButtonPressed += HandleD_ButtonPressed;
    }
    private void OnDisable()
    {
        PlayerInputHandler.OnA_ButtonPressed -= HandleA_ButtonPressed;
       PlayerInputHandler.OnB_ButtonPressed -= HandleB_ButtonPressed;
       PlayerInputHandler.OnC_ButtonPressed -= HandleC_ButtonPressed;
       PlayerInputHandler.OnD_ButtonPressed -= HandleD_ButtonPressed;
    }

    private void HandleA_ButtonPressed()
    {
       // Debug.Log("A Button Pressed");
    }

    private void HandleB_ButtonPressed()
    {
        Debug.Log("B Button Pressed");
    }

    private void HandleC_ButtonPressed()
    {
        Debug.Log("C Button Pressed");
    }

    private void HandleD_ButtonPressed()
    {
        Debug.Log("D Button Pressed");
    }
}
