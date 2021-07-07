using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Titan
{
    public static class SQLTesting
    {
        private static readonly string baseDataDirectory = Path.Combine("\\\\ausefpdfs01ns\\Shares\\Public\\Aaron Penny\\Data\\PCLTest");

        private static readonly string databaseName = "SolInv.db";

        public static string dbString = Path.Combine(baseDataDirectory, databaseName);

        /// <summary>
        /// Static method to allow local data services to initialise their associated database conveniently.
        /// </summary>
        /// <param name="database">The SQLite database connection</param>
        /// <param name="databaseName">The SQLite database name</param>
        /// <param name="tables">The SQLite database tables to create (if required)</param>
        /// <returns>An initialised SQLite database connection</returns>
        public static SQLiteConnection InitializeLocalDatabase(SQLiteConnection database, params Type[] tables)
        {
            if (database == null)
            {
                if (!Directory.Exists(baseDataDirectory)) Directory.CreateDirectory(baseDataDirectory);

                database = new SQLiteConnection(dbString);
            }

            database.CreateTables(CreateFlags.None, tables);

            return database;
        }
    }

    /// <summary>
    /// Represents a local 'Todo' item.
    /// </summary>
    [Table("Todos")]
    public class LocalTodo
    {
        #region Properties

        /// <summary>
        /// The unique ID of the Todo item.
        /// </summary>
        [Indexed(Name = "CompositeKey", Order = 1, Unique = true), AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// The ID of the User the Todo item is assigned to.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The Title/Description of the Todo item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Whether or not the Todo item has been completed.
        /// </summary>
        public bool Completed { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Overrides the standard 'ToString' method with the Title of the Todo.
        /// </summary>
        /// <returns>The Title of the Todo with a prefix</returns>
        public override string ToString() => $"Title: {Title}";

        #endregion
    }

    [Table("BinList")] 
    public class Bin
    {
        [Indexed(Name = "BinCompKey", Order = 1, Unique = true), NotNull]
        public string Code { get; set; }
        [Indexed(Name = "BinCompKey", Order = 2, Unique = true), NotNull]
        public string Location { get; set; }
        [NotNull]
        public string ZoneCode { get; set; }
        public string Description { get; set; }
        public bool Empty { get; set; }
        public bool Assigned { get; set; }
        public int Ranking { get; set; }
        public float UsedCube { get; set; }
        public float MaxCube { get; set; }
        public DateTime LastCCDate { get; set; }
        public DateTime LastPIDate { get; set; }
    }

    [Table("ItemData")]
    public class Item
    {
        [PrimaryKey]
        public int Number { get; set; }
        public string Description { get; set; }
        public int Category { get; set; }
        public int Platform { get; set; }
        public int Division { get; set; }
        public int Genre { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double Cube { get; set; }
        public bool Preowned { get; set; }
    }

    [Table("BinContents")]
    public class Stock
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Location { get; set; }
        public string ZoneCode { get; set; }
        public string BinCode { get; set; }
        public int ItemNumber { get; set; }
        public string Barcode { get; set; }
        public string UoMCode { get; set; }
        public int Qty { get; set; }
        public int PickQty { get; set; }
        public int PutAwayQty { get; set; }
        public int NegAdjQty { get; set; }
        public int PosAdjQty { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool Fixed { get; set; }
    }

    [Table("UoMData")]
    public class UoM
    {
        [Indexed(Name = "UoMCompKey", Order = 1, Unique = true)]
        public string Code { get; set; }
        [Indexed(Name = "UoMCompKey", Order = 2, Unique = true)]
        public int ItemNumber { get; set; }
        public int QtyPerUoM { get; set; }
        public int MaxQty { get; set; }
        public bool InnerPack { get; set; }
        public bool ExcludeCartonization { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double Cube { get; set; }
        
    }
}
