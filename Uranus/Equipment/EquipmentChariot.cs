using Uranus.Equipment.Model;
using System;
using System.IO;

namespace Uranus.Equipment;

public sealed class EquipmentChariot : MasterChariot
{
    public override string DatabaseName => "Equipment.sqlite";

    public override Type[] Tables { get; } =
    {
        typeof(Checklist), typeof(CompletedChecklist), typeof(MachineType), typeof(Machine)
    };

    /*************************** Constructors ****************************/

    public EquipmentChariot(string solLocation)
    {
        // Try first to use the given directory, if not then use local file.
        BaseDataDirectory = Path.Combine(solLocation, "Equipment");
        InitializeDatabaseConnection();
    }

    /// <summary>
    /// Resets the connection using the given location string.
    /// </summary>
    /// <param name="solLocation">A directory location, in which the Equipment database does/should reside.</param>
    public override void ResetConnection(string solLocation)
    {
        // First thing is to nullify the current database (connection).
        Database = null;

        BaseDataDirectory = Path.Combine(solLocation, "Equipment");
        InitializeDatabaseConnection();
    }

    /***************************** CREATE Data ****************************/

    /******************************* Get Data *****************************/
    /* Batteries */

    /* Chargers */

    /* Checklists */

    /* Machine */

    /******************************* Put Data *****************************/

    /****************************** Post Data *****************************/

    /***************************** Delete Data ****************************/


}