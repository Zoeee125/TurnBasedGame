using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedGame.Classes.objects
{
    public class WorldObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public bool Lootable { get; set; }
        public bool Removable { get; set; }

        public WorldObject(int x, int y, string name, bool lootable, bool removable)
        {
            X = x;
            Y = y;
            Name = name;
            Lootable = lootable;
            Removable = removable;
        }

        public virtual void Interact()
        {
            Console.WriteLine($"{Name} interacts with the world.");
        }
    }

}
