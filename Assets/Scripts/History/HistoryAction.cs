using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.History
{
    public enum ActionType
    {
        Shift,
        Rotation,
        Move
    }

    public interface HistoryAction
    {
        Vector3 InitalPosition { get; }
        ActionType Action { get; }
        Vector3 ActionDirection { get; }
        HistoryAction Reverse();
    }
}
