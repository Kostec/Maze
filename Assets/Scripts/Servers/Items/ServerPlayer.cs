using Assets.Scripts.Servers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Servers.Items
{
    public class ServerPlayer : IPlayer
    {
        static uint counter = 0;

        public uint Id { get; }

        public string Name { get; }

        public PlayerType Type { get; }

        public Vector3 InitalPosition { get; }

        public bool isActive { get; set; }

        public ServerPlayer(string name, PlayerType type, Vector3 initalPosition)
        {
            Id = counter++;
            Name = name;
            Type = type;
            InitalPosition = initalPosition;
            isActive = false;
        }
    }
}
