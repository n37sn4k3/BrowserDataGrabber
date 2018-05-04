namespace BrowserDataGrabber
{
    public class Utils
    {
        public static void CopyFile(string sourceFilePath, string destFilePath)
        {
            try
            {
                System.IO.File.Copy(sourceFilePath, destFilePath);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public static void CopyFile(string sourceFilePath, string destFilePath, bool overwrite)
        {
            try
            {
                System.IO.File.Copy(sourceFilePath, destFilePath, overwrite);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public static void CreateDirectory(string directoryPath)
        {
            try
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        public static void CreateDirectoryIfNotExists(string directoryPath)
        {
            if (!System.IO.Directory.Exists(directoryPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(directoryPath);
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
