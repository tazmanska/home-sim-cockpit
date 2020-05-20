/*
 * Utworzone przez SharpDevelop.
 * Użytkownik: Tomek
 * Data: 2010-02-06
 * Godzina: 16:28
 * 
 * Do zmiany tego szablonu użyj Narzędzia | Opcje | Kodowanie | Edycja Nagłówków Standardowych.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;

namespace HomeSimCockpit
{
    /// <summary>
    /// Description of AppSettings.
    /// </summary>
    public class AppSettings : ConfigurationSection
    {
        private AppSettings()
        {
        }
        
        [ConfigurationProperty("lastFiles")]
        public LastOpenedFileCollection LastFiles
        {
            get { return (LastOpenedFileCollection)base["lastFiles"]; }
            set { base["lastFiles"] = value; }
        }
        
        [ConfigurationProperty("language", DefaultValue = Languages.English)]
        public Languages Language
        {
            get { return (Languages)base["language"]; }
            set { base["language"] = value; }
        }
        
        [ConfigurationProperty("priority", DefaultValue = ProcessPriorityClass.Normal)]
        public ProcessPriorityClass Priority
        {
            get { return (ProcessPriorityClass)base["priority"]; }
            set { base["priority"] = value; }
        }
        
        [ConfigurationProperty("processors", DefaultValue = 1)]
        public int Processors
        {
            get { return (int)base["processors"]; }
            set { base["processors"] = value; }
        }
        
        [ConfigurationProperty("logCounters", DefaultValue = false)]
        public bool LogCounters
        {
            get { return (bool)base["logCounters"]; }
            set { base["logCounters"] = value; }
        }
        
        [ConfigurationProperty("checkUpdates", DefaultValue = false)]
        public bool CheckUpdateOnStartup
        {
            get { return (bool)base["checkUpdates"]; }
            set { base["checkUpdates"] = value; }
        }
        
        [ConfigurationProperty("updateUrl", DefaultValue = "http://homesimcockpit.com/update/")]
        public string UpdateUrl
        {
            get { return (string)base["updateUrl"]; }
        }
        
        public void Save()
        {
            Configuration conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettings instance = (AppSettings)conf.GetSection(SECTION_NAME);
            if (instance == null)
            {
                instance = new AppSettings();
                conf.Sections.Add("hscSettings", instance);
            }
            instance.Language = Language;
            instance.LastFiles = LastFiles;
            instance.Priority = Priority;
            instance.Processors = Processors;
            instance.CheckUpdateOnStartup = CheckUpdateOnStartup;
            conf.Save(ConfigurationSaveMode.Full, false);
        }
        
        private static AppSettings __instance = null;
        private static readonly string SECTION_NAME = "hscSettings";

        public static AppSettings Instance
        {
            get
            {
                if (__instance == null)
                {
                    Configuration conf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    __instance = (AppSettings)conf.GetSection(SECTION_NAME);
                    if (__instance == null)
                    {
                        __instance = new AppSettings();
                    }
                }
                return __instance;
            }
        }
    }
    
    public class LastOpenedFileCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LastOpenedFile();
        }
        
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LastOpenedFile)element).FilePath;
        }
        
        private LastOpenedFile[] _lastOpenedFiles = null;
        
        public LastOpenedFile[] LastOpenedFiles
        {
            get
            {
                if (_lastOpenedFiles == null)
                {
                    _lastOpenedFiles = new LastOpenedFile[Count];
                    for (int i = 0; i < _lastOpenedFiles.Length; i++)
                    {
                        _lastOpenedFiles[i] = (LastOpenedFile)BaseGet(i);
                    }
                    Array.Sort(_lastOpenedFiles);
                }
                return _lastOpenedFiles;
            }
        }
        
        public void AddLastFile(string filePath)
        {
            List<LastOpenedFile> lasts = new List<LastOpenedFile>(LastOpenedFiles);
            LastOpenedFile find = lasts.Find(delegate(LastOpenedFile o)
                                             {
                                                 return o.FilePath.ToLowerInvariant() == filePath.ToLowerInvariant();
                                             });
            if (find != null)
            {
                lasts.Remove(find);
            }
            lasts.Insert(0, new LastOpenedFile() { FilePath = filePath, FileIndex = int.MinValue} );
            lasts.Sort();
            BaseClear();
            for (int i = 0; i < lasts.Count && i < 10; i++)
            {
                lasts[i].FileIndex = i;
                BaseAdd(i, lasts[i]);
            }
            _lastOpenedFiles = null;
        }
        
        public void ClearHistory()
        {
            _lastOpenedFiles = null;
            BaseClear();
        }
    }
    
    public class LastOpenedFile : ConfigurationElement, IComparable<LastOpenedFile>
    {
        [ConfigurationProperty("filePath")]
        public string FilePath
        {
            get { return (string)base["filePath"]; }
            set { base["filePath"] = value; }
        }
        
        [ConfigurationProperty("fileIndex")]
        public int FileIndex
        {
            get { return (int)base["fileIndex"]; }
            set { base["fileIndex"] = value; }
        }
        
        public int CompareTo(LastOpenedFile other)
        {
            return FileIndex.CompareTo(other.FileIndex);
        }
    }
}