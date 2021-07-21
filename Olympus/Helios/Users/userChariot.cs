using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Olympus.Helios.Users.Model;

namespace Olympus.Helios.Users
{

    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string message) : base(message) { }
    }

    /// <summary>
    ///  The chariot for transferring user data back and forth from the database.
    /// </summary>
    public class UserChariot : MasterChariot
    {
        public override string DatabaseName { get; } = "Users.sqlite";

        public override Type[] Tables { get; } = new Type[] {typeof(User), typeof(Role), typeof(Login)};

        /*************************** Constructors ****************************/

        public UserChariot()
        {
            // Try first to use the directory based on App.Settings, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(App.Settings.SolLocation, "Users");
                InitializeDatabaseConnection();
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Users");
                InitializeDatabaseConnection();
            }
        }

        public UserChariot(string solLocation)
        {
            // Try first to use the given directory, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(solLocation, "Users");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Users");
                InitializeDatabaseConnection();
            }
        }

        public override void ResetConnection()
        {
            // Try first to use the directory based on App.Settings, if not then use local file.
            try
            {
                BaseDataDirectory = Path.Combine(App.Settings.SolLocation, "Users");
                InitializeDatabaseConnection();
            }
            catch
            {
                MessageBox.Show("Reverting to local use database.", "Error loading database.", MessageBoxButton.OK, MessageBoxImage.Warning);
                BaseDataDirectory = Path.Combine(App.BaseDirectory(), "Sol", "Users");
                InitializeDatabaseConnection();
            }
        }

        /***************************** CREATE Data ****************************/
        //public bool AddUser(int userID, string password)
        //{
        //    try
        //    {
        //        Database.Open();
        //        // Check if user already exists.
        //        string sql = $"SELECT [id] FROM [user] WHERE [id] = {userID};";
        //        SQLiteCommand command = new SQLiteCommand(sql, Database);
        //        object result = command.ExecuteScalar();
        //        if (result == null) throw new UserAlreadyExistsException("");

        //        // Check if default Role exists. Add it if it doesn't.
        //        command.CommandText = $"SELECT [name] FROM [role] WHERE [name] = 'default';";
        //        result = command.ExecuteScalar();
        //        if (result == null) AddDefaultRole();

        //        // Add relevant data to relevant tables.
        //        command.CommandText = $"INSERT INTO [user] (id, role_name) VALUES ({userID}, 'default');";
        //        command.ExecuteNonQuery();
        //        command.CommandText = $"INSERT INTO [login] (user_id, password) VALUES ({userID}, '{password}');";
        //        command.ExecuteNonQuery();

        //        Database.Close();
        //        return true;
        //    }
        //    catch (UserAlreadyExistsException)
        //    {
        //        MessageBox.Show($"The user [{userID}] already exists.");
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Toolbox.ShowUnexpectedException(ex);
        //        return false;
        //    }
        //}

        //private bool AddDefaultRole()
        //{
        //    try
        //    {
        //        Database.Open();
        //        string sql = $"SELECT [name] FROM [role] WHERE [name] = 'default;";
        //        SQLiteCommand command = new SQLiteCommand(sql, Database);
        //        object result = command.ExecuteScalar();
        //        if (result != null)
        //        {
        //            Database.Close();
        //            return false;
        //        }
        //        command.CommandText = $"INSERT INTO [role] (name) VALUES ('default');";
        //        command.ExecuteNonQuery();

        //        Database.Close();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Toolbox.ShowUnexpectedException(ex);
        //        return false;
        //    }
        //}

        /****************************** READ Data *****************************/

        /* Users */

        /* Log Ins */

        /* Roles */

        /****************************** UPDATE Data *****************************/

        /***************************** DELETE Data ****************************/


    }
}
