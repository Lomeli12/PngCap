using System;
using System.IO;
using System.Collections.Generic;

namespace PngCap {
    public class Config {
        readonly string fileName;
        readonly Dictionary<string, object> values;
        
        public Config(string filename) {
            this.fileName = filename;
            values = new Dictionary<string, object>();
            readFile();
        }
        
        public int getInt(string name) {
            var key = "I:" + name;
            return values.ContainsKey(key) ? (int)values[key] : 0;
        }
        
        public void setInt(string name, int value) {
            var key = "I:" + name;
            if (values.ContainsKey(key)) values[key] = value;
            else values.Add(key, value);
            writeFile();
        }
        
        public float getFloat(string name) {
            var key = "F:" + name;
            return values.ContainsKey(key) ? (float)values[key] : 0f;
        }
        
        public void setFloat(string name, float value) {
            var key = "F:" + name;
            if (values.ContainsKey(key)) values[key] = value;
            else values.Add(key, value);
            writeFile();
        }
        
        public bool getBool(string name) {
            var key = "B:" + name;
            return values.ContainsKey(key) ? (bool)values[key] : false;
        }
        
        public void setBool(string name, bool value) {
            var key = "B:" + name;
            if (values.ContainsKey(key)) values[key] = value;
            else values.Add(key, value);
            writeFile();
        }
        
        void writeFile() {
            var lines = new string[values.Count];
            var count = 0;
            foreach (var entry in values) {
                lines[count] = entry.Key + "=" + entry.Value;
                count++;
            }
            File.WriteAllLines(fileName, lines);
        }
        
        void readFile() {
            if (File.Exists(fileName)) {
                var fileLines = File.ReadAllLines(fileName);
                foreach (string line in fileLines) {
                    if (line.Length <= 2 || line.StartsWith("#", StringComparison.CurrentCultureIgnoreCase) ||
                        string.IsNullOrEmpty(line) || !line.Contains("=")) continue;
                    var typeSplit = line.Split(new String[] { ":" }, 2, StringSplitOptions.None);
                    if (typeSplit != null && typeSplit.Length == 2) {
                        var itemSplit = typeSplit[1].Split(new String[] { "=" }, 2, StringSplitOptions.None);
                        var typeChar = typeSplit[0][0];
                        if (itemSplit != null && itemSplit.Length == 2) {
                            var name = itemSplit[0];
                            var value = itemSplit[1];
                            switch (typeChar) {
                                case 'I':
                                    values.Add("I:" + name, int.Parse(value));
                                    break;
                                case 'F':
                                    values.Add("F:" + name, float.Parse(value));
                                    break;
                                case 'B':
                                    values.Add("B:" + name, bool.Parse(value));
                                    break;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
        }
    }
}
