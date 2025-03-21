using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedGame.Classes.objects
{
    public class Obstacle : WorldObject
    {
        public Obstacle(int x, int y, string name)
            : base(x, y, name, false, false)  // cannot be looted or removed
        {
        }

        public override void Interact()
        {
            Console.WriteLine($"{Name} is an obstacle and cannot be moved.");
        }
    }

}
