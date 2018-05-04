namespace BrowserDataGrabber
{
    public class Configuration
    {
        // Google Chrome
        public static readonly string s_r_googleChromeMainDirectory = System.IO.Path.Combine("Google", "Chrome"); // Google Chrome Main Directory
        public static readonly string s_r_googleChromeUserDataDirectory = System.IO.Path.Combine(s_r_googleChromeMainDirectory, "User Data"); // Google Chrome (User Data) Directory
        public static readonly string s_r_googleChromeDefaultFolder = System.IO.Path.Combine(s_r_googleChromeUserDataDirectory, "Default"); // Google Chrome (User Data/Default) Folder

        public static readonly string s_r_googleChromeMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_localApplicationDataFolderPath, s_r_googleChromeMainDirectory); // Google Chrome Main Directory Path
        public static readonly string s_r_googleChromeUserDataDirectoryPath = System.IO.Path.Combine(Environment.s_r_localApplicationDataFolderPath, s_r_googleChromeUserDataDirectory); // Google Chrome (User Data) Directory Path
        public static readonly string s_r_googleChromeDefaultFolderPath = System.IO.Path.Combine(Environment.s_r_localApplicationDataFolderPath, s_r_googleChromeDefaultFolder); // Google Chrome (User Data/Default) Folder Path
        // - Google Chrome

        // Opera Stable & Opera Next & Opera Developer
        public static readonly string s_r_operaStable = "Opera Stable";
        public static readonly string s_r_operaNext = "Opera Next";
        public static readonly string s_r_operaDeveloper = "Opera Developer";

        public static readonly string s_r_operaStableMainDirectory = System.IO.Path.Combine("Opera Software", s_r_operaStable); // Opera Stable Main Directory
        public static readonly string s_r_operaNextMainDirectory = System.IO.Path.Combine("Opera Software", s_r_operaNext); // Opera Next Main Directory
        public static readonly string s_r_operaDeveloperMainDirectory = System.IO.Path.Combine("Opera Software", s_r_operaDeveloper); // Opera Developer Main Directory

        public static readonly string s_r_operaStableMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDataFolderPath, s_r_operaStableMainDirectory); // Opera Stable Main Directory Path
        public static readonly string s_r_operaNextMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDataFolderPath, s_r_operaNextMainDirectory); // Opera Next Main Directory Path
        public static readonly string s_r_operaDeveloperMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDataFolderPath, s_r_operaDeveloperMainDirectory); // Opera Developer Main Directory Path
        // - Opera Stable & Opera Next & Opera Developer
    }
}
