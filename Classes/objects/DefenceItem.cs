using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedGame.Classes.objects
{
    public class DefenceItem : WorldObject
    {
        public int Defense { get; set; }

        public DefenceItem(int x, int y, string name, int defense)
            : base(x, y, name, true, true)  // can be looted and removed
        {
            Defense = defense;
        }

        public override void Interact()
        {
            Console.WriteLine($"{Name} is a defense item with {Defense} defense points.");
        }
    }
}
