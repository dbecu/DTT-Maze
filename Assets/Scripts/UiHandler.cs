using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UiHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField widthInput;
    [SerializeField]
    private TMP_InputField heightInput;
    [SerializeField]
    private GridsInitializer initializer;
    [SerializeField]
    private TextMeshProUGUI description;
    [SerializeField]
    private Button startGameBtn;
    [SerializeField]
    private Button generateMazeBtn;
    [SerializeField]
    private int minCell = 3;
    [SerializeField]
    private int maxCell = 300;

    //Linked to start game button
    public void PressedEnterButton()
    {
        // if inputs are int valid
        if (int.TryParse(widthInput.text, out int width) 
            && int.TryParse(heightInput.text, out int height))
        {

            //if inputs are higher than 10
            if (width < minCell || height < minCell)
            {
                description.text = $"Please a value larger or equal to {minCell}";
            } else if (width > maxCell || height > maxCell)
            {
                description.text = $"It would work, but it's far too slow. Keep it under {maxCell} please";
            }
            else
            {
                initializer.SetUpGame(width, height);
            }
        }
        else
        {
            description.text = "Please input something valid";
        }
    }
    public void ActivateButton(bool isActive)
    {
        startGameBtn.interactable = isActive;
        generateMazeBtn.interactable = isActive;
    }

}


