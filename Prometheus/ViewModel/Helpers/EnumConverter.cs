using System.Collections.Generic;

namespace Prometheus.ViewModel.Helpers;

public enum EDataCategory
{
    Inventory,
    Staff,
    Equipment,
    Users
}

public enum EDataType
{
    // Inventory -22
    Batch,
    Bay,
    BayZone,
    BinContentsUpdate,
    BinExtension,
    Move,
    NAVBin,
    NAVCategory,
    NAVDivision,
    NAVGenre,
    NAVItem,
    NAVLocation,
    NAVMoveLine,
    NAVPlatform,
    NAVStock,
    NAVTransferOrder,
    NAVUoM,
    NAVZone,
    Stock,
    SubStock,
    TableUpdate,
    ZoneAccessLevel,

    // Staff -24
    Clan,
    Department,
    DepartmentProject,
    Employee,
    EmployeeAvatar,
    EmployeeDepartmentLoaning,
    EmployeeIcon,
    EmployeeInduction,
    EmployeeProject,
    EmployeeShift,
    EmployeeVehicle,
    Image,
    Induction,
    Licence,
    LicenceImage,
    Locker,
    Project,
    ProjectIcon,
    StaffRole,
    Shift,
    ShiftRule,
    TagUse,
    TempTag,
    Vehicle,

    // Equipment -4
    Checklist,
    CompletedChecklist,
    Machine,
    MachineType,

    // Users -3
    Login,
    UserRole,
    User
}

public static class EnumConverter
{
    public static string DataCategoryToString(EDataCategory category)
    {
        return category switch
        {
            EDataCategory.Equipment => "Equipment",
            EDataCategory.Inventory => "Inventory",
            EDataCategory.Staff => "Staff",
            EDataCategory.Users => "Users",
            _ => "None"
        };
    }

    public static EDataCategory StringToDataCategory(string category)
    {
        category = category.ToUpper();
        return category switch
        {
            "INVENTORY" => EDataCategory.Inventory,
            "EQUIPMENT" => EDataCategory.Equipment,
            "STAFF" => EDataCategory.Staff,
            "USERS" => EDataCategory.Users,
            _ => EDataCategory.Inventory
        };
    }

    public static List<EDataCategory> GetDataCategories()
    {
        return new List<EDataCategory>
        {
            EDataCategory.Inventory,
            EDataCategory.Equipment,
            EDataCategory.Staff,
            EDataCategory.Users
        };
    }

    public static List<EDataType> GetTypeList(EDataCategory category)
    {
        return category switch
        {
            EDataCategory.Inventory => new List<EDataType>
            {
                EDataType.Batch,
                EDataType.Bay,
                EDataType.BayZone,
                EDataType.BinContentsUpdate,
                EDataType.BinExtension,
                EDataType.Move,
                EDataType.NAVBin,
                EDataType.NAVCategory,
                EDataType.NAVDivision,
                EDataType.NAVGenre,
                EDataType.NAVItem,
                EDataType.NAVLocation,
                EDataType.NAVMoveLine,
                EDataType.NAVPlatform,
                EDataType.NAVStock,
                EDataType.NAVTransferOrder,
                EDataType.NAVUoM,
                EDataType.NAVZone,
                EDataType.Stock,
                EDataType.SubStock,
                EDataType.TableUpdate,
                EDataType.ZoneAccessLevel
            },
            EDataCategory.Equipment => new List<EDataType>
            {
                EDataType.Checklist, EDataType.CompletedChecklist, EDataType.Machine, EDataType.MachineType
            },
            EDataCategory.Staff => new List<EDataType>
            {
                EDataType.Clan,
                EDataType.Department,
                EDataType.DepartmentProject,
                EDataType.Employee,
                EDataType.EmployeeAvatar,
                EDataType.EmployeeDepartmentLoaning,
                EDataType.EmployeeIcon,
                EDataType.EmployeeInduction,
                EDataType.EmployeeProject,
                EDataType.EmployeeShift,
                EDataType.EmployeeVehicle,
                EDataType.Image,
                EDataType.Induction,
                EDataType.Licence,
                EDataType.LicenceImage,
                EDataType.Locker,
                EDataType.Project,
                EDataType.ProjectIcon,
                EDataType.StaffRole,
                EDataType.Shift,
                EDataType.ShiftRule,
                EDataType.TagUse,
                EDataType.TempTag,
                EDataType.Vehicle
            },
            EDataCategory.Users => new List<EDataType> { EDataType.User, EDataType.UserRole, EDataType.Login },
            _ => new List<EDataType>()
        };
    }

    public static EDataCategory DataTypeToCategory(EDataType type)
    {
        return type switch
        {
            <= EDataType.ZoneAccessLevel => EDataCategory.Inventory,
            <= EDataType.Vehicle => EDataCategory.Staff,
            <= EDataType.MachineType => EDataCategory.Equipment,
            _ => EDataCategory.Users
        };
    }

}