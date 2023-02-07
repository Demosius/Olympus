using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Khaos.Models;
using Khaos.Properties;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Khaos.ViewModels;

public class KhaosVM : INotifyPropertyChanged, IDBInteraction
{
    public Helios Helios { get; set; }

    [CanBeNull] public KhaosInstance Instance { get; set; }

    public List<string> InstanceNameList { get; set; }

    #region INotifyPropertyChanged Members

    public ObservableCollection<string> InstanceNames { get; set; }

    private bool useBufferBatches;
    public bool UseBufferBatches
    {
        get => useBufferBatches;
        set
        {
            useBufferBatches = value;
            OnPropertyChanged();
            if (Instance is not null)
                Instance.UseBufferBatches = value;
            else
            {
                Settings.Default.UseBufferBatches = value;
                Settings.Default.Save();
            }
        }
    }

    private bool splitMakeBulk;
    public bool SplitMakeBulk
    {
        get => splitMakeBulk;
        set
        {
            splitMakeBulk = value;
            OnPropertyChanged();
            if (Instance is not null)
                Instance.SplitMakeBulk = value;
            else
            {
                Settings.Default.SplitMakeBulk = value;
                Settings.Default.Save();
            }
        }
    }

    private bool splitProductType;
    public bool SplitProductType
    {
        get => splitProductType;
        set
        {
            splitProductType = value;
            OnPropertyChanged();
            if (Instance is not null)
                Instance.SplitProductType = value;
            else
            {
                Settings.Default.SplitProductType = value;
                Settings.Default.Save();
            }
        }
    }

    private bool capCube;
    public bool CapCube
    {
        get => capCube;
        set
        {
            capCube = value;
            OnPropertyChanged();
            if (Instance is not null)
                Instance.CapCube = value;
            else
            {
                Settings.Default.CapCube = value;
                Settings.Default.Save();
            }
        }
    }

    private bool useCountCap;
    public bool UseCountCap
    {
        get => useCountCap;
        set
        {
            useCountCap = value;
            OnPropertyChanged();
            if (Instance is not null)
                Instance.UseCountCap = value;
            else
            {
                Settings.Default.UseCountCap = value;
                Settings.Default.Save();
            }
        }
    }

    private bool useCountTarget;
    public bool UseCountTarget
    {
        get => useCountTarget;
        set
        {
            useCountTarget = value;
            OnPropertyChanged();
            if (Instance is not null)
                Instance.UseCountTarget = value;
            else
            {
                Settings.Default.UseCountTarget = value;
                Settings.Default.Save();
            }
        }
    }

    private float cubeCap;
    public string CubeCap
    {
        get => cubeCap.ToString(CultureInfo.CurrentCulture);
        set
        {
            if (float.TryParse(value, out cubeCap))
            {
                if (Instance is not null)
                    Instance.CubeCap = cubeCap;
                else
                {
                    Settings.Default.CubeCap = cubeCap;
                    Settings.Default.Save();
                }
            }
            OnPropertyChanged();
        }
    }

    private int groupCap;
    public string GroupCap
    {
        get => groupCap.ToString();
        set
        {
            if (int.TryParse(value, out groupCap))
            {
                if (Instance is not null)
                    Instance.GroupCap = groupCap;
                else
                {
                    Settings.Default.GroupCap = groupCap;
                    Settings.Default.Save();
                }
            }
            OnPropertyChanged();
        }
    }

    private float cubeTarget;
    public string CubeTarget
    {
        get => cubeTarget.ToString(CultureInfo.CurrentCulture);
        set
        {
            if (float.TryParse(value, out cubeTarget))
            {
                if (Instance is not null)
                    Instance.CubeTarget = cubeTarget;
                else
                {
                    Settings.Default.CubeTarget = cubeTarget;
                    Settings.Default.Save();
                }
            }
            OnPropertyChanged();
        }
    }

    private int groupTarget;
    public string GroupTarget
    {
        get => groupTarget.ToString();
        set
        {
            if (int.TryParse(value, out groupTarget))
            {
                if (Instance is not null)
                    Instance.GroupTarget = groupTarget;
                else
                {
                    Settings.Default.GroupTarget = groupTarget;
                    Settings.Default.Save();
                }
            }
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }
    public RepairDataCommand RepairDataCommand { get; set; }

    #endregion

    public KhaosVM(Helios helios)
    {
        Helios = helios;

        RefreshDataCommand = new RefreshDataCommand(this);
        RepairDataCommand = new RepairDataCommand(this);

        useBufferBatches = Settings.Default.UseBufferBatches;
        splitMakeBulk = Settings.Default.SplitMakeBulk;
        splitProductType = Settings.Default.SplitProductType;
        capCube = Settings.Default.CapCube;
        cubeCap = Settings.Default.CubeCap;
        groupCap = Settings.Default.GroupCap;
        useCountCap = Settings.Default.UseCountCap;
        useCountTarget = Settings.Default.UseCountTarget;
        cubeTarget = Settings.Default.CubeTarget;
        groupTarget = Settings.Default.GroupTarget;
    }

    public void RefreshData()
    {
        throw new NotImplementedException();
    }

    public void RepairData()
    {
        throw new NotImplementedException();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}