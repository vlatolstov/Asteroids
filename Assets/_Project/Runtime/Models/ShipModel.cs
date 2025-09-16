using _Project.Runtime.Abstract.Configs;
using _Project.Runtime.Abstract.MVP;
using _Project.Runtime.Data;

namespace _Project.Runtime.Models
{
    public class ShipModel : BaseModel
    {
        private readonly IWorldConfig _worldConfig;
        private bool _shipInGame;

        public ShipModel(IWorldConfig worldConfig)
        {
            _worldConfig = worldConfig;
        }

        protected override void OnEventPublished(IData eventData)
        {
            switch (eventData)
            {
                case ShipSpawnRequest:
                    if (!_shipInGame)
                    {
                        Publish(new ShipSpawnCommand(_worldConfig.WorldRect.center));
                    }

                    break;
                case ShipDespawnRequest despawn:
                    Publish(new ShipDespawnCommand(despawn.ViewId));
                    _shipInGame = false;
                    break;
                case ShipDestroyed destroyed:
                    Publish(new ShipDespawnCommand(destroyed.ViewId));
                    ChangeData<ShipSpawned>(spawned => new ShipSpawned(false, spawned.ViewId, spawned.Position));
                    _shipInGame = false;
                    break;
                case ShipSpawned info:
                        _shipInGame = info.Status;
                    break;
            }
        }
    }
}