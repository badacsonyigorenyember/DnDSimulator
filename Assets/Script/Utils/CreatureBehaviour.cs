using System;
using Script.Models;
using Script.Utils.Data;
using Script.Utils.Interfaces;

namespace Script.Utils
{
    [Serializable]
    public class CreatureBehaviour : EntityBehaviour
    {
        public override void Init(IEntityData data) {
            CreatureData creatureData = data as CreatureData;
            Entity = new Creature(creatureData, true);
        }
    }
}
