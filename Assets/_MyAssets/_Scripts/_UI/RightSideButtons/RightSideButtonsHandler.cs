using UnityEngine;
using UnityEngine.UI;

public class RightSideButtonsHandler : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Transform[] buttonSlots;

    private Transform[] _availableButtonSlots;

    [Header("Buttons")]
    public Button GrabButton;
    public Button DialogueButton;
    public Button ReleaseButton;
    public Button RecycleBinButton;
    public Button WasteBinButton;

    private void Awake()
    {
        _availableButtonSlots = (Transform[])buttonSlots.Clone();

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
        }
        else
        {
            Debug.LogWarning("No available button positions left!");
        }
    }

    void DisableButton(Button button)
    {
        if (!button.IsActive()) return;

        FreePosition(button);
        button.gameObject.SetActive(false);
    }

    #endregion

    #region Button Slots
    void FreePosition(Button button)
    {
        Transform parentSlot = button.transform.parent;

        for (int i = 0; i < buttonSlots.Length; i++)
        {
            if (buttonSlots[i] == parentSlot)
            {
                _availableButtonSlots[i] = buttonSlots[i];
                break;
            }
        }
    }

    Transform GetNextEmptyPosition()
    {
        for (int i = 0; i < _availableButtonSlots.Length; i++)
        {
            if (_availableButtonSlots[i] != null)
            {
                Transform slot = _availableButtonSlots[i];
                _availableButtonSlots[i] = null;
                return slot;
            }
        }
        return null;
    }
    #endregion


}

