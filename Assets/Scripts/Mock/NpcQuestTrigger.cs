using Assets.Scripts.Interactable;
using Assets.Scripts.QuestSystem;
using Assets.Scripts.Utils;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Yarn.Unity;

namespace Assets.Scripts.Mock
{
    public class NpcQuestTrigger : MonoBehaviour
    {
        [SerializeField] AInteractable _interactable;
        [SerializeField] QuestData _questData;
        [SerializeField] DialogueRunner _dialogueRunner;

        public void Start()
        {
            _interactable.OnInteract += () => CheckQuest().Forget();
        }

        async UniTaskVoid CheckQuest()
        {
            if (_dialogueRunner.IsDialogueRunning)
                return;

            await UniTask.NextFrame();

            _dialogueRunner.StartDialogue("Teste");

            await UniTask.WaitUntil(() => !_dialogueRunner.IsDialogueRunning);

            if (_questData.Quest.IsCompleted)
            {
                _dialogueRunner.StartDialogue("QuestWasCompleted");
                return;
            }

            if (_questData.Quest.CanBeCompleted())
            {
                _dialogueRunner.StartDialogue("QuestCanBeCompleted");
                _questData.Quest.Complete();
            }
            else
            {
                _dialogueRunner.StartDialogue("QuestCantBeCompleted");
            }
        }

        [YarnCommand("completeQuest")]
        public void CompleteQuest()
        {
            _questData.Quest.Complete();
        }
    }
}