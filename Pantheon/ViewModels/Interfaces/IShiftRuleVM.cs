﻿namespace Pantheon.ViewModels.Interfaces;

public interface IShiftRuleVM
{
    public bool IsValid { get; }
    public bool InEdit { get; set; }
    public bool IsNew { get; }
}