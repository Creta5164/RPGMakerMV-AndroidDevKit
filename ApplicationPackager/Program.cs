using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace ApplicationPackager
{
    enum LogLevel {
        Info,
        Warn,
        Error
    }
    
    //dotnet build command : dotnet publish -c Release --runtime win-x64
    //other platform needs change win-x64 to support platform RID.
    //https://docs.microsoft.com/ko-kr/dotnet/core/rid-catalog
    
    class Program
    {
        static Option option;
        static string logFileName;
        static bool isLogFileReady;
        
        static bool forceMode;
        static FileStream   stream;
        static StreamWriter logWriter;
        
        static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-f")
                forceMode = true;
            
            option = Option.Load();
            
            if (option == null || option.IsValid() != OptionState.OK) {
                
                string currentDirPath = Directory.GetCurrentDirectory();
                
                ShowLog(LogLevel.Error, "There is an error in the config file.");
                ShowLog(LogLevel.Error, $"Please edit '{Option.CONFIGFILE}' to setup packaging environment.");
                
                switch (option.IsValid())
                {
                    case OptionState.NoAssetsExist:
                        ShowLog(LogLevel.Error, " => Target android's assets folder (assets-path) doesn't exist.");
                        break;
                        
                    case OptionState.NoRPGMVExist:
                        ShowLog(LogLevel.Error, " => Target RPG MV folder (rpgmv-path) doesn't exist.");
                        break;
                }
                
                ShowLog(LogLevel.Error, $"Current working path : {currentDirPath}");
                ShowLog(LogLevel.Error, $"Current working path's directories=========\n{string.Join('\n', Directory.GetDirectories(currentDirPath))}");
                
                if (!forceMode) {
                    
                    ShowLog("Press any key to exit...");
                    Console.Read();
                }
                
                return 1;
            }
            
            InitLog();
            
            for (int ignoreFolderIndex = 0; ignoreFolderIndex < option.IgnoreFolders.Length; ignoreFolderIndex++)
                option.IgnoreFolders[ignoreFolderIndex] = $"{option.RPGMVPath}\\{option.IgnoreFolders[ignoreFolderIndex]}";
            
            ShowLog("This tool helps copying to android assets folder.");
            
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) {
                ShowLog(LogLevel.Warn, "Program already running!");
                FinalizeLog();
                
                if (!forceMode) {
                    
                    ShowLog("Press any key to exit...");
                    Console.Read();
                }
                
                return 2;
            }
            
            if (!forceMode) {
                
                ShowLog("Start packaging in 5 seconds...");
                Thread.Sleep(5000);
            }
                
            long changedFiles = 0,
                 addedFiles = 0,
                 removedFiles = 0;
            
            try {
                string[] allFiles     = Directory.GetFiles(option.RPGMVPath, "*", SearchOption.AllDirectories);
                
                string fullPath;
                string alterPath;
                string alterFilePath;
                string fileName;
                
                ShowLog("Applying changes to package from source...");
                
                if (!Directory.Exists(option.AssetsPath))
                    Directory.CreateDirectory(option.AssetsPath);
                    
                bool IsIgnoreFileOrPath(string path, string name) {
                    
                    int i;
                    
                    for (i = 0; i < option.IgnoreFiles.Length; i++)
                        if (name == option.IgnoreFiles[i]) return true;
                    
                    for (i = 0; i < option.IgnoreFolders.Length; i++)
                        if (path == option.IgnoreFolders[i]) return true;
                        
                    return false;
                }
                
                Array.ForEach(allFiles,
                    filePath => {
                        if (filePath.IndexOf(option.RPGMVPath) < 0) {
                            ShowLog(LogLevel.Warn, $"An error occurred navigating the file path. {filePath}");
                            return;
                        }
                        
                        fileName      = Path.GetFileName(filePath);
                        fullPath      = Path.GetDirectoryName(filePath);
                        
                        if (IsIgnoreFileOrPath(fullPath, fileName)) return;
                        
                        fullPath      = fullPath.Split(option.RPGMVPath)[1];
                        alterPath     = $"{option.AssetsPath}{fullPath}";
                        alterFilePath = $"{alterPath}\\{fileName}";
                        
                        string test = Path.GetDirectoryName(filePath);
                        
                        if (!Directory.Exists(alterPath)) {
                            ShowLog($"'{fullPath}' is generated.");
                            
                            Directory.CreateDirectory(alterPath);
                        }
                        
                        if (!File.Exists(alterFilePath)) {
                            ShowLog($"'{fileName}' is generated.");
                            addedFiles++;
                            
                            File.Copy(filePath, alterFilePath);
                        } else {
                            
                            if (!MD5Equals(filePath, alterFilePath)) {
                                ShowLog($"'{fileName}' is updated. overwritten on the package.");
                                changedFiles++;
                                
                                File.Copy(filePath, alterFilePath, true);
                            }
                        }
                    }
                );
                
                ShowLog("Checking the source in the package to clean up the files that should not be in the package...");
                
                allFiles     = Directory.GetFiles(option.AssetsPath, "*", SearchOption.AllDirectories);
                
                Array.ForEach(allFiles,
                    filePath => {
                        if (filePath.IndexOf(option.AssetsPath) < 0) {
                            ShowLog(LogLevel.Warn, $"An error occurred navigating the file path. {filePath}");
                            return;
                        }
                        
                        fileName      = Path.GetFileName(filePath);
                        
                        fullPath      = Path.GetDirectoryName(filePath).Split(option.AssetsPath)[1];
                        alterPath     = $"{option.RPGMVPath}{fullPath}";
                        alterFilePath = $"{alterPath}\\{fileName}";
                        
                        if (!Directory.Exists(alterPath) &&
                            Directory.Exists(Path.GetDirectoryName(filePath))) {
                            ShowLog($"'{alterPath}' has been removed from the package.");
                            
                            Directory.Delete(Path.GetDirectoryName(filePath), true);
                            removedFiles++;
                            
                            return;
                        }
                        
                        if (!File.Exists(alterFilePath) &&
                            File.Exists(filePath)) {
                            ShowLog($"'{alterFilePath}' has been removed from the package.");
                            removedFiles++;
                            
                            File.Delete(filePath);
                        }
                    }
                );
            } catch (Exception e) {
                
                ShowLog(LogLevel.Error, "An error occurred while packaging.");
                
                ShowLog(LogLevel.Error, "====================================================");
                ShowLog(LogLevel.Error, e.Message);
                ShowLog(LogLevel.Error, e.Source);
                ShowLog(LogLevel.Error, e.StackTrace);
                ShowLog(LogLevel.Error, "====================================================");
                
                FinalizeLog();
                
                if (!forceMode) {
                    
                    ShowLog("Press any key to exit...");
                    Console.Read();
                }
                
                return 3;
            }
            
            ShowLog("====================================================");
            ShowLog("Packaging complete!");
            
            ShowLog($"Changed file(s) : {changedFiles}");
            ShowLog($"Newly added file(s) : {addedFiles}");
            ShowLog($"Removed file(s) : {removedFiles}");
            ShowLog("====================================================");
            
            FinalizeLog();
            
            if (!forceMode) {
                
                ShowLog("Press any key to exit...");
                Console.Read();
            }
            
            return 0;
        }

        private static void ShowLog(string v)
            => ShowLog(LogLevel.Info, v);
        
        private static void ShowLog(LogLevel level, string v)
        {
            string log;
            switch (level) {
                default:
                case LogLevel.Info:  log = $"[Info] {v}"; break;
                case LogLevel.Warn:  log = $"[Warn] {v}"; break;
                case LogLevel.Error: log = $"[Error] {v}"; break;
            }
            
            if (isLogFileReady) {
                logWriter.WriteLine(log);
            }
            //    logger.AppendLine(log);
            
            Console.WriteLine(log);
        }
        
        static void InitLog() {
            if (isLogFileReady) return;
            
            isLogFileReady = true;
            
            isLogFileReady &= !string.IsNullOrEmpty(option.LogPath);
            
            if (!isLogFileReady) return;
            
            if (!Directory.Exists(option.LogPath))
                Directory.CreateDirectory(option.LogPath);
            
            var time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            
            logFileName = $"{time}.txt";
            int existCount = 0;
            
            string fullPath = Path.Combine(option.LogPath, logFileName);
            
            //Change file name if target file already exists.
            while (File.Exists(fullPath)) {
                logFileName = $"{time}_({++existCount}).txt";
                fullPath = Path.Combine(option.LogPath, logFileName);
            }
            
            stream    = File.Create(fullPath);
            logWriter = new StreamWriter(stream);
        }
        
        static void FinalizeLog()
        {
            if (!isLogFileReady) return;
            
            logWriter?.Close();
            logWriter?.Dispose();
            
            stream?.Close();
            stream?.Dispose();
            
            isLogFileReady = false;
        }
        
        static bool MD5Equals(string FirstPath, string SecondPath)
        {
            byte[] FirstBytes, SecondBytes;
            using (var FirstMD5  = MD5.Create())
            using (var SecondMD5 = MD5.Create()) {
                
                FirstBytes = File.ReadAllBytes(FirstPath);
                FirstMD5.TransformBlock(FirstBytes, 0, FirstBytes.Length, FirstBytes, 0);
                
                SecondBytes = File.ReadAllBytes(SecondPath);
                SecondMD5.TransformBlock(SecondBytes, 0, SecondBytes.Length, SecondBytes, 0);
                
                FirstBytes  = FirstMD5.ComputeHash(FirstBytes);
                SecondBytes = SecondMD5.ComputeHash(SecondBytes);
            }
            
            return FirstBytes.SequenceEqual(SecondBytes);
        }
    }
}
