using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Prometheus.ViewModel.Helpers
{
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
            if (category == EDataCategory.Equipment)
                return "Equipment";
            if (category == EDataCategory.Inventory)
                return "Inventory";
            if (category == EDataCategory.Staff)
                return "Staff";
            if (category == EDataCategory.Users)
                return "Users";
            return "None";
        }

        public static EDataCategory StringToDataCategory(string category)
        {
            category = (category ?? "inventory").ToUpper();
            if (category == "INVENTORY")
                return EDataCategory.Inventory;
            if (category == "EQUIPMENT")
                return EDataCategory.Equipment;
            if (category == "STAFF")
                return EDataCategory.Staff;
            if (category == "USERS")
                return EDataCategory.Users;
            return EDataCategory.Inventory;
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
            if (category == EDataCategory.Inventory)
                return new List<EDataType>
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
                };
            if (category == EDataCategory.Equipment)
                return new List<EDataType>
                {
                    EDataType.Checklist,
                    EDataType.CompletedChecklist,
                    EDataType.Machine,
                    EDataType.MachineType
                };
            if (category == EDataCategory.Staff)
                return new List<EDataType>
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
                };
            if (category == EDataCategory.Users)
                return new List<EDataType>
                {
                    EDataType.User,
                    EDataType.UserRole,
                    EDataType.Login
                };
            return new List<EDataType> { };
        }

        public static EDataCategory DataTypeToCatagory(EDataType type)
        {
            if (type <= EDataType.ZoneAccessLevel)
                return EDataCategory.Inventory;
            if (type <= EDataType.Vehicle)
                return EDataCategory.Staff;
            if (type <= EDataType.MachineType)
                return EDataCategory.Equipment;
            return EDataCategory.Users;
        }

        public static string DataTypeToString(EDataType type)
        {
            if (type == EDataType.Batch)
                return "Batch";
            if (type == EDataType.Bay)
                return "Bay";
            if (type == EDataType.BayZone)
                return "BayZone";
            if (type == EDataType.BinContentsUpdate)
                return "BinContetnsUpdate";
            if (type == EDataType.BinExtension)
                return "BinExtension";
            if (type == EDataType.Checklist)
                return "Checklist";
            if (type == EDataType.Clan)
                return "Clan";
            if (type == EDataType.CompletedChecklist)
                return "CompletedChecklist";
            if (type == EDataType.Department)
                return "Department";
            if (type == EDataType.DepartmentProject)
                return "DepartmentProject";
            if (type == EDataType.Employee)
                return "Employee";
            if (type == EDataType.EmployeeAvatar)
                return "EmployeeAvatar";
            if (type == EDataType.EmployeeDepartmentLoaning)
                return "EmployeeDepartmentLoaning";
            if (type == EDataType.EmployeeIcon)
                return "EmployeeIcon";
            if (type == EDataType.EmployeeInduction)
                return "EmployeeInduction";
            if (type == EDataType.EmployeeProject)
                return "EmployeeProject";
            if (type == EDataType.EmployeeShift)
                return "EmployeeShift";
            if (type == EDataType.EmployeeVehicle)
                return "EmployeeVehicle";
            if (type == EDataType.Image)
                return "Image";
            if (type == EDataType.Induction)
                return "Induction";
            if (type == EDataType.Licence)
                return "Licence";
            if (type == EDataType.LicenceImage)
                return "LicenceImage";
            if (type == EDataType.Locker)
                return "Locker";
            if (type == EDataType.Login)
                return "Login";
            if (type == EDataType.Machine)
                return "Machine";
            if (type == EDataType.MachineType)
                return "MachineType";
            if (type == EDataType.Move)
                return "Move";
            if (type == EDataType.NAVBin)
                return "NAVBin";
            if (type == EDataType.NAVCategory)
                return "NAVCategory";
            if (type == EDataType.NAVDivision)
                return "NAVDivision";
            if (type == EDataType.NAVGenre)
                return "NAVGenre";
            if (type == EDataType.NAVItem)
                return "NAVItem";
            if (type == EDataType.NAVLocation)
                return "NAVLocation";
            if (type == EDataType.NAVMoveLine)
                return "NAVMoveLine";
            if (type == EDataType.NAVPlatform)
                return "NAVPlatform";
            if (type == EDataType.NAVStock)
                return "NAVStock";
            if (type == EDataType.NAVTransferOrder)
                return "NAVTransferOrder";
            if (type == EDataType.NAVUoM)
                return "NAVUoM";
            if (type == EDataType.NAVZone)
                return "NAVZone";
            if (type == EDataType.Project)
                return "Project";
            if (type == EDataType.ProjectIcon)
                return "ProjectIcon";
            if (type == EDataType.Shift)
                return "Shift";
            if (type == EDataType.ShiftRule)
                return "ShiftRule";
            if (type == EDataType.StaffRole)
                return "StaffRole";
            if (type == EDataType.Stock)
                return "Stock";
            if (type == EDataType.SubStock)
                return "SubStock";
            if (type == EDataType.TableUpdate)
                return "TableUpdate";
            if (type == EDataType.TagUse)
                return "TagUse";
            if (type == EDataType.TempTag)
                return "TempTag";
            if (type == EDataType.User)
                return "User";
            if (type == EDataType.UserRole)
                return "UserRole";
            if (type == EDataType.Vehicle)
                return "Vehicle";
            if (type == EDataType.ZoneAccessLevel)
                return "ZoneAccessLevel";
            return "None";
        }

        public static EDataType StringToDataType(string type)
        {
            type = (type ?? "none").ToUpper();
            if (type == "BATCH")
                return EDataType.Batch;
            if (type == "BAY")
                return EDataType.Bay;
            if (type == "BAYZONE")
                return EDataType.BayZone;
            if (type == "BINCONTENTSUPDATE")
                return EDataType.BinContentsUpdate;
            if (type == "BINEXTENSION")
                return EDataType.BinExtension;
            if (type == "CHECKLIST")
                return EDataType.Checklist;
            if (type == "CLAN")
                return EDataType.Clan;
            if (type == "COMPLETEDCHECKLIST")
                return EDataType.CompletedChecklist;
            if (type == "DEPARTMENT")
                return EDataType.Department;
            if (type == "DEPARTMENTPROJECT")
                return EDataType.DepartmentProject;
            if (type == "EMPLOYEE")
                return EDataType.Employee;
            if (type == "EMPLOYEEAVATAR")
                return EDataType.EmployeeAvatar;
            if (type == "EMPLOYEEDEPARTMENTLOANING")
                return EDataType.EmployeeDepartmentLoaning;
            if (type == "EMPLOYEEICON")
                return EDataType.EmployeeIcon;
            if (type == "EMPLOYEEINDUCTION")
                return EDataType.EmployeeInduction;
            if (type == "EMPLOYEEPROJECT")
                return EDataType.EmployeeProject;
            if (type == "EMPLOYEESHIFT")
                return EDataType.EmployeeShift;
            if (type == "EMPLOYEEVEHICLE")
                return EDataType.EmployeeVehicle;
            if (type == "IMAGE")
                return EDataType.Image;
            if (type == "INDUCTION")
                return EDataType.Induction;
            if (type == "LICENCE")
                return EDataType.Licence;
            if (type == "LICENCEIMAGE")
                return EDataType.LicenceImage;
            if (type == "LOCKER")
                return EDataType.Locker;
            if (type == "LOGIN")
                return EDataType.Login;
            if (type == "MACHINE")
                return EDataType.Machine;
            if (type == "MACHINETYPE")
                return EDataType.MachineType;
            if (type == "MOVE")
                return EDataType.Move;
            if (type == "NAVBIN")
                return EDataType.NAVBin;
            if (type == "NAVCATEGORY")
                return EDataType.NAVCategory;
            if (type == "NAVDIVISION")
                return EDataType.NAVDivision;
            if (type == "NAVGENRE")
                return EDataType.NAVGenre;
            if (type == "NAVITEM")
                return EDataType.NAVItem;
            if (type == "NAVLOCATION")
                return EDataType.NAVLocation;
            if (type == "NAVMOVELINE")
                return EDataType.NAVMoveLine;
            if (type == "NAVPLATFORM")
                return EDataType.NAVPlatform;
            if (type == "NAVSTOCK")
                return EDataType.NAVStock;
            if (type == "NAVTRANSFERORDER")
                return EDataType.NAVTransferOrder;
            if (type == "NAVUOM")
                return EDataType.NAVUoM;
            if (type == "NAVZONE")
                return EDataType.NAVZone;
            if (type == "PROJECT")
                return EDataType.Project;
            if (type == "PROJECTICON")
                return EDataType.ProjectIcon;
            if (type == "SHIFT")
                return EDataType.Shift;
            if (type == "SHIFTRULE")
                return EDataType.ShiftRule;
            if (type == "STAFFROLE")
                return EDataType.StaffRole;
            if (type == "STOCK")
                return EDataType.Stock;
            if (type == "SUBSTOCK")
                return EDataType.SubStock;
            if (type == "TABLEUPDATE")
                return EDataType.TableUpdate;
            if (type == "TAGUSE")
                return EDataType.TagUse;
            if (type == "TEMPTAG")
                return EDataType.TempTag;
            if (type == "USER")
                return EDataType.User;
            if (type == "USERROLE")
                return EDataType.UserRole;
            if (type == "VEHICLE")
                return EDataType.Vehicle;
            if (type == "ZONEACCESSLEVEL")
                return EDataType.ZoneAccessLevel;
            return EDataType.Batch;
        }
    }
}
