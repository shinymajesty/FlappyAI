using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

namespace Game
{
    public class FileDAO<T> where T : class
    {
        private readonly string _directory;

        public FileDAO(string directory)
        {
            _directory = directory;
        }

        private string GetFilePath(string filename) => _directory;
        

        public void Save(string filename, T data)
        {
            string path = GetFilePath(filename);
            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public T? Load(string filename)
        {
            string path = GetFilePath(filename);
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public bool Delete(string filename)
        {
            string path = GetFilePath(filename);
            if (!File.Exists(path))
                return false;

            File.Delete(path);
            return true;
        }

        public List<string> ListFiles()
        {
            var files = Directory.GetFiles(_directory);
            var filenames = new List<string>();
            foreach (var file in files)
            {
                filenames.Add(Path.GetFileName(file));
            }
            return filenames;
        }

        public bool Exists(string filename)
        {
            return File.Exists(GetFilePath(filename));
        }
    }
}
