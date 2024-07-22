using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using UnityEngine;
using Test;

namespace Table
{
    public struct TestOperationData
    {
        public int day;
        public int assignStrategy;
        public int exploreStrategy;
        public int storylineStrategy;
        public int prepareExplore;
        public List<List<int>> resourceAlloc;
        public List<List<int>> exploreOptions;
        public List<List<int>> storylineOptions;
    }

    public static class TestOperationTable
    {
        public static List<TestOperationData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("testOperation.csv");
            foreach (var row in rawTable.data)
            {
                TestOperationData data = new();
                data.day = Convert.ToInt32(rawTable.Get("day", row));
                data.assignStrategy = Convert.ToInt32(rawTable.Get("assignStrategy", row));
                data.exploreStrategy = Convert.ToInt32(rawTable.Get("exploreStrategy", row));
                data.storylineStrategy = Convert.ToInt32(rawTable.Get("storylineStrategy", row));
                data.prepareExplore = Convert.ToInt32(rawTable.Get("prepareExplore", row));
                data.resourceAlloc = rawTable.GetList2<int>("resourceAlloc", row);
                data.exploreOptions = rawTable.GetList2<int>("exploreOptions", row);
                data.storylineOptions = rawTable.GetList2<int>("storylineOptions", row);
                datas.Add(data);
            }
        }
    }

    public struct TestCheckData
    {
        public int day;
    }

    public static class TestCheckTable
    {
        public static List<TestCheckData> datas = new();

        //读csv初始化数据
        public static void Init()
        {
            if (datas.Any())
                return;
            RawTable rawTable = GameUtil.ReadCsvTable("testCheck.csv");
            foreach (var row in rawTable.data)
            {
                TestCheckData data = new();
                datas.Add(data);
            }
        }
    }
}
