using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace DatabaseFunctionsGenerator
{
    public static class IO
    {
        public static string GetPath(string path)
        {
            //this is designed to determine if it is running on a webserver or desktop
            string relativePath = path;

            if (!Path.IsPathRooted(path) && HttpRuntime.AppDomainAppId != null)
            {
                relativePath = Path.Combine(@HostingEnvironment.ApplicationPhysicalPath, $"bin\\{path}");
            }

            return relativePath;
        }

        public static void CreateDirectory(string path)
        {
            Directory.CreateDirectory(GetPath(path));
        }

        public static bool DoesDirectoryExists(string path)
        {
            return Directory.Exists(GetPath(path));
        }

        public static void CopyFile(string source, string destination)
        {
            File.Copy(GetPath(source), GetPath(destination), true);
        }

        public static void CopyDirectory(string source, string destination)
        {
            foreach(string directory in Directory.GetDirectories(source))
            {
                string newPath = $"{destination}\\{new DirectoryInfo(directory).Name}";
                if (!DoesDirectoryExists(newPath))
                    CreateDirectory(newPath);
                CopyDirectory(directory, newPath);
            }

            foreach(string file in Directory.GetFiles(source))
            {
                CopyFile(file, $"{destination}\\{Path.GetFileName(file)}");
            }
        }

        public static string ReadFile(string path)
        {
            String text;
            StreamReader reader;

            reader = new StreamReader(GetPath(path));
            text = "";

            text = reader.ReadToEnd();

            reader.Dispose();

            return text;
        }

        public static void WriteFile(string path, string text)
        {
            StreamWriter writer;

            writer = new StreamWriter(GetPath(path));

            writer.Write(text);

            writer.Dispose();
        }
    }
}
