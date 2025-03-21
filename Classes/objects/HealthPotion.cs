using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedGame.Classes.objects
{
    public class HealthPotion : WorldObject
    {
        public int HealingAmount { get; set; }

        public HealthPotion(int x, int y, string name, int healingAmount)
            : base(x, y, name, true, true)  // lootable and will be removed after use
        {
            HealingAmount = healingAmount;
        }

        public override void Interact()
        {
            Console.WriteLine($"{Name} heals for {HealingAmount} HP and disappears.");
        }
    }

}
