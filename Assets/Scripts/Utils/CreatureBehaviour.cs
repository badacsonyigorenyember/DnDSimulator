using System;
using Models;
using Models.Interfaces;
using Utils.Interfaces;

namespace Utils
{
    [Serializable]
    public class CreatureBehaviour : EntityBehaviour
    {
        public override void Init(IEntity data) {
            Entity = data as Creature;
        }
    }
}
