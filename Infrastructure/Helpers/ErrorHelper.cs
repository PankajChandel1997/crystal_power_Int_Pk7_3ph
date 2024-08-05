using System.IO;
using System.Reflection;

namespace Infrastructure.Helpers
{
    public class ErrorHelper
    {
        public void WriteLog(string errorMessage)
        {
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\logs.txt";

            if (!File.Exists(path))
            { 
                // Create a file to write to
                FileStream fs = File.Create(path);
                fs.Dispose();
            }

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.AutoFlush = true;
                writer.WriteLine("Error ===> " + errorMessage + System.Environment.NewLine);
                writer.Dispose();
            }
        }
    }

    public static class WriteToFile
    {
        public static void WriteLog(string errorMessage)
        {
            string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\logs.txt";

            if (!File.Exists(path))
            {
                // Create a file to write to
                FileStream fs = File.Create(path);
                fs.Dispose();
            }

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.AutoFlush = true;
                writer.WriteLine("Error ===> " + errorMessage + System.Environment.NewLine);
                writer.Dispose();
            }
        }
    }
}
