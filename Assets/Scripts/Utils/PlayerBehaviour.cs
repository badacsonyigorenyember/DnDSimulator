using System;
using Models;
using Utils.Data;
using Utils.Interfaces;

namespace Utils
{
    [Serializable]
    public class PlayerBehaviour : EntityBehaviour
    {
        public override void Init(IEntityData data) {
            PlayerData playerData = data as PlayerData;
            Entity = new Player(playerData, true);
        }
    }
}