using Script.Utils;

public static class CreatureDtoHandler
{
    public static void CreatureDtoToCreature(CreatureBehaviour creatureBehaviour, CreatureDto creatureDto) {
        creatureBehaviour.creatureName = creatureDto.creatureName;
        creatureBehaviour.currentHp = creatureDto.currentHp;
        creatureBehaviour.maxHp = creatureDto.maxHp;
        creatureBehaviour.isPlayer = creatureDto.isCharacter;
        creatureBehaviour.initiativeModifier = creatureDto.initiativeModifier;

        creatureBehaviour.gameObject.name = creatureDto.creatureName;
        creatureBehaviour.SetVisibleClientRpc(creatureDto.visible);
    }
}
