using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;

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
        public UserChariot()
        {
            FilePath = "./Sol/Users/Users.sqlite";
            Connect();
        }

        public UserChariot(string solLocation)
        {
            FilePath = solLocation + "/Users/Users.sqlite";
            Connect();
        }

        /******************************* Get Data *****************************/
        /* Users */

        /* Log Ins */

        /* Roles */

        /******************************* Put Data *****************************/
        public bool AddUser(int userID, string password)
        {
            try
            {
                Conn.Open();
                // Check if user already exists.
                string sql = $"SELECT [id] FROM [user] WHERE [id] = {userID};";
                SQLiteCommand command = new SQLiteCommand(sql, Conn);
                object result = command.ExecuteScalar();
                if (result == null) throw new UserAlreadyExistsException("");

                // Check if default Role exists. Add it if it doesn't.
                command.CommandText = $"SELECT [name] FROM [role] WHERE [name] = 'default';";
                result = command.ExecuteScalar();
                if (result == null) AddDefaultRole();

                // Add relevant data to relevant tables.
                command.CommandText = $"INSERT INTO [user] (id, role_name) VALUES ({userID}, 'default');";
                command.ExecuteNonQuery();
                command.CommandText = $"INSERT INTO [login] (user_id, password) VALUES ({userID}, '{password}');";
                command.ExecuteNonQuery();

                Conn.Close();
                return true;
            }
            catch (UserAlreadyExistsException)
            {
                MessageBox.Show($"The user [{userID}] already exists.");
                return false;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        private bool AddDefaultRole()
        {
            try
            {
                Conn.Open();
                string sql = $"SELECT [name] FROM [role] WHERE [name] = 'default;";
                SQLiteCommand command = new SQLiteCommand(sql, Conn);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    Conn.Close();
                    return false;
                }
                command.CommandText = $"INSERT INTO [role] (name) VALUES ('default');";
                command.ExecuteNonQuery();

                Conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Toolbox.ShowUnexpectedException(ex);
                return false;
            }
        }

        /****************************** Post Data *****************************/

        /***************************** Delete Data ****************************/

        /****************************Table Definitions*************************/
        private static readonly string LoginDefinition =
            @"create table login
            (
                user_id  int  not null
                    constraint login_pk
                        primary key
                    references user
                        on update cascade on delete cascade,
                password text not null
            );

            create unique index login_user_id_uindex
                on login (user_id);";

        private static readonly string RoleDefinition =
            @"-- auto-generated definition
            create table role
            (
                name              text not null
                    constraint role_pk
                        primary key,
                create_user       int     default -2 not null,
                delete_user       int     default -2 not null,
                view_user         int     default -5 not null,
                modify_user       int     default -2 not null,
                create_employee   int     default -2 not null,
                delete_employee   int     default -2 not null,
                view_employee     int     default -5 not null,
                modify_employee   int     default -2 not null,
                assign_role       boolean default FALSE not null,
                create_department boolean default FALSE not null,
                delete_department boolean default FALSE not null,
                modify_department boolean default FALSE not null,
                create_clan       boolean default FALSE not null,
                delete_clan       boolean default FALSE not null,
                modify_clan       boolean default FALSE not null,
                create_shift      int     default -1 not null,
                delete_shift      int     default -1 not null,
                modify_shift      int     default -1 not null,
                create_licence    boolean default FALSE not null,
                delete_licence    boolean default FALSE not null,
                view_licence      boolean default FALSE not null,
                modify_licence    boolean default FALSE not null,
                create_vehicle    boolean default FALSE not null,
                delete_vehicle    boolean default FALSE not null,
                view_vehicle      boolean default FALSE not null,
                modify_vehicle    boolean default FALSE not null,
                create_induction  boolean default FALSE not null,
                delete_induction  boolean default FALSE not null,
                modify_induction  boolean default FALSE not null,
                view_induction    boolean default FALSE not null,
                loan_employee     boolean default FALSE not null
            );

            create unique index role_name_uindex
                on role (name);

";

        private static readonly string UserDefinition =
            @"create table user
            (
                id        int not null
                    constraint user_pk
                        primary key,
                role_name int
                    references role
            );

            create unique index user_id_uindex
                on user (id);";

        public override Dictionary<string, string> TableDefinitions
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "login", LoginDefinition },
                    { "role", RoleDefinition },
                    { "user", UserDefinition }
                };
            }
        }   
    }
}
