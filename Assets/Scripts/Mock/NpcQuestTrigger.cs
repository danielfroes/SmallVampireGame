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
        }

        [YarnCommand("completeQuest")]
        public void CompleteQuest()
        {
            _questData.Quest.Complete();
        }

        [YarnFunction("isQuestCompleted")]
        public static bool IsQuestComplete()
        {
            return _questData.Quest.IsCompleted;
        }

        [YarnFunction("canCompleteQuest")]
        public static bool CanCompleteQuest()
        {
            return _questData.Quest.CanBeCompleted();
        }

    }
}