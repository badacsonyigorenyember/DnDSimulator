public static class EntityWriterHelper
{
    public static EntityDto EntityToEntityDto(Entity entity) {
        EntityDto edto = new EntityDto(entity.entityName, entity.currentHp, 
            entity.maxHp, entity.isCharacter, entity.position, entity.initiativeModifier);

        return edto;
    } 
    
    public static void EntityDtoToEntity(Entity entity, EntityDto entityDto) {
        entity.entityName = entityDto.entityName;
        entity.currentHp = entityDto.currentHp;
        entity.maxHp = entityDto.maxHp;
        entity.isCharacter = entityDto.isCharacter;
        entity.position = entityDto.position;
        entity.initiativeModifier = entityDto.initiativeModifier;
    } 
}
