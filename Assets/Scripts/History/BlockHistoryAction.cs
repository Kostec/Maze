using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.History
{
    public class BlockHistoryAction : HistoryAction
    {
        public Vector3 InitalPosition { get; }
        public ActionType Action { get; }
        public Vector3 ActionDirection { get; }
        public BlockHistoryAction(Vector3 blockPosition, ActionType blockAction, Vector3 actionDirection)
        {
            this.InitalPosition = blockPosition;
            this.Action = blockAction;
            this.ActionDirection = actionDirection; ;
        }
        public HistoryAction Reverse()
        {
            return new BlockHistoryAction(InitalPosition, Action, -ActionDirection);
        }
    }
}
