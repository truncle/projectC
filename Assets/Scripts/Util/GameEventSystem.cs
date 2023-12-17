using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Util
{
    public enum UIEvent
    {
        TEST,
    }
    public class GameEventSystem
    {
        private static GameEventSystem instance;

        public static GameEventSystem Instance
        {
            get
            {
                instance ??= new GameEventSystem();
                return instance;
            }
        }

        // 声明一个委托类型，用于作为事件的数据类型
        public delegate void UIEventHandler(UIEvent ue);

        // 声明事件
        public event UIEventHandler UIEvent;

        // 触发事件的方法
        public void SendUIEvent(string operation)
        {
            // 检查是否有事件订阅者，若有则触发事件
            UIEvent?.Invoke((UIEvent)Enum.Parse(typeof(UIEvent), operation));
        }
    }
}
