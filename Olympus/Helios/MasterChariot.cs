using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace Olympus.Helios
{
    public abstract class MasterChariot
    {
        protected string _filePath;


        public MasterChariot(string filePath)
        {
            _filePath = filePath;
        }

        public abstract void RepairDatabase();

        public abstract void ValidateDatabase();

        public abstract void DeleteDatabase();

        public abstract void BuildDatabase();

        public abstract void EmptyDatabase();

        ~MasterChariot();

        public abstract string FilePath { get; }

    }
}
