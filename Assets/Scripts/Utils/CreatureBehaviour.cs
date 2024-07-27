using System;
using Models;
using Utils.Data;
using Utils.Interfaces;

namespace Utils
{
    [Serializable]
    public class CreatureBehaviour : EntityBehaviour
    {
        public override void Init(IEntityData data) {
            CreatureData creatureData = data as CreatureData;
            Entity = new Creature(creatureData);
        }
    }
}
