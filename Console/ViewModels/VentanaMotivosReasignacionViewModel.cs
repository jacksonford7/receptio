using Console.ServicioConsole;
using RECEPTIO.CapaPresentacion.UWP.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Console.ViewModels
{
    internal class VentanaMotivosReasignacionViewModel : NotificationBase
    {
        #region Variables
        private readonly ServicioConsoleClient _servicio;
        private readonly Dictionary<long, Tuple<long, short>> _tickets;
        private ObservableCollection<REASSIGNMENT_MOTIVE> _motivos;
        private RelayCommand _comandoReasignar;
        private REASSIGNMENT_MOTIVE _motivoSeleccionado;
        private ObservableCollection<USER_SESSION> _LstsesionesUsuarios;
        private ObservableCollection<TROUBLE_DESK_USER> _sesionesUsuarios;
        private TROUBLE_DESK_USER _sesionUsuarioSeleccionada;
        internal short Resultado = -1;
        private bool _chequear;
        private bool _habilitaCmb;
        private bool _habilitaChk;
        #endregion

        #region Constructor
        internal VentanaMotivosReasignacionViewModel(ServicioConsoleClient servicio, Dictionary<long, Tuple<long, short>> tickets)
        {
            _servicio = servicio;
            _tickets = tickets;
            _comandoReasignar = new RelayCommand(Reasignar);
            PropertyChanged += (s, e) => _comandoReasignar.RaiseCanExecuteChanged();
            ObtenerMotivos();
            ActivarCheckBox();
        }
        #endregion

        #region Propiedades
        public RelayCommand ComandoReasignar
        {
            get
            {
                return _comandoReasignar;
            }
            set
            {
                SetProperty(ref _comandoReasignar, value);
            }
        }

        public ObservableCollection<REASSIGNMENT_MOTIVE> Motivos
        {
            get
            {
                return _motivos;
            }
            set
            {
                SetProperty(ref _motivos, value);
            }
        }

        public REASSIGNMENT_MOTIVE MotivoSeleccionado
        {
            get
            {
                return _motivoSeleccionado;
            }
            set
            {
                SetProperty(ref _motivoSeleccionado, value);
            }
        }

        public ObservableCollection<USER_SESSION> ListaSesionesUsuarios
        {
            get
            {
                return _LstsesionesUsuarios;
            }
            set
            {
                SetProperty(ref _LstsesionesUsuarios, value);
            }
        }

        public ObservableCollection<TROUBLE_DESK_USER> SesionesUsuarios
        {
            get
            {
                return _sesionesUsuarios;
            }
            set
            {
                SetProperty(ref _sesionesUsuarios, value);
            }
        }

        public TROUBLE_DESK_USER SesionUsuarioSeleccionada
        {
            get
            {
                return _sesionUsuarioSeleccionada;
            }
            set
            {
                SetProperty(ref _sesionUsuarioSeleccionada, value);
            }
        }

        public bool ChequeaChkBox
        {
            get
            {
                return _chequear;
            }
            set
            {
                if (_chequear == value)
                    return;
                _chequear = value;
                RaisePropertyChanged("ChequeaChkBox");
                ActivarCombo();
            }
        }

        public bool HabilitaCombo
        {
            get
            {
                return _habilitaCmb;
            }
            set
            {
                if (_habilitaCmb == value)
                    return;
                _habilitaCmb = value;
                RaisePropertyChanged("HabilitaCombo");
                ActivarCombo();
            }
        }

        public bool HabilitaCheckBox
        {
            get
            {
                return _habilitaChk;
            }
            set
            {
                if (_habilitaChk == value)
                    return;
                _habilitaChk = value;
                RaisePropertyChanged("HabilitaCheckBox");
                ActivarCheckBox();
            }
        }
        #endregion

        #region Metodos
        private async void ObtenerMotivos()
        {
            Motivos = await _servicio.ObtenerMotivosReasignacionAsync();
            MotivoSeleccionado = Motivos.FirstOrDefault();
            CargarSesionesUsuariosAsync();
        }

        public async void Reasignar(object obj)
        {
            if (BotonPresionado)
                return;
            BotonPresionado = true;

            if (HabilitaCheckBox)
            {
                if (ChequeaChkBox)
                {
                    if (SesionUsuarioSeleccionada is null)
                    {
                        return;
                    }
                    Resultado = await _servicio.ReasignarTicketsSuspendidosEspecificoAsync(_tickets, MotivoSeleccionado.ID, ((DatosLogin)App.Current.Resources["DatosLogin"]).Usuario,SesionUsuarioSeleccionada);
                    BotonPresionado = false;
                    return;
                }
            }
            Resultado = await _servicio.ReasignarTicketsSuspendidosAsync(_tickets, MotivoSeleccionado.ID, ((DatosLogin)App.Current.Resources["DatosLogin"]).Usuario);
            BotonPresionado = false;
        }

        private async void CargarSesionesUsuariosAsync()
        {
            ListaSesionesUsuarios = await _servicio.ObtenerSesionesUsuariosAsync();

            ObservableCollection<TROUBLE_DESK_USER> v_listaUser = new ObservableCollection<TROUBLE_DESK_USER>() ;

            for (int i = 0; i < ListaSesionesUsuarios.Count; i++)
            {
                v_listaUser.Add( ListaSesionesUsuarios[i].TROUBLE_DESK_USER);
            }

            SesionesUsuarios =  v_listaUser;
            SesionUsuarioSeleccionada = SesionesUsuarios.FirstOrDefault();
        }

        private void ActivarCombo()
        {
            if (ChequeaChkBox)
            {
                HabilitaCombo = true;
                CargarSesionesUsuariosAsync();
            }
            else
            {
                HabilitaCombo = false;
            }   
        }

        private void ActivarCheckBox()
        {
            if (_tickets.Count > 1)
            {
                HabilitaCheckBox  = false;
            }
            else
            {
                HabilitaCheckBox = true;
            }
        }
        #endregion
    }

}
