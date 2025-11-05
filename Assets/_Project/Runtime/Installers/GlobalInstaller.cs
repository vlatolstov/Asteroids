using _Project.Runtime.SceneManagement;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "GlobalInstaller", menuName = "Installers/Global Installer")]
    public class GlobalInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<SceneLoader>()
                .AsSingle();
        }
    }
}