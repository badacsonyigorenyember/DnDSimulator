using System;
using Script.Models;
using Script.Utils.Data;
using Script.Utils.Interfaces;

namespace Script.Utils
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