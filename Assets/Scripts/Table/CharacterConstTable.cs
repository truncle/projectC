using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;

namespace Table
{
    public struct CharacterConstData
    {
        public int id;
        public string name;
        public List<List<int>> statesInitial;
        public List<List<int>> statesLimit;
        public string characterAltas;
        public string characterHead;
        public string characterIcon;
    }

    public static class CharacterConstTable
    {
        public static List<CharacterConstData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("characterConst.csv");
            foreach (var row in rawTable.data)
            {
                CharacterConstData data = new();
                data.id = Convert.ToInt32(rawTable.Get("id", row));
                data.name = rawTable.Get("name", row);
                data.statesInitial = rawTable.GetList2<int>("statesInitial", row);
                data.statesLimit = rawTable.GetList2<int>("statesLimit", row);
                data.characterAltas = rawTable.Get("characterAltas", row);
                data.characterHead = rawTable.Get("characterHead", row);
                data.characterIcon = rawTable.Get("characterIcon", row);
                datas.Add(data);
            }
        }

        public static CharacterConstData Get(int id)
        {
            Init();
            return datas.Where(e => e.id == id).First();
        }
    }
}
