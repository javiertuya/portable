using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace Giis.Portable.Util
{
    /// <summary>
    /// Basic file management Java/C# compatible
    /// </summary>
    public static class FileUtil
    {
        public static string FileRead(string fileName, bool throwIfNotExists)
        {
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);
            if (throwIfNotExists)
                throw new PortableException("File does not exist " + fileName);
            else
                return null;
        }
        public static string FileRead(string path, string name, bool throwIfNotExists)
        {
            return FileRead(Path.Combine(path, name), throwIfNotExists);
        }
        public static string FileRead(string path, string name)
        {
            return FileRead(Path.Combine(path, name), true);
        }
        public static string FileRead(string fileName)
        {
            return FileRead(fileName, true);
        }
        public static List<string> FileReadLines(string fileName)
        {
            return FileReadLines(fileName, true);
        }
        public static List<string> FileReadLines(string path, string name)
        {
            return FileReadLines(Path.Combine(path, name), true);
        }
        public static List<string> FileReadLines(string fileName, bool throwIfNotExists)
        {
            try
            {
                string[] linesArray = File.ReadAllLines(fileName);
                return new List<string>(linesArray);
            }
            catch (Exception e)
            {
                if (!throwIfNotExists)
                    return new List<string>();
                throw new PortableException("Error reading file " + fileName, e);
            }
        }
        public static List<string> FileReadLines(string path, string name, bool throwIfNotExists)
        {
            return FileReadLines(Path.Combine(path, name), throwIfNotExists);
        }

        public static void FileWrite(string path, string name, string content)
        {
            File.WriteAllText(Path.Combine(path, name), content);
        }
        public static void FileWrite(string fileName, string contents)
        {
            File.WriteAllText(fileName, contents);
        }
        public static void FileAppend(string path, string name, string content)
        {
            FileAppend(GetPath(path, name), content);
        }
        public static void FileAppend(string fileName, string line)
        {
            File.AppendAllText(fileName, line);
        }
        public static void CopyFile(string source, string dest)
        {
            File.Copy(source, dest);
        }

        /// <summary>
        /// Returns files at the specified folder that match a wildcard
        /// </summary>
        public static List<string> GetFilesMatchingWildcard(string folder, string fileNameWildcard)
        {
            string[] files = Directory.GetFiles(folder, fileNameWildcard, SearchOption.TopDirectoryOnly);
            List<string> list = new List<string>();
            foreach (string file in files)
                list.Add(Path.GetFileName(file));
            list.Sort(); //to make it repetible (linux returns different order than windows)
            return list;
        }
        public static IList<string> GetFileListInDirectory(string path)
        {
            //En java se tiene solo nombre, pero .net devuelve path y nombre
            List<string> fileNames = new List<string>();
            try
            {
                IList<string> filesPath = Directory.GetFiles(path);
                foreach (string filePath in filesPath)
                    fileNames.Add(GetFileNameOnly(filePath));
            }
            catch (Exception)
            {
                throw new PortableException("Can't browse directory at path " + path);
            }
            fileNames.Sort(); //to make it repetible (linux returns different order than windows)
            return fileNames;
        }
        private static string GetFileNameOnly(string fileWithPath)
        {
            //puee haber mezcla de separadores / \, busca el ultimo de ellos
            int first = -1; //si no se encuentram obtendra desde fist+1, es decir, desde cero
            for (int i = 0; i < fileWithPath.Length; i++)
                if (fileWithPath[i] == '/' || fileWithPath[i] == '\\')
                    first = i;
            return fileWithPath.Substring(first + 1, fileWithPath.Length - first - 1);
        }

        public static string GetPath(params string[] path)
        {
            return Path.Combine(path);
        }
        public static string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }
        public static string GetRelativePath(String path)
        {
            return Path.Combine(Parameters.GetProjectRoot(), path); //en net, niveles por encima pues se ejecuta desde la carpeta de la dll
        }

        /** 
         * Crea la carpeta indicada como parametro (la carpeta puede existir, o si no existe, debe existir el padre)
         */
        public static void CreateDirectory(string filePath)
        {
            Directory.CreateDirectory(filePath);
        }

        public static void DeleteFilesInDirectory(string path)
        {
            //la lista de ficheros viene sin path por compatibilidad con java, por lo que al borrar debe anyadir el path
            IList<string> files = GetFileListInDirectory(path);
            foreach (string fileName in files)
                File.Delete(Path.Combine(path, fileName));
        }

    }
}
