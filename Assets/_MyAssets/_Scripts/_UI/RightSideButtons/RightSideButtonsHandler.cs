using UnityEngine;
using UnityEngine.UI;

public class RightSideButtonsHandler : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform[] buttonSlots;
	[SerializeField] Transform buttonDefaultParent;

    [Header("Buttons")]
    public Button GrabButton;
    public Button DialogueButton;
    public Button ReleaseButton;
    public Button RecycleBinButton;
    public Button WasteBinButton;

    private void Awake()
    {
        GrabButton.gameObject.SetActive(false);
        DialogueButton.gameObject.SetActive(false);
        ReleaseButton.gameObject.SetActive(false);
        RecycleBinButton.gameObject.SetActive(false);
        WasteBinButton.gameObject.SetActive(false);

        DialogueButton.onClick.AddListener(() => { ToggleDialogueButton(false); }); 
    }

    #region Toggle Visibility Button Methods
    public void ToggleGrabButton(bool enable)
    {
        if (enable) EnableButton(GrabButton);
        else DisableButton(GrabButton);
    }

    public void ToggleDialogueButton(bool enable)
    {
        if (enable) EnableButton(DialogueButton);
        else DisableButton(DialogueButton);
    }

    public void ToggleReleaseButton(bool enable)
    {
        if (enable) EnableButton(ReleaseButton);
        else DisableButton(ReleaseButton);
    }

	public void ToggleRecycleButton(bool enable)
	{
		if (enable) EnableButton(RecycleBinButton);
		else DisableButton(RecycleBinButton);
	}

	public void ToggleWasteButton(bool enable)
	{
		if (enable) EnableButton(WasteBinButton);
		else DisableButton(WasteBinButton);
	}

	#endregion

	#region Button Enabling / Disabling Helper Methods

	void EnableButton(Button button)
    {
        if (button.IsActive()) return;

        button.gameObject.SetActive(true);

        Transform targetSlot = GetNextEmptyPosition();
        if (targetSlot != null)
        {
            button.transform.SetParent(targetSlot, false);
            button.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("No available button positions left!");
        }
    }

    void DisableButton(Button button)
    {
        FreePositionOfButton(button);
        button.gameObject.SetActive(false);
    }

    #endregion

    #region Button Slots
    void FreePositionOfButton(Button button)
    {
        button.transform.SetParent(buttonDefaultParent);
    }

    Transform GetNextEmptyPosition()
    {
        for (int i = 0; i < buttonSlots.Length; i++)
        {            
            if(buttonSlots[i].childCount == 0)
            {
                return buttonSlots[i];
            }            
        }
        return null;
    }
    #endregion


}

