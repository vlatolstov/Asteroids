using _Project.Runtime.Services;
using UnityEngine;
using Zenject;

namespace _Project.Runtime.Installers
{
    [CreateAssetMenu(fileName = "MenuInstaller", menuName = "Installers/MenuInstaller")]
    public class MenuInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<MenuLoadingTaskService>()
                .AsSingle()
                .NonLazy();
        }
    }
}