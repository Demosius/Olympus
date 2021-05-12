using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

/// <summary>
///     Master chariot abstract class to be used as the base class
///     for all database chariots.
///     
///     Each chariot is to be designed as the interface, to carry 
///     data to and from, for a specific database.
///     
///     Master chariot class to define expected functionality.
/// </summary>

namespace Olympus.Helios
{
    public abstract class MasterChariot
    {
        protected string _filePath;



        public MasterChariot(string filePath)
        {
            _filePath = filePath;
        }

        // Basic database management on a higher level. 
        // Each simply retruns true if the action was successful, often based
        // on whether the database is valid.
        public abstract bool RepairDatabase();

        public abstract bool ValidateDatabase();

        public abstract bool DeleteDatabase();

        public abstract bool BuildDatabase();

        public abstract bool EmptyDatabase();

        public abstract string FilePath { get; }

    }
}
