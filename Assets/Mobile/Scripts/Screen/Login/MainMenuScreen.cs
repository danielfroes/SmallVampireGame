using Assets.Scripts.Screens;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Screen.MainMenu
{ 
    public class MainMenuScreen : AScreen
    {
        [SerializeField] Button _playButton;
        [SerializeField, Scene] int _mainSceneIndex; 

        public override void OnShow(object[] args)
        {
            _playButton.onClick.AddListener(GoToMainScene);
        }

        void GoToMainScene()
        {
            SceneManager.LoadSceneAsync(_mainSceneIndex, LoadSceneMode.Additive);
            CloseScreen();
        }

        public override void OnHide()
        {
            base.OnHide();
        }
    }

}