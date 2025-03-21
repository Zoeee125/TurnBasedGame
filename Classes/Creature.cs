using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnBasedGame.Classes.objects;

namespace TurnBasedGame.Classes
{
    internal class Creature
    {
        string? name;
        int lifePoints;
        int damagePoints;
        int defencePoints;
        List<WorldObject> inventory;
        
        public Creature() { }
        

        int Attack()
        {
            return 0;
        }
        void RecieveHit(int hit)
        {
            int damageTaken = hit - defencePoints;
            if (damageTaken > 0)
            {
                lifePoints = lifePoints - damageTaken;
            }
        }
        void Pick(WorldObject obj)
        {
            inventory.Add(obj);
        }
    }
}
