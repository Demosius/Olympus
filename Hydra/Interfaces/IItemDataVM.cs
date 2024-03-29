﻿using Hydra.ViewModels.Commands;
using Uranus.Interfaces;

namespace Hydra.Interfaces;

public interface IItemDataVM : IDBInteraction, IItemFilters
{
    public ActivateAllItemsCommand ActivateAllItemsCommand { get; set; }
    public DeActivateAllItemsCommand DeActivateAllItemsCommand { get; set; }
    public ExclusiveItemActivationCommand ExclusiveItemActivationCommand { get; set; }

    public void ActivateAllItems();
    public void DeActivateAllItems();
    public void ExclusiveItemActivation();
}