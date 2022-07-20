using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Servers.Interfaces
{
    public enum PlayerType
    {
        NPC,
        Human
    }

    public interface IPlayer
    {
        uint Id { get; }
        string Name { get; }
        PlayerType Type { get; }
        Vector3 InitalPosition { get; }
        bool isActive { get; }
    }
}
