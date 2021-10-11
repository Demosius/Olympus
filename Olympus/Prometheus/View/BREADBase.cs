﻿using Olympus.Prometheus.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Olympus.Prometheus.View
{
    /// <summary>
    /// Base page class for all CRUD/BREAD based pages for adjusting data.
    /// </summary>
    public abstract class BREADBase : Page
    {
        // Each one should reference a specific data type.
        public abstract EDataType DataType { get; }
    }
}
