using Script.Utils;

public static class FileHandler
{
    public static void EntityDtoToEntity(Entity entity, EntityDto entityDto) {
        entity.entityName = entityDto.entityName;
        entity.currentHp = entityDto.currentHp;
        entity.maxHp = entityDto.maxHp;
        entity.isCharacter = entityDto.isCharacter;
        entity.position = entityDto.position;
        entity.initiativeModifier = entityDto.initiativeModifier; 
    }

    public static EntityDto EntityToEntityDto(Entity entity) {
        return new EntityDto
        {
            entityName = entity.entityName,
            currentHp = entity.currentHp,
            maxHp = entity.maxHp,
            isCharacter = entity.isCharacter,
            position = entity.position,
            initiativeModifier = entity.initiativeModifier
        };
    }
}
