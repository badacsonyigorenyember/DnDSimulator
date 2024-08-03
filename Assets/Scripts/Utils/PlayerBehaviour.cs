using System;
using Models;
using Models.Interfaces;
using Utils.Interfaces;

namespace Utils
{
    [Serializable]
    public class PlayerBehaviour : EntityBehaviour
    {
        public void Init(IEntity data) {
            Entity = data as Player;
        }
    }
}