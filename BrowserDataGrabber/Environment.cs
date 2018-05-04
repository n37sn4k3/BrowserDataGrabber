namespace BrowserDataGrabber
{
    public class Environment
    {
        public static readonly string s_r_userName = System.Environment.UserName;
        public static readonly string s_r_machineName = System.Environment.MachineName;

        public static readonly string s_r_applicationData = System.Environment.SpecialFolder.ApplicationData.ToString();
        public static readonly string s_r_applicationDataFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

        public static readonly string s_r_localApplicationData = System.Environment.SpecialFolder.LocalApplicationData.ToString();
        public static readonly string s_r_localApplicationDataFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);

        public static readonly string s_r_commonApplicationData = System.Environment.SpecialFolder.CommonApplicationData.ToString();
        public static readonly string s_r_commonApplicationDataFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);

        // Application Relative
        public static readonly string s_r_applicationDirectoryPath = System.IO.Path.Combine(s_r_applicationDataFolderPath, GetProductName());
        // - Application Relative

        public static string GetProductName()
        {
            // System.Reflection.Assembly callingAssembly = System.Reflection.Assembly.GetCallingAssembly();
            // System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(executingAssembly.Location);

            return fileVersionInfo.ProductName;
        }

        public static string GetProductVersion()
        {
            // System.Reflection.Assembly callingAssembly = System.Reflection.Assembly.GetCallingAssembly();
            // System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(executingAssembly.Location);

            return fileVersionInfo.ProductVersion;
        }
    }
}
