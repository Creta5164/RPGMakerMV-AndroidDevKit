using System;
using System.IO;
using Newtonsoft.Json;

namespace ApplicationPackager
{
    public enum OptionState {
        OK,
        NoRPGMVExist,
        NoAssetsExist
    }
    
    /// <summary>
    /// Builder option JSON file.
    /// </summary>
    public sealed class Option
    {
        public const string CONFIGFILE = "packager-config.json";
        
        /// <summary>
        /// ignore files list
        /// </summary>
        [JsonProperty("ignore-files")]
        public string[] IgnoreFiles
         = new string[] {
            "Game.rpgproject",
            "write here files you want.json",
            "to exclude from the.ogg",
            "file being packaged.png",
            "like this.txt"
        };
        
        /// <summary>
        /// ignore folders list
        /// </summary>
        [JsonProperty("ignore-folders")]
        public string[] IgnoreFolders
         = new string[] {
            "save",
            "write here folders you want",
            "to exclude from the",
            "file being packaged",
            "like this."
        };
        
        /// <summary>
        /// original source path
        /// </summary>
        [JsonProperty("rpgmv-path")]
        public string RPGMVPath = "MV";
        
        /// <summary>
        /// publish path
        /// </summary>
        [JsonProperty("assets-path")]
        public string AssetsPath = "Android\\app\\src\\main\\assets";
        
        /// <summary>
        /// logging path
        /// </summary>
        [JsonProperty("build-log-path")]
        public string LogPath = "build-log";
        
        public static Option Load()
        {
            Option option = null;
            if (File.Exists(CONFIGFILE))
                option = JsonConvert.DeserializeObject<Option>(File.ReadAllText(CONFIGFILE));
            else {
                option = new Option();
                
                if (!Directory.Exists(option.LogPath))
                    Directory.CreateDirectory(option.LogPath);
                
                File.WriteAllText(CONFIGFILE, JsonConvert.SerializeObject(option, Formatting.Indented));
            }
            
            return option;
        }
        
        internal OptionState IsValid()
        {
            if (!Directory.Exists(RPGMVPath))
                return OptionState.NoRPGMVExist;
            
            if (!Directory.Exists(AssetsPath))
                return OptionState.NoAssetsExist;
            
            return OptionState.OK;
        }
    }
}