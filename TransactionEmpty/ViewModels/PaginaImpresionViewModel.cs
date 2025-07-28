using OnBarcode.Barcode;
using RECEPTIO.CapaPresentacion.UI.ImpresoraZebra;
using RECEPTIO.CapaPresentacion.UI.Interfaces.Impresora;
using RECEPTIO.CapaPresentacion.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using TransactionEmpty.Properties;
using TransactionEmpty.ServicioTransactionEmpty;
using TransactionEmpty.Views;

namespace TransactionEmpty.ViewModels
{
    internal class PaginaImpresionViewModel : EstadoProceso
    {
        #region Campos
        private VentanaPrincipalViewModel _viewModel;
        #endregion

        #region Constructor
        internal PaginaImpresionViewModel()
        {
            Titulo = "IMPRESION TICKET";
        }
        #endregion

        #region Propiedades
        #endregion

        #region Metodos
        internal override void EstablecerControles(VentanaPrincipalViewModel viewModel)
        {
            _viewModel = viewModel;
            Worker.DoWork += IniciarHilo;
            Worker.ProgressChanged += Progreso;
            Worker.RunWorkerCompleted += ProcesoCompletado;
            Dispatcher.Interval = new TimeSpan(0, 0, 0, 6);
            Dispatcher.Tick += Temporizador;
            Dispatcher.Stop();
            EstablecerPropiedadesViewModel();
            Mensaje = Resources.MensajeImpresion;
            Navegar();
            Procesar();
        }

        private void Temporizador(object sender, EventArgs e)
        {
            Dispatcher.Stop();
            VerificarEstadoImpresora();
        }

        private void BorrarArchivoImagenCodigoBarras()
        {
            try
            {
                var archivo = $@"{AppDomain.CurrentDomain.BaseDirectory}{_viewModel.DatosN4.PreGate.PRE_GATE_ID}";
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory))
                    File.Delete(archivo);
                if (_viewModel.DatosN4.XmlRecepcion != null)
                {
                    archivo = $@"{AppDomain.CurrentDomain.BaseDirectory}{_viewModel.DatosN4.IdPreGateRecepcion}";
                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory))
                        File.Delete(archivo);
                }
            }
            catch (Exception)
            {
            }
        }

        private void VerificarEstadoImpresora()
        {
            string v_linea = "0";
            try
            {
                if (ConfigurationManager.AppSettings["ValidarImpresora"] == "0")
                {
                    var transaccion = new KIOSK_TRANSACTION
                    {
                        IS_OK = true,
                        TRANSACTION_ID = _viewModel.DatosN4.IdTransaccion,
                        PRE_GATE_ID = _viewModel.DatosN4.PreGate.PRE_GATE_ID,
                        PROCESSES = new System.Collections.ObjectModel.ObservableCollection<PROCESS> { new PROCESS
                        {
                            STEP = "IMPRESION",
                            RESPONSE = ""
                        } },
                        KIOSK = _viewModel.Quiosco
                    };

                    transaccion.PROCESSES.FirstOrDefault().IS_OK = true;
                    transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 15;
                    _viewModel.Servicio.RegistrarProceso(transaccion);
                    BorrarArchivoImagenCodigoBarras();
                    CambiarEstado();
                }
                else
                {

                    IEstadoImpresora estadoImpresora = new EstadoImpresora();
                    _viewModel.MensajesEstadoImpresora = estadoImpresora.VerEstado();
                    v_linea = "83";
                    var transaccion = new KIOSK_TRANSACTION
                    {
                        IS_OK = true,
                        TRANSACTION_ID = _viewModel.DatosN4.IdTransaccion,
                        PROCESSES = new System.Collections.ObjectModel.ObservableCollection<PROCESS> { new PROCESS
                        {
                            STEP = "IMPRESION",
                            RESPONSE = ""
                        } },
                        KIOSK = _viewModel.Quiosco
                    };
                    v_linea = "95";
                    if (_viewModel.MensajesEstadoImpresora.Item1.Count == 0)
                    {
                        v_linea = "98";
                        transaccion.PROCESSES.FirstOrDefault().IS_OK = true;
                        transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 15; v_linea = "100";
                        _viewModel.Servicio.RegistrarProceso(transaccion); v_linea = "101";
                        BorrarArchivoImagenCodigoBarras(); v_linea = "102";
                        CambiarEstado();
                    }
                    else
                    {
                        v_linea = "107";
                        var mensajesErrores = _viewModel.MensajesEstadoImpresora.Item1.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
                        var mensajesAdvertencias = _viewModel.MensajesEstadoImpresora.Item2.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
                        v_linea = "110";
                        transaccion.PROCESSES.FirstOrDefault().RESPONSE = $"Errores: {mensajesErrores}///Advertencias : {mensajesAdvertencias}";
                        transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 16;
                        v_linea = "113";
                        _viewModel.Servicio.RegistrarProceso(transaccion);
                        v_linea = "115";
                        MostrarErrorEnPantalla();
                        v_linea = "117";
                        _viewModel.ServicioAnuncianteProblema.AnunciarProblema(_viewModel.DatosN4.IdTransaccion);
                    }
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("TransactionEmpty - PaginaImpresionViewModel - VerificarEstadoImpresora - Error:{0} - Source:{1} - Linea {2} ", ex.Message, ex.Source, v_linea);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }

        private void EstablecerPropiedadesViewModel()
        {
            ColorTextoMensaje = (System.Windows.Media.Brush)_viewModel.Convertidor.ConvertFromString("#191007");
            _viewModel.InicioBackground = System.Windows.Media.Brushes.Transparent;
            _viewModel.TicketBackground = (System.Windows.Media.Brush)_viewModel.Convertidor.ConvertFromString("#EF6C00");
            _viewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            _viewModel.PasoActual = "IMPRESION";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            _viewModel.RutaImagenInicio = @"..\Imagenes\Ingreso.png";
            _viewModel.RutaImagenTicket = @"..\Imagenes\Ticket_Blanco.png";
        }

        private void Navegar()
        {
            var paso = new PaginaMultiuso { DataContext = this };
            paso.KeepAlive = false;
            _viewModel.Contenedor.Navigate(paso);
        }

        private void Procesar()
        {
            Worker.RunWorkerAsync();
        }

        private void Progreso(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            _viewModel.EstaOcupado = true;
            _viewModel.MensajeBusy = Resources.Procesando;
        }

        private void ProcesoCompletado(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            _viewModel.EstaOcupado = false;
            if (e.Error == null)
                Dispatcher.Start();
            else
            {
                Mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 32).USER_MESSAGE;
                ColorTextoMensaje = System.Windows.Media.Brushes.Red;
                throw e.Error;
            }
        }

        private void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
            var imprimirTicket = new ImprimirTicketEntregaVacio();
            imprimirTicket.Procesar(_viewModel);
        }

        internal void MostrarErrorEnPantalla()
        {
            Mensaje = _viewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 16).USER_MESSAGE;
            ColorTextoMensaje = System.Windows.Media.Brushes.Red;
        }

        internal override void CambiarEstado()
        {
            _viewModel.SetearEstado(new PaginaInicioViewModel());
        }

        public static void LogEventos(string text)
        {
            try
            {
                string misDatos = Path.Combine(ConfigurationManager.AppSettings["RutaLog"].ToString()) + "\\LogEvent.txt";
                StreamWriter escritor;
                escritor = File.AppendText(misDatos);
                escritor.Write(DateTime.Now.ToString() + " - " + text + "\n");
                escritor.Flush();
                escritor.Close();
            }
            catch
            {
            }
        }
        #endregion
    }

    internal abstract class ImprimirTicket : IDisposable
    {
        private bool _disposed;
        protected readonly string CodigoBarra;
        protected readonly string Xml;
        protected readonly object DatosExtras;
        protected readonly Font Negrita6;
        protected readonly Font Negrita8;
        protected readonly Font Negrita12;
        protected readonly Font Normal6;
        protected readonly Font Normal8;
        protected PrintDocument PrintDocument;
        protected Pdf DocumentoPdf;

        internal ImprimirTicket(string codigoBarra, string xml, object datosExtras)
        {
            CodigoBarra = codigoBarra;
            Xml = xml;
            DatosExtras = datosExtras;
            Negrita6 = new Font("Arial", 6, FontStyle.Bold);
            Negrita8 = new Font("Arial", 8, FontStyle.Bold);
            Negrita12 = new Font("Arial", 12, FontStyle.Bold);
            Normal6 = new Font("Arial", 6);
            Normal8 = new Font("Arial", 8);
            PrintDocument = new PrintDocument();
            DocumentoPdf = new Pdf();
        }

        internal void Imprimir()
        {
            var barcode = new Linear { Type = BarcodeType.CODE39, Data = CodigoBarra };
            barcode.drawBarcode($"{CodigoBarra}");
            PrintDocument.PrintPage += EventoImprimir;
            PrintDocument.Print();
        }

        protected abstract void EventoImprimir(object sender, PrintPageEventArgs ev);

        protected abstract void Pdf();

        protected void CrearDirectorios()
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}");
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO");
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\BRBK_CFS");
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\EXPO");
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\EXPO\BRBK_CFS");
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\BANANO");
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\VACIO");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (Negrita12 != null)
                    Negrita12.Dispose();
                if (Negrita8 != null)
                    Negrita8.Dispose();
                if (Negrita6 != null)
                    Negrita6.Dispose();
                if (Normal8 != null)
                    Normal8.Dispose();
                if (Normal6 != null)
                    Normal6.Dispose();
                if (PrintDocument != null)
                {
                    PrintDocument.Dispose();
                    DocumentoPdf.Dispose();
                }
            }
            _disposed = true;
        }
    }

    internal class ImprimirEntregaVacioEntrada : ImprimirTicket
    {
        internal string Titulo;
        internal string Deposito;

        internal ImprimirEntregaVacioEntrada(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }


        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            string v_linea = "0";
            try
            {
                var datosExtras = (PRE_GATE)DatosExtras;

                if (datosExtras.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                {
                    Titulo = "ENTREGA DE CONTENEDOR CISE";
                    ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                    ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                    var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + CodigoBarra);
                    ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                    ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 170);
                    try { ev.Graphics.DrawString(datosExtras.TRUCK_LICENCE, Normal8, Brushes.Black, 120, 170); } catch { }
                    ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 190);
                    try { ev.Graphics.DrawString(datosExtras.DRIVER_ID, Normal8, Brushes.Black, 125, 190); } catch { }
                    ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 210);
                    try { ev.Graphics.DrawString(datosExtras.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 210); } catch { ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 210); }

                    ev.Graphics.DrawString("PRE-GATES:", Negrita8, Brushes.Black, 20, 250);
                    try { ev.Graphics.DrawString(datosExtras.PRE_GATE_ID.ToString(), Normal8, Brushes.Black, 125, 250); } catch { }

                    ev.HasMorePages = false;
                }
                else
                {
                    v_linea = "296";
                    ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                    ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70); v_linea = "299";
                    var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + CodigoBarra);
                    ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                    ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 170);
                    try { ev.Graphics.DrawString(datosExtras.TRUCK_LICENCE, Normal8, Brushes.Black, 140, 170); v_linea = "303"; } catch { }
                    ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 190);
                    try { ev.Graphics.DrawString(datosExtras.DRIVER_ID, Normal8, Brushes.Black, 140, 190); v_linea = "305"; } catch { }
                    ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 210);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(Xml, "driver", "driver-name"), Normal6, Brushes.Black, 140, 210); v_linea = "307"; } catch { }
                    var contador = 0;
                    var contenedores = BaseXml.ObtenerEtiquetas(Xml, "container"); v_linea = "309";

                    try
                    {
                        foreach (var contenedor in contenedores)
                        {
                            ev.Graphics.DrawString("CONTENEDOR:", Negrita8, Brushes.Black, 20, 230 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "eqid") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "eqid").Value, Normal8, Brushes.Black, 140, 230 + contador); } catch { }
                            ev.Graphics.DrawString("POSICION:", Negrita8, Brushes.Black, 20, 250 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "slot") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "slot").Value, Normal8, Brushes.Black, 140, 250 + contador); } catch { }
                            ev.Graphics.DrawString("Iso:", Negrita8, Brushes.Black, 20, 270 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "type") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "type").Value, Normal8, Brushes.Black, 140, 270 + contador); } catch { }
                            ev.Graphics.DrawString("Línea Naviera:", Negrita8, Brushes.Black, 20, 290 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "line-id") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "line-id").Value, Normal8, Brushes.Black, 140, 290 + contador); } catch { }
                            ev.Graphics.DrawString("Sello 1:", Negrita8, Brushes.Black, 20, 310 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-1") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-1").Value, Normal8, Brushes.Black, 140, 310 + contador); } catch { }
                            ev.Graphics.DrawString("Sello 2:", Negrita8, Brushes.Black, 20, 330 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-2") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-2").Value, Normal8, Brushes.Black, 140, 330 + contador); } catch { }
                            ev.Graphics.DrawString("Sello 3:", Negrita8, Brushes.Black, 20, 350 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-3") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-3").Value, Normal8, Brushes.Black, 140, 350 + contador); } catch { }
                            ev.Graphics.DrawString("Sello 4:", Negrita8, Brushes.Black, 20, 370 + contador);
                            try { ev.Graphics.DrawString(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-4") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-4").Value, Normal8, Brushes.Black, 140, 370 + contador); } catch { }
                            ev.Graphics.DrawString("Deposito:", Negrita8, Brushes.Black, 20, 390 + contador);
                            ev.Graphics.DrawString(Deposito, Negrita8, Brushes.Black, 140, 390 + contador); v_linea = "329";
                            contador = contador + 180;
                        }
                    }
                    catch (Exception e)
                    {
                        string v_error = string.Format("TransactionEmpty - PaginaImpresionViewModel - ImprimirEntregaVacioEntrada - EventoImprimir - Foreach- Linea:{0} - Error:{1} - Source:{2}", v_linea, e.Message, e.Source);
                        PaginaImpresionViewModel.LogEventos(v_error);
                    }
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("TransactionEmpty - PaginaImpresionViewModel - ImprimirEntregaVacioEntrada - EventoImprimir - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }

            try
            {
                Pdf();
            }
            catch (Exception)
            {
            }
            ev.HasMorePages = false;
        }

        protected override void Pdf()
        {
            var datosExtras = (PRE_GATE)DatosExtras;
            CrearDirectorios();
            var contenedoresStringBuilder = new StringBuilder();
            if (datosExtras.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
            {
                DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);

                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);

                DocumentoPdf.EstablecerFont("Verdana", 18, true);
                DocumentoPdf.EscribirTexto(Titulo, 20, 70);

                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(datosExtras.PRE_GATE_ID.ToString(), 20, 90);

                DocumentoPdf.EstablecerFont("Verdana", 13, false);

                DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 110);
                try { DocumentoPdf.EscribirTexto(datosExtras.TRUCK_LICENCE, 180, 110); } catch { }
                DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 130);
                try { DocumentoPdf.EscribirTexto(datosExtras.DRIVER_ID, 180, 130); } catch { }

                DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 150);
                try { DocumentoPdf.EscribirTexto(datosExtras.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), 180, 150); } catch { }
                //}

                DocumentoPdf.EscribirTexto("PRE-GATES:", 20, 170);
                try { DocumentoPdf.EscribirTexto(datosExtras.PRE_GATE_ID.ToString(), 180, 170); } catch { }
            }
            else
            {
                DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);
                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);
                DocumentoPdf.EstablecerFont("Verdana", 18, true);
                DocumentoPdf.EscribirTexto(Titulo, 20, 70);
                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(datosExtras.PRE_GATE_ID.ToString(), 20, 90);
                DocumentoPdf.EstablecerFont("Verdana", 13, false);
                DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 110);
                DocumentoPdf.EscribirTexto(datosExtras.TRUCK_LICENCE, 140, 110);
                DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 130);
                DocumentoPdf.EscribirTexto(datosExtras.DRIVER_ID, 140, 130);
                DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 150);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(Xml, "driver", "driver-name"), 140, 150);
                var contador = 0;
                var contenedores = BaseXml.ObtenerEtiquetas(Xml, "container");

                foreach (var contenedor in contenedores)
                {
                    if (string.IsNullOrEmpty(contenedoresStringBuilder.ToString()))
                        contenedoresStringBuilder.Append($"{contenedor.Attributes().FirstOrDefault(x => x.Name == "eqid").Value}");
                    else
                        contenedoresStringBuilder.Append($"-{contenedor.Attributes().FirstOrDefault(x => x.Name == "eqid").Value}");
                    DocumentoPdf.EscribirTexto("CONTENEDOR:", 20, 170 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "eqid").Value, 140, 170 + contador);
                    DocumentoPdf.EscribirTexto("POSICION:", 20, 190 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "slot") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "slot").Value, 140, 190 + contador);
                    DocumentoPdf.EscribirTexto("Iso:", 20, 210 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "type") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "type").Value, 140, 210 + contador);
                    DocumentoPdf.EscribirTexto("Línea Naviera:", 20, 230 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "line-id") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "line-id").Value, 140, 230 + contador);
                    DocumentoPdf.EscribirTexto("Sello 1:", 20, 250 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-1") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-1").Value, 140, 250 + contador);
                    DocumentoPdf.EscribirTexto("Sello 2:", 20, 270 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-2") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-2").Value, 140, 270 + contador);
                    DocumentoPdf.EscribirTexto("Sello 3:", 20, 290 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-3") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-3").Value, 140, 290 + contador);
                    DocumentoPdf.EscribirTexto("Sello 4:", 20, 310 + contador);
                    DocumentoPdf.EscribirTexto(contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-4") == null ? "" : contenedor.Attributes().FirstOrDefault(x => x.Name == "seal-4").Value, 140, 310 + contador);
                    DocumentoPdf.EscribirTexto("Deposito:", 20, 330 + contador);
                    DocumentoPdf.EscribirTexto(Deposito, 140, 330 + contador);
                    contador = contador + 180;
                }
            }
            DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\VACIO\{contenedoresStringBuilder.ToString()}-I.pdf");
        }
    }

    internal class ImprimirSalidaRecepcionVacio : ImprimirTicket
    {
        internal string Titulo;

        internal ImprimirSalidaRecepcionVacio(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            string v_linea = "0";
            try
            {

                var datosExtras = (PRE_GATE)DatosExtras;
                var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction"); v_linea = "416";

                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                //ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + CodigoBarra);
                ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 170);
                try { ev.Graphics.DrawString(datosExtras.TRUCK_LICENCE, Normal8, Brushes.Black, 140, 170); v_linea = "424"; } catch { }
                ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 190);
                try { ev.Graphics.DrawString(datosExtras.DRIVER_ID, Normal8, Brushes.Black, 140, 190); } catch { }
                ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 210); v_linea = "427";
                try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(Xml, "driver", "driver-name"), Normal6, Brushes.Black, 140, 210); } catch { }

                var contador = 0;
                foreach (var transaccion in transacciones)
                {
                    var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content"); v_linea = "433";
                    string xmlInterno = "";
                    try { xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); } catch { }
                    var danos = new StringBuilder();
                    foreach (var dano in BaseXml.ObtenerEtiquetas(transaccion.ToString(), "damage"))
                        try { danos.Append($"{dano.Attributes().FirstOrDefault(x => x.Name == "description").Value} "); } catch { }

                    try { Titulo += ((BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranCtrFreightKind").Replace("</>", "").Replace("<>", "")) == "MTY" ? " VACIO" : " EXPORTACION"); } catch { }
                    ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70); v_linea = "440";

                    ev.Graphics.DrawString("CONTENEDOR:", Negrita8, Brushes.Black, 20, 230 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 230 + contador); } catch { }
                    ev.Graphics.DrawString("Booking:", Negrita8, Brushes.Black, 20, 250 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "departure-order-nbr"), Normal8, Brushes.Black, 140, 250 + contador); } catch { }
                    ev.Graphics.DrawString("Cliente:", Negrita8, Brushes.Black, 20, 270 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranShipper").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 270 + contador); } catch { }
                    ev.Graphics.DrawString("Iso:", Negrita8, Brushes.Black, 20, 290 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "type"), Normal8, Brushes.Black, 140, 290 + contador); } catch { }
                    ev.Graphics.DrawString("Línea Naviera:", Negrita8, Brushes.Black, 20, 310 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 310 + contador); } catch { }
                    ev.Graphics.DrawString("Nave:", Negrita8, Brushes.Black, 20, 330 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvCvdCarrierVehicleName").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 330 + contador); v_linea = "453"; } catch { }
                    ev.Graphics.DrawString("Referencia:", Negrita8, Brushes.Black, 20, 350 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 350 + contador); v_linea = "455"; } catch { }
                    ev.Graphics.DrawString("Sello 1:", Negrita8, Brushes.Black, 20, 370 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-1"), Normal8, Brushes.Black, 140, 370 + contador); } catch { }
                    ev.Graphics.DrawString("Sello 2:", Negrita8, Brushes.Black, 20, 390 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-2"), Normal8, Brushes.Black, 140, 390 + contador); v_linea = "459"; } catch { }
                    ev.Graphics.DrawString("Sello 3:", Negrita8, Brushes.Black, 20, 410 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-3"), Normal8, Brushes.Black, 140, 410 + contador); } catch { }
                    ev.Graphics.DrawString("Sello 4:", Negrita8, Brushes.Black, 20, 430 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-4"), Normal8, Brushes.Black, 140, 430 + contador); v_linea = "463"; } catch { }
                    ev.Graphics.DrawString("Salida Camión:", Negrita8, Brushes.Black, 20, 450 + contador);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 140, 450 + contador);
                    ev.Graphics.DrawString("Peso Contenedor (KG):", Negrita8, Brushes.Black, 20, 470 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "gross-weight"), Normal8, Brushes.Black, 150, 470 + contador); v_linea = "467"; } catch { }
                    ev.Graphics.DrawString("Daños:", Negrita8, Brushes.Black, 20, 490 + contador);
                    try { ev.Graphics.DrawString(danos.ToString(), Normal6, Brushes.Black, 140, 490 + contador); v_linea = "469"; } catch { }
                    contador = contador + 300;
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("TransactionEmpty - PaginaImpresionViewModel - ImprimirEntregaVacioEntrada - EventoImprimir - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }

            try
            {
                Pdf();
            }
            catch (Exception)
            {
            }
            ev.HasMorePages = false;
        }

        protected override void Pdf()
        {
            var datosExtras = (PRE_GATE)DatosExtras;
            var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction");

            CrearDirectorios();
            DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);
            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);
            DocumentoPdf.EstablecerFont("Verdana", 18, true);
            DocumentoPdf.EscribirTexto(Titulo, 20, 70);
            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(datosExtras.PRE_GATE_ID.ToString(), 20, 90);
            DocumentoPdf.EstablecerFont("Verdana", 13, false);
            DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 110);
            DocumentoPdf.EscribirTexto(datosExtras.TRUCK_LICENCE, 140, 110);
            DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 130);
            DocumentoPdf.EscribirTexto(datosExtras.DRIVER_ID, 140, 130);
            DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 150);
            DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(Xml, "driver", "driver-name"), 140, 150);

            var contador = 0;
            var contenedores = new StringBuilder();
            foreach (var transaccion in transacciones)
            {
                var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content");
                var xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", "");
                var danos = new StringBuilder();
                foreach (var dano in BaseXml.ObtenerEtiquetas(transaccion.ToString(), "damage"))
                    danos.Append(BaseXml.ObtenerValorEtiqueta(dano.ToString(), "description").Replace("</>", "").Replace("<>", ""));

                DocumentoPdf.EscribirTexto("CONTENEDOR:", 20, 170 + contador);
                if (string.IsNullOrEmpty(contenedores.ToString()))
                    contenedores.Append($"{BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", "")}");
                else
                    contenedores.Append($"-{BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", "")}");
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", ""), 140, 170 + contador);
                DocumentoPdf.EscribirTexto("Booking:", 20, 190 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "departure-order-nbr"), 140, 190 + contador);
                DocumentoPdf.EscribirTexto("Cliente:", 20, 210 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranShipper").Replace("</>", "").Replace("<>", ""), 140, 210 + contador);
                DocumentoPdf.EscribirTexto("Iso:", 20, 230 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "type"), 140, 230 + contador);
                DocumentoPdf.EscribirTexto("Línea Naviera:", 20, 250 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), 140, 250 + contador);
                DocumentoPdf.EscribirTexto("Nave:", 20, 270 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvCvdCarrierVehicleName").Replace("</>", "").Replace("<>", ""), 140, 270 + contador);
                DocumentoPdf.EscribirTexto("Referencia:", 20, 290 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvId").Replace("</>", "").Replace("<>", ""), 140, 290 + contador);
                DocumentoPdf.EscribirTexto("Sello 1:", 20, 310 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-1"), 140, 310 + contador);
                DocumentoPdf.EscribirTexto("Sello 2:", 20, 330 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-2"), 140, 330 + contador);
                DocumentoPdf.EscribirTexto("Sello 3:", 20, 350 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-3"), 140, 350 + contador);
                DocumentoPdf.EscribirTexto("Sello 4:", 20, 370 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-4"), 140, 370 + contador);
                DocumentoPdf.EscribirTexto("Salida Camión:", 20, 390 + contador);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 140, 390 + contador);
                DocumentoPdf.EscribirTexto("Peso Contenedor (KG):", 20, 410 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "gross-weight"), 170, 410 + contador);
                DocumentoPdf.EscribirTexto("Daños:", 20, 430 + contador);
                DocumentoPdf.EscribirTexto(danos.ToString(), 140, 430 + contador);
                contador = contador + 300;
            }
            DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\VACIO\{contenedores.ToString()}-S.pdf");
        }
    }

    internal abstract class ImprimirTicketEntrada
    {
        protected ImprimirTicketEntrada Siguiente { get; set; }

        protected void EstablecerSiguiente(ImprimirTicketEntrada siguiente)
        {
            Siguiente = siguiente;
        }

        internal abstract void Procesar(VentanaPrincipalViewModel viewModel);
    }

    internal class ImprimirTicketEntregaVacio : ImprimirTicketEntrada
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            var titulo = "";
            string v_linea = "0";
            try
            {
                if (viewModel.DatosN4.XmlRecepcion != null)
                {
                    v_linea = "1";
                    titulo = "RECEPCION DE CONTENEDOR";// VACIO";
                    using (var impresion = new ImprimirSalidaRecepcionVacio(viewModel.DatosN4.IdPreGateRecepcion.ToString(), viewModel.DatosN4.XmlRecepcion, viewModel.DatosN4.PreGate) { Titulo = titulo })
                        impresion.Imprimir();

                    v_linea = "2";
                }
                titulo = "ENTREGA DE CONTENEDOR VACIO";

                string v_deposito = string.Empty;
                string[] v_cadena;
                try
                {
                    v_cadena = viewModel.DatosN4.PreGate.PRE_GATE_DETAILS.Where(p => p.REFERENCE_ID == "I").FirstOrDefault().TRANSACTION_NUMBER.ToString().Split('-');
                    v_linea = "3";
                    if (viewModel.DatosN4.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                    {
                        v_deposito = "CISE – OPACIF";//viewModel.ObtenerDeposito(int.Parse(v_cadena[0]));
                    }
                    else
                    {
                        v_deposito = viewModel.ObtenerDeposito(int.Parse(v_cadena[5]));
                    }
                    v_linea = "4";
                }
                catch
                {
                    v_deposito = string.Empty;
                }
                v_linea = "5";
                using (var impresion = new ImprimirEntregaVacioEntrada(viewModel.DatosN4.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, viewModel.DatosN4.PreGate) { Titulo = titulo, Deposito = v_deposito })
                    impresion.Imprimir();
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Error al intentar imprimir ticket en TransactionEmpty - PaginaImpresionViewModel - ImprimirEntregaVacioEntrada - EventoImprimir - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }

        }
    }
}