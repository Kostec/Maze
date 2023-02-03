using Assets.Scripts.Servers.Controllers.FieldControllers;
using Assets.Scripts.Servers.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Servers.Interfaces
{
    public struct ShiftResponse
    {
        public bool success;
        public Dictionary<Vector3, IBlock> line;
        public Vector3 toBuffer;
        public Vector3 bufferTo;
    }
    public enum RotateSide
    {
        Left,
        Right
    }
    public struct RotateResponse
    {
        public bool success;
        public int angle;
        public IEnumerable<Vector3> directions;
    }

    /// <summary>
    /// Интерфейс-обёртка над сервером 
    /// Пропускает через себя запрос к реальному серверу
    /// </summary
    public interface IServerWrapper
    {
        static uint MaximumPlayerCount = 1;
        ShiftResponse ShiftLine(IBlock baseBlock, Vector3 direction);
        RotateResponse RotateBlock(IBlock block, RotateSide side);
        bool MovePlayerToTarget(Player player, Vector3 target);
        bool PlayerPutCard(Player player, IFieldItem card);
        Dictionary<Vector3, IBlock> GenerateField(uint width, uint heigth, ShapeMode shapeMode);
        IPlayer CreatePlayer(string name, PlayerType type);
    }
}
