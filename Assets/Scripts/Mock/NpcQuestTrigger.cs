using Assets.Scripts.Interactable;
using Assets.Scripts.QuestSystem;
using Assets.Scripts.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.Mock
{
    public class NpcQuestTrigger : MonoBehaviour
    {
        [SerializeField] AInteractable _interactable;
        [SerializeField] QuestData _questData;

        public void Start()
        {
            _interactable.OnInteract += CheckQuest;
        }

        void CheckQuest()
        {

            if(_questData.Quest.IsCompleted)
            {
                Debug.Log("Quest ja completada");
                return;
            }

            if(_questData.Quest.CanBeCompleted())
            {
                _questData.Quest.Complete();
                Debug.Log("Deu certo");
            }
            else
            {
                Debug.Log("Quest nao completa");
            }
        }
    }
}