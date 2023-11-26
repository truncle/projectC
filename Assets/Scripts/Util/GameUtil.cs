using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Util
{
    public static class GameUtil
    {
        static string tableFolderPath = Application.dataPath + "/CsvTable/";

        public static void Print()
        {
            Debug.Log("Hello");
        }

        public static void GetCondition(string str)
        {
            string pattern = "";
        }

        public static Table.RawTable ReadCsvTable(string filePath)
        {
            string path = tableFolderPath + filePath;
            using StreamReader reader = new(path, Encoding.UTF8);
            List<List<string>> data = new();
            List<string> fields = new(reader.ReadLine().Split(","));
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                List<string> lineData = new(line.Split(","));
            }
            return new Table.RawTable
            {
                fields = fields,
                data = data,
            };
        }
    }

}
