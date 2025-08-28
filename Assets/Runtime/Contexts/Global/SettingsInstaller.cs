using UnityEngine;
using Zenject;

namespace Runtime.Contexts.Global
{
    [CreateAssetMenu(fileName = "SettingsInstaller", menuName = "Settings/Settings Installer")]
    public class SettingsInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
        }
    }
}