using System;
using Models;
using Models.Interfaces;
using Utils.Interfaces;

namespace Utils
{
    [Serializable]
    public class CreatureBehaviour : EntityBehaviour
    {
        public void Init(IEntity data) {
            Entity = data as Creature;
        }
    }
}
