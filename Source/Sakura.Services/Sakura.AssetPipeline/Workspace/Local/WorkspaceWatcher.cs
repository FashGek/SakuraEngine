﻿namespace Sakura.AssetPipeline
{
    using System;
    using System.IO;

    public class WorkspaceWatcher : FileSystemWatcher
    {
        public WorkspaceWatcher(string Workspace, string LocalPath, string DatabaseLocation)
            : base(LocalPath)
        {
            this.NotifyFilter = NotifyFilters.Attributes
                     | NotifyFilters.CreationTime
                     | NotifyFilters.DirectoryName
                     | NotifyFilters.FileName
                     | NotifyFilters.LastAccess
                     | NotifyFilters.LastWrite
                     | NotifyFilters.Security
                     | NotifyFilters.Size;

            this.Changed += OnChanged;
            this.Created += OnCreated;
            this.Deleted += OnDeleted;
            this.Renamed += OnRenamed;
            this.Error += OnError;

            this.IncludeSubdirectories = true;
            this.EnableRaisingEvents = true;
            Console.WriteLine($"Start Watching: {LocalPath}!");
            Manifest = new LocalManifest(Path, DatabaseLocation);
        }
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
            //LiveScanner.ScanMetaAt(e.FullPath);
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");

        }

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) => PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
        public LocalManifest Manifest { get; }
    }
}
