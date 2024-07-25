namespace FileHandling
{
    public static class CreatureDtoHandler
    {
        public static void CreatureDtoToCreature(Creature creature, CreatureDto creatureDto) {
            creature.creatureName = creatureDto.creatureName;
            creature.currentHp = creatureDto.currentHp;
            creature.maxHp = creatureDto.maxHp;
            creature.isPlayer = creatureDto.isCharacter;
            creature.initiativeModifier = creatureDto.initiativeModifier;

            creature.gameObject.name = creatureDto.creatureName;
            creature.SetVisibleClientRpc(creatureDto.visible);
        }
    }
}
