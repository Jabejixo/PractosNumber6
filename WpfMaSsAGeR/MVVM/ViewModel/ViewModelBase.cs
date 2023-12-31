﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaSsAGeR.MVVM.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        protected void Set<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (field == null) return;
            if (field.Equals(value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
