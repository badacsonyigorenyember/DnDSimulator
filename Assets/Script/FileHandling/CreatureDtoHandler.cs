public static class CreatureDtoHandler
{
    public static CreatureDto CreatureToCreatureDto(Creature creature) {
        CreatureDto cdto = new CreatureDto(creature.creatureName, creature.currentHp, 
            creature.maxHp, creature.isPlayer, creature.position, creature.initiativeModifier);

        return cdto;
    } 
    
    public static void CreatureDtoToCreature(Creature creature, CreatureDto creatureDto) {
        creature.creatureName = creatureDto.creatureName;
        creature.currentHp = creatureDto.currentHp;
        creature.maxHp = creatureDto.maxHp;
        creature.isPlayer = creatureDto.isCharacter;
        creature.position = creatureDto.position;
        creature.initiativeModifier = creatureDto.initiativeModifier;
    } 
}
