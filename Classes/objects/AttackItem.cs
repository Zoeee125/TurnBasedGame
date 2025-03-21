namespace TurnBasedGame.Classes.objects
{
    public class AttackItem : WorldObject
    {
        public int Damage { get; set; }

        public AttackItem(int x, int y, string name, int damage)
            : base(x, y, name, true, true)  // Útočné předměty lze lootnout a odstranit
        {
            Damage = damage;
        }

        public override void Interact()
        {
            Console.WriteLine($"{Name} is an attack item with {Damage} damage.");
        }
    }


}
