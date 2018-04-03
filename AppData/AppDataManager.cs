using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Orpheus.AppData
{
    class AppDataManager
    {
        private AppDataContent _appDataContent;
        private string _dataFileName = "appData.json";
        public AppDataManager(AppDataContent appDataContent)
        {
            _appDataContent = appDataContent;
        }

        public void GetData()
        {
            var json = FileManager.ReadFrom(_dataFileName);
            if(json != string.Empty)
            {
                ParseJsonData(json);
                return;
            }
            else
            {
                SaveData();
                json = FileManager.ReadFrom(_dataFileName);
                ParseJsonData(json);
            }
        }

        private void ParseJsonData(string json)
        {

            var content = Deserializer.ToObject<AppDataContent>(json);
            _appDataContent.UpdateContextStreams(content.Streams);
        }

        public void SaveData()
        {
            _appDataContent.Streams = _appDataContent.StreamsFromContext;
            var contentStr = Serializer.ToString<AppDataContent>(_appDataContent);
            FileManager.SaveTo(_dataFileName, contentStr);
        }
    }

    public static class FileManager
    {
        public static void SaveTo(string fileName, string content)
        {
            var path = AssemblyPath + @"\" + fileName;

            if (!File.Exists(path))
            {
                File.Create(path);
            }
            else
            {
                File.WriteAllText(path, content);
            }
        }

        public static string ReadFrom(string fileName)
        {
            var path = AssemblyPath + @"\" + fileName;
            
            return (File.Exists(path)) ? File.ReadAllText(path) : string.Empty;
        }

        private static string AssemblyPath => System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
    }

    public static class Serializer
    {
        public static string ToString<T>(T obj)
        {
            var jsonData = JsonConvert.SerializeObject(obj);
            return jsonData;
        }
    }

    public static class Deserializer
    {
        public static T ToObject<T>(string json)
        {
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }
    }
}
