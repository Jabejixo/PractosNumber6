using System;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;

namespace JsonLib
{
    public class JsonManager
    {


        public static string ConvertToJson<T>(T model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public static T? ConvertToString<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static string ConvertCollectionToJson<T>(ObservableCollection<T> collection)
        {
            return JsonConvert.SerializeObject(collection);
        }

        public static ObservableCollection<T>? ConvertToCollection<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<ObservableCollection<T>>(jsonString);
        }


        public static void Serialize<T>(ObservableCollection<T> list, string fileName)
        {
            if (!File.Exists($"{fileName}.json"))
            {
                File.Create($"{fileName}.json");
                string json = JsonConvert.SerializeObject(list);
                File.WriteAllText($"{fileName}.json", $"\r\n{json}");
            }
            else
            {
                string json = JsonConvert.SerializeObject(list);
                File.WriteAllText($"{fileName}.json", $"\r\n{json}");
            }
        }


        public static ObservableCollection<T> Deserialization<T>(string fileName)
        {
            ObservableCollection<T> collection;
            if (!File.Exists($"{fileName}.json"))
            {
                File.Create($"{fileName}.json");
                return collection = new ObservableCollection<T>();
            }
            else
            {
                string info = File.ReadAllText($"{fileName}.json");
                collection = JsonConvert.DeserializeObject<ObservableCollection<T>>(info);
                return collection;
            }
        }
    }
}
