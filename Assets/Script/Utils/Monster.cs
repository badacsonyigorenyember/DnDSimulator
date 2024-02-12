public class Monster
{
    public string Name;
    public int MaxHp;
    public int CurrentHp;

    public Monster(MonsterObj obj) {
        Name = obj.Name;
        MaxHp = CurrentHp = obj.GetHp;
    }
}
