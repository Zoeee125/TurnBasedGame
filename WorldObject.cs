using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnBasedGame
{
    internal class WorldObject
    {
        String? name;
        bool lootable;
        bool removeable;
    }
    internal class DefenceItem
    {
        String? name;
        int reduceHitPoint;
    }
    internal class AttackItem
    {
        String? name;
        int hit;
        int range;
    }
}
