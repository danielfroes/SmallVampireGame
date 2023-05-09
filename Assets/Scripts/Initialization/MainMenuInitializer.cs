using Assets.Scripts.Initialization;
using Assets.Scripts.Screen.MainMenu;
using Assets.Scripts.Screens;
using Assets.Scripts.Utils;

namespace Assets.Scripts.InitializationSteps
{
    public class MainMenuInitializer : IInitializationStep
    {
        public void Run()
        {
            ServiceLocator.Get<ScreenService>().Show<MainMenuScreen>();
        }

        public void Dispose()
        {
        }
    }

}