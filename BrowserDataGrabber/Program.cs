namespace BrowserDataGrabber
{
    class Program
    {
        static void Main(string[] args)
        {
            // Application
            if (Environment.s_r_applicationDirectoryPath != null)
            {
                Utils.CreateDirectoryIfNotExists(Environment.s_r_applicationDirectoryPath);
            }

            System.Console.WriteLine("Product Name: " + Environment.GetProductName());
            System.Console.WriteLine("Product Version: " + Environment.GetProductVersion() + "\n");

            System.Console.WriteLine("User Name: " + Environment.s_r_userName);
            System.Console.WriteLine("Machine Name: " + Environment.s_r_machineName + "\n");

            System.Console.WriteLine("Application Data: " + Environment.s_r_applicationData);
            System.Console.WriteLine("Application Data Folder Path: " + Environment.s_r_applicationDataFolderPath + "\n");

            System.Console.WriteLine("Local Application Data: " + Environment.s_r_localApplicationData);
            System.Console.WriteLine("Local Application Data Folder Path: " + Environment.s_r_localApplicationDataFolderPath + "\n");

            System.Console.WriteLine("Common Application Data: " + Environment.s_r_commonApplicationData);
            System.Console.WriteLine("Common Application Data Folder Path: " + Environment.s_r_commonApplicationDataFolderPath + "\n");
            // - Application

            Browser.ExtractGoogleChromeData(); // Google Chrome

            Browser.ExtractOperaData(Configuration.s_r_operaStable); // Opera Stable
            Browser.ExtractOperaData(Configuration.s_r_operaNext); // Opera Next
            Browser.ExtractOperaData(Configuration.s_r_operaDeveloper); // Opera Developer
        }
    }
}
