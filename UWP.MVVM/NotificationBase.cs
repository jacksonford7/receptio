using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RECEPTIO.CapaPresentacion.UWP.MVVM
{
    public abstract class NotificationBase : INotifyPropertyChanged
    {
        protected bool BotonPresionado;
        private bool _estaOcupado;
        private bool _estaHabilitado;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool EstaOcupado
        {
            get
            {
                return _estaOcupado;
            }
            set
            {
                if (_estaOcupado == value)
                    return;
                _estaOcupado = value;
                RaisePropertyChanged("EstaOcupado");
            }
        }

        public bool EstaHabilitado
        {
            get
            {
                return _estaHabilitado;
            }
            set
            {
                if (_estaHabilitado == value)
                    return;
                _estaHabilitado = value;
                RaisePropertyChanged("EstaHabilitado");
            }
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] String property = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            RaisePropertyChanged(property);
            return true;
        }

        protected bool SetProperty<T>(T currentValue, T newValue, Action DoSet, [CallerMemberName] String property = null)
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue)) return false;
            DoSet.Invoke();
            RaisePropertyChanged(property);
            return true;
        }

        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public abstract class NotificationBase<T> : NotificationBase where T : class, new()
    {
        protected T This;

        public static implicit operator T(NotificationBase<T> thing) { return thing.This; }

        public NotificationBase(T thing = null)
        {
            This = thing ?? new T();
        }
    }
}
