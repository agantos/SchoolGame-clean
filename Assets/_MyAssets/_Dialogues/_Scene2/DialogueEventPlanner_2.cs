using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueEventPlanner_2 : DialogueEventPlanner_Base
{
    DialogueManager _dialogueManager;
    PlayerManager _playerManager;

    [SerializeField] Transform margaret;
    [SerializeField] SCR_DialogueNode returnToMargaretDialogue;

    private void Start()
    {
        _dialogueManager = FindAnyObjectByType<DialogueManager>();
        _playerManager = FindAnyObjectByType<PlayerManager>();

        CreateEvent("NoTalk", DialogueEvent.OnDialogueEvent.END_NODE, SpawnReturnToMargaretDialogue);
        CreateEvent("GoTalk", DialogueEvent.OnDialogueEvent.START_NODE, LookAtMargaret);
    }

     async UniTask SpawnReturnToMargaretDialogue()
    {
        await UniTask.Delay(4000);
        _dialogueManager.DialogueToStart = returnToMargaretDialogue;
        _dialogueManager.StartDialogue();

    }

    async UniTask LookAtMargaret()
    {
        await _playerManager.LookAt(margaret, keepFocus: true);
        await _playerManager.ReturnToPlayer();
    }

}
