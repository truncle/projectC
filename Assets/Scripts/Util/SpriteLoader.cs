using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Unity.VisualScripting.FullSerializer;

namespace Util
{
    public static class SpriteLoader
    {
        private static Dictionary<int, Sprite> itemSprites;
        private static void LoadItemSprite()
        {
            ItemTable.Init();
            itemSprites = new();
            foreach (ItemData itemData in ItemTable.datas)
            {
                Sprite sprite = Resources.Load<Sprite>(Path.Combine("Pictures", "Items", itemData.path));
                itemSprites[itemData.id] = sprite;
            }
        }

        public static Sprite GetItemSprite(int itemId)
        {
            if (itemSprites == null)
                LoadItemSprite();
            itemSprites.TryGetValue(itemId, out Sprite sprite);
            //todo add default sprite
            return sprite;
        }
    }
}
