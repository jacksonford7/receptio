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
using Transaction.Properties;
using Transaction.ServicioTransaction;
using Transaction.Views;

namespace Transaction.ViewModels
{
    internal class PaginaImpresionViewModel : EstadoProceso
    {
        #region Campos
        internal VentanaPrincipalViewModel ViewModel;
        private bool _esSegundoCiclo;
        #endregion

        #region Constructor
        internal PaginaImpresionViewModel()
        {
            EsVisibleBoton = System.Windows.Visibility.Collapsed;
            Titulo = "IMPRESION TICKET";
        }
        #endregion

        #region Propiedades
        #endregion

        #region Metodos
        internal override void EstablecerControles(VentanaPrincipalViewModel viewModel)
        {
            ViewModel = viewModel;
            Worker.DoWork += IniciarHilo;
            Worker.ProgressChanged += Progreso;
            Worker.RunWorkerCompleted += ProcesoCompletado;
            Dispatcher.Tick += Temporizador;
            Dispatcher.Stop();
            EstablecerPropiedadesViewModel();
            Mensaje = Resources.MensajeImpresion;
            Navegar();
            Procesar();
        }

        private void Temporizador(object sender, EventArgs e)
        {
            if (_esSegundoCiclo)
            {
                Dispatcher.Stop();
                VerificarEstadoImpresora();
                BorrarArchivoImagenCodigoBarras();
                _esSegundoCiclo = false;
            }
            else
            {
                _esSegundoCiclo = true;
            }
        }

        private void BorrarArchivoImagenCodigoBarras()
        {
            var archivo = $@"{AppDomain.CurrentDomain.BaseDirectory}ImagenCodigoBarra";
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory))
                File.Delete(archivo);
        }

        protected virtual void VerificarEstadoImpresora()
        {
            if (ConfigurationManager.AppSettings["ValidarImpresora"] == "0")
            {
                var transaccion = new KIOSK_TRANSACTION
                {
                    TRANSACTION_ID = ViewModel.DatosPreGate.IdTransaccion,
                    PRE_GATE_ID = ViewModel.DatosPreGate.PreGate.PRE_GATE_ID,
                    PROCESSES = new List<PROCESS> { new PROCESS
                        {
                            STEP = "IMPRESION",
                            RESPONSE = ""
                        } },
                    KIOSK = ViewModel.Quiosco
                };

                transaccion.PROCESSES.FirstOrDefault().IS_OK = true;
                transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 15;
                ViewModel.Servicio.RegistrarProceso(transaccion);
                CambiarEstado();
            }
            else
            {
                IEstadoImpresora estadoImpresora = new EstadoImpresora();
                ViewModel.MensajesEstadoImpresora = estadoImpresora.VerEstado();
                var transaccion = new KIOSK_TRANSACTION
                {
                    TRANSACTION_ID = ViewModel.DatosPreGate.IdTransaccion,
                    PRE_GATE_ID = ViewModel.DatosPreGate.PreGate.PRE_GATE_ID,
                    PROCESSES = new List<PROCESS> { new PROCESS
                            {
                                STEP = "IMPRESION",
                                RESPONSE = ""
                            } },
                    KIOSK = ViewModel.Quiosco
                };
                if (ViewModel.MensajesEstadoImpresora.Item1.Count == 0)
                {
                    transaccion.PROCESSES.FirstOrDefault().IS_OK = true;
                    transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 15;
                    ViewModel.Servicio.RegistrarProceso(transaccion);
                    CambiarEstado();
                }
                else
                {
                    var mensajesErrores = ViewModel.MensajesEstadoImpresora.Item1.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
                    var mensajesAdvertencias = ViewModel.MensajesEstadoImpresora.Item2.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
                    transaccion.PROCESSES.FirstOrDefault().RESPONSE = $"Errores: {mensajesErrores}///Advertencias : {mensajesAdvertencias}";
                    transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 16;
                    ViewModel.Servicio.RegistrarProceso(transaccion);
                    MostrarErrorEnPantalla();
                    ViewModel.ServicioAnuncianteProblema.AnunciarProblema(ViewModel.DatosPreGate.IdTransaccion);
                }
            }
        }

        private void EstablecerPropiedadesViewModel()
        {
            ColorTextoMensaje = (System.Windows.Media.Brush)ViewModel.Convertidor.ConvertFromString("#191007");
            ViewModel.InicioBackground = System.Windows.Media.Brushes.Transparent;
            ViewModel.HuellaBackground = System.Windows.Media.Brushes.Transparent;
            ViewModel.RfidBackground = System.Windows.Media.Brushes.Transparent;
            ViewModel.ProcesoBackground = System.Windows.Media.Brushes.Transparent;
            ViewModel.TicketBackground = (System.Windows.Media.Brush)ViewModel.Convertidor.ConvertFromString("#EF6C00");
            ViewModel.BarreraBackground = System.Windows.Media.Brushes.Transparent;
            ViewModel.Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            ViewModel.PuedoIrHome = false;
            ViewModel.PasoActual = "IMPRESION";
            EstablecerIconos();
        }

        private void EstablecerIconos()
        {
            ViewModel.RutaImagenInicio = @"..\Imagenes\Ingreso.png";
            ViewModel.RutaImagenHuella = @"..\Imagenes\Huella.png";
            ViewModel.RutaImagenRfid = @"..\Imagenes\Rfid.png";
            ViewModel.RutaImagenProceso = @"..\Imagenes\Proceso.png";
            ViewModel.RutaImagenTicket = @"..\Imagenes\Ticket_Blanco.png";
            ViewModel.RutaImagenBarrera = @"..\Imagenes\Barrera.png";
        }

        private void Navegar()
        {
            var paso = new PaginaMultiuso { DataContext = this };
            paso.KeepAlive = false;
            ViewModel.Contenedor.Navigate(paso);
        }

        private void Procesar()
        {
            Worker.RunWorkerAsync();
        }

        internal void Progreso(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ViewModel.EstaOcupado = true;
            ViewModel.MensajeBusy = Resources.Procesando;
        }

        internal void ProcesoCompletado(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            ViewModel.EstaOcupado = false;
            if (e.Error == null)
                Dispatcher.Start();
            else
            {
                Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 32).USER_MESSAGE;
                ColorTextoMensaje = System.Windows.Media.Brushes.Red;
                throw e.Error;
            }
        }

        protected virtual void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
            var imprimirTicketEntrada = new ImprimirEntradaDeliveryImportFull();
            imprimirTicketEntrada.Procesar(ViewModel);
        }

        internal void MostrarErrorEnPantalla()
        {
            Mensaje = ViewModel.Mensajes.FirstOrDefault(m => m.MESSAGE_ID == 16).USER_MESSAGE;
            ColorTextoMensaje = System.Windows.Media.Brushes.Red;
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaBarreraViewModel());
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

    internal class PaginaSalidaImpresionViewModel : PaginaImpresionViewModel
    {
        #region Campos
        #endregion

        #region Constructor
        internal PaginaSalidaImpresionViewModel() : base()
        {
        }
        #endregion

        #region Propiedades
        #endregion

        #region Metodos
        protected override void VerificarEstadoImpresora()
        {
            if (ConfigurationManager.AppSettings["ValidarImpresora"] == "0")
            {
                var transaccion = new KIOSK_TRANSACTION
                {
                    TRANSACTION_ID = Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion),
                    PRE_GATE_ID = ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID,
                    PROCESSES = new List<PROCESS> { new PROCESS
                        {
                            STEP = "IMPRESION",
                            RESPONSE = ""
                        } },
                    KIOSK = ViewModel.Quiosco
                };

                transaccion.PROCESSES.FirstOrDefault().IS_OK = true;
                transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 15;
                ViewModel.Servicio.RegistrarProceso(transaccion);
                CambiarEstado();
            }
            else
            {
                IEstadoImpresora estadoImpresora = new EstadoImpresora();
                ViewModel.MensajesEstadoImpresora = estadoImpresora.VerEstado();
                var transaccion = new KIOSK_TRANSACTION
                {
                    TRANSACTION_ID = Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion),
                    PRE_GATE_ID = ViewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID,
                    PROCESSES = new List<PROCESS> { new PROCESS
                            {
                                STEP = "IMPRESION",
                                RESPONSE = ""
                            } },
                    KIOSK = ViewModel.Quiosco
                };
                if (ViewModel.MensajesEstadoImpresora.Item1.Count == 0)
                {
                    transaccion.PROCESSES.FirstOrDefault().IS_OK = true;
                    transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 15;
                    ViewModel.Servicio.RegistrarProceso(transaccion);
                    CambiarEstado();
                }
                else
                {
                    var mensajesErrores = ViewModel.MensajesEstadoImpresora.Item1.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
                    var mensajesAdvertencias = ViewModel.MensajesEstadoImpresora.Item2.Aggregate("", (actual, item) => (actual == "" ? "" : actual + ",") + item);
                    transaccion.PROCESSES.FirstOrDefault().RESPONSE = $"Errores: {mensajesErrores}///Advertencias : {mensajesAdvertencias}";
                    transaccion.PROCESSES.FirstOrDefault().MESSAGE_ID = 16;
                    ViewModel.Servicio.RegistrarProceso(transaccion);
                    MostrarErrorEnPantalla();
                    ViewModel.ServicioAnuncianteProblema.AnunciarProblema(Convert.ToInt32(ViewModel.DatosPreGateSalida.IdTransaccion));
                }
            }
        }

        protected override void IniciarHilo(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Worker.ReportProgress(0);
            var imprimirTicketSalida = new ImprimirSalidaDeliveryImportFull();
            imprimirTicketSalida.Procesar(ViewModel);
        }

        internal override void CambiarEstado()
        {
            ViewModel.SetearEstado(new PaginaSalidaBarreraViewModel());
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
            barcode.drawBarcode("ImagenCodigoBarra");
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

    internal class ImprimirDeliveryImportReceiveExportFullDrayOffReceiveEmpty : ImprimirTicket //1 3 6 7 10
    {
        internal string Titulo;

        internal ImprimirDeliveryImportReceiveExportFullDrayOffReceiveEmpty(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            string v_linea = "359";
            try
            {
                var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
                var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction");
                v_linea = "364";
                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 170);
                try { ev.Graphics.DrawString(datosExtras.Item1.TRUCK_LICENCE, Normal8, Brushes.Black, 120, 170); } catch { }
                ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 190);
                try { ev.Graphics.DrawString(datosExtras.Item1.DRIVER_ID, Normal8, Brushes.Black, 125, 190); } catch { }
                ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 210);
                try { ev.Graphics.DrawString($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", Normal6, Brushes.Black, 125, 210); } catch { }
                v_linea = "376";
                var contador = 0;
                foreach (var transaccion in transacciones)
                {
                    var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content").FirstOrDefault();
                    string xmlInterno;
                    try { xmlInterno = contenidoInterno.ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); } catch { xmlInterno = ""; }
                    v_linea = "382";
                    ev.Graphics.DrawString("CONTENEDOR:", Negrita8, Brushes.Black, 20, 230 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", ""), Negrita8, Brushes.Black, 120, 230 + contador); v_linea = "384"; } catch { }
                    ev.Graphics.DrawString("POSICION:", Negrita8, Brushes.Black, 20, 250 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "posLocId").Replace("Y-CGSA-", "").Replace("</>", "").Replace("<>", ""), Negrita8, Brushes.Black, 120, 250 + contador); v_linea = "386"; } catch { }
                    ev.Graphics.DrawString("CLIENTE:", Negrita8, Brushes.Black, 20, 270 + contador); v_linea = "387";
                    try
                    {
                        if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                            try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranConsigneeName").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 120, 270 + contador); } catch { }
                        else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                            try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranShipper").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 120, 270 + contador); } catch { }
                        else
                            try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "unit-flex-string-11"), Normal8, Brushes.Black, 120, 270 + contador); } catch { }
                    }
                    catch { }
                    ev.Graphics.DrawString("LINEA:", Negrita8, Brushes.Black, 20, 290 + contador); v_linea = "394";
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 120, 290 + contador); } catch { }
                    ev.Graphics.DrawString("SELLO 1:", Negrita8, Brushes.Black, 20, 310 + contador); v_linea = "396";
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-1"), Normal8, Brushes.Black, 120, 310 + contador); } catch { }
                    ev.Graphics.DrawString("SELLO 2:", Negrita8, Brushes.Black, 20, 330 + contador); v_linea = "398";
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-2"), Normal8, Brushes.Black, 120, 330 + contador); } catch { }
                    ev.Graphics.DrawString("SELLO 3:", Negrita8, Brushes.Black, 20, 350 + contador); v_linea = "400";
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-3"), Normal8, Brushes.Black, 120, 350 + contador); } catch { }
                    ev.Graphics.DrawString("SELLO 4:", Negrita8, Brushes.Black, 20, 370 + contador); v_linea = "402";
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-4"), Normal8, Brushes.Black, 120, 370 + contador); } catch { }

                    contador = contador + 160;
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirDeliveryImportReceiveExportFullDrayOffReceiveEmpty - EventoImprimir - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
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
            var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
            var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction");

            CrearDirectorios();
            DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);

            DocumentoPdf.EstablecerFont("Verdana", 18, true);
            DocumentoPdf.EscribirTexto(Titulo, 20, 70);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 20, 90);

            DocumentoPdf.EstablecerFont("Verdana", 13, false);
            DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 110);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 140, 110);

            DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 130);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 140, 130);

            DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 150);
            DocumentoPdf.EscribirTexto($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", 140, 150);

            var contador = 0;
            var contenedores = new StringBuilder();
            foreach (var transaccion in transacciones)
            {
                var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content").FirstOrDefault();
                var xmlInterno = contenidoInterno.ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", "");

                DocumentoPdf.EscribirTexto("CONTENEDOR:", 20, 170 + contador);
                if (string.IsNullOrEmpty(contenedores.ToString()))
                    contenedores.Append($"{BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", "")}");
                else
                    contenedores.Append($"-{BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", "")}");
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", ""), 140, 170 + contador);

                DocumentoPdf.EscribirTexto("POSICION:", 20, 190 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "posLocId").Replace("Y-CGSA-", "").Replace("</>", "").Replace("<>", ""), 140, 190 + contador);

                DocumentoPdf.EscribirTexto("CLIENTE:", 20, 210 + contador);
                if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranConsigneeName").Replace("</>", "").Replace("<>", ""), 140, 210 + contador);
                else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranShipper").Replace("</>", "").Replace("<>", ""), 140, 210 + contador);
                else
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "unit-flex-string-11").Replace("<>", ""), 140, 210 + contador);

                DocumentoPdf.EscribirTexto("LINEA:", 20, 230 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), 140, 230 + contador);

                DocumentoPdf.EscribirTexto("SELLO 1:", 20, 250 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-1"), 140, 250 + contador);

                DocumentoPdf.EscribirTexto("SELLO 2:", 20, 270 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-2"), 140, 270 + contador);

                DocumentoPdf.EscribirTexto("SELLO 3:", 20, 290 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-3"), 140, 290 + contador);

                DocumentoPdf.EscribirTexto("SELLO 4:", 20, 310 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-4"), 140, 310 + contador);
                contador = contador + 160;
            }

            if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6))
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\{contenedores.ToString()}-I.pdf");
            else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\EXPO\{contenedores.ToString()}-I.pdf");
            else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\VACIO\{contenedores.ToString()}-I.pdf");

        }
    }

    internal class ImprimirSalidaDeliveryImportReceiveExportFullReceiveEmpty : ImprimirTicket
    {
        internal string Titulo;
        internal string Deposito;

        internal ImprimirSalidaDeliveryImportReceiveExportFullReceiveEmpty(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            string v_linea = "514";
            try
            {
                var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
                var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction");
                v_linea = "519";
                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 170);
                ev.Graphics.DrawString(datosExtras != null ? datosExtras.Item1 != null ? string.IsNullOrEmpty(datosExtras.Item1.TRUCK_LICENCE) ? string.Empty : datosExtras.Item1.TRUCK_LICENCE : "" : "", Normal8, Brushes.Black, 140, 170); v_linea = "526";
                ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 190);
                ev.Graphics.DrawString(datosExtras != null ? datosExtras.Item1 != null ? string.IsNullOrEmpty(datosExtras.Item1.DRIVER_ID) ? string.Empty : datosExtras.Item1.DRIVER_ID : "" : "", Normal8, Brushes.Black, 140, 190); v_linea = "528";
                ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 210);
                try { ev.Graphics.DrawString($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", Normal6, Brushes.Black, 140, 210); v_linea = "530"; } catch { };

                var contador = 0;
                foreach (var transaccion in transacciones)
                {
                    var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content"); v_linea = "535";
                    string xmlInterno;
                    try { xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); v_linea = "536"; } catch { xmlInterno = ""; }
                    var danos = new List<string>();

                    try
                    {
                        foreach (var dano in BaseXml.ObtenerEtiquetas(transaccion.ToString(), "damage"))
                            try { danos.Add($"{dano.Attributes().FirstOrDefault(x => x.Name == "description").Value} - {dano.Attributes().FirstOrDefault(x => x.Name == "type-description").Value} - {dano.Attributes().FirstOrDefault(x => x.Name == "component-description").Value}"); } catch { }
                    }
                    catch { }

                    ev.Graphics.DrawString("CONTENEDOR:", Negrita8, Brushes.Black, 20, 230 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 230 + contador); v_linea = "542"; } catch { }
                    try
                    {
                        if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                        {
                            ev.Graphics.DrawString("Número Carga:", Negrita8, Brushes.Black, 20, 250 + contador);
                            try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "blNbr").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 250 + contador); v_linea = "546"; } catch { }
                        }
                        else
                        {
                            ev.Graphics.DrawString("Booking:", Negrita8, Brushes.Black, 20, 250 + contador);
                            if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16))
                            {
                                try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranEqoNbr").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 250 + contador); v_linea = "553"; } catch { }
                            }
                            else
                            {
                                try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "departure-order-nbr"), Normal8, Brushes.Black, 140, 250 + contador); v_linea = "557"; } catch { }
                            }
                        }
                    }
                    catch { }
                    try
                    {
                        ev.Graphics.DrawString("Cliente:", Negrita8, Brushes.Black, 20, 270 + contador); v_linea = "560";
                        if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                            try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranConsigneeName").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 270 + contador); } catch { }
                        else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                            try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranShipper").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 270 + contador); } catch { }
                        else
                            try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "unit-flex-string-11"), Normal6, Brushes.Black, 140, 270 + contador); } catch { }
                        ev.Graphics.DrawString("Iso:", Negrita8, Brushes.Black, 20, 290 + contador);
                        try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "type"), Normal8, Brushes.Black, 140, 290 + contador); } catch { }
                        ev.Graphics.DrawString("Línea Naviera:", Negrita8, Brushes.Black, 20, 310 + contador);
                        try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 310 + contador); } catch { }
                        ev.Graphics.DrawString("Nave:", Negrita8, Brushes.Black, 20, 330 + contador);
                        if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                            try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranUnitFlexString12").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 330 + contador); } catch { }
                        else
                            try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvCvdCarrierVehicleName").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 330 + contador); } catch { }
                    }
                    catch { }

                    ev.Graphics.DrawString("Referencia:", Negrita8, Brushes.Black, 20, 350 + contador); v_linea = "576";
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 140, 350 + contador); } catch { }
                    ev.Graphics.DrawString("Sello 1:", Negrita8, Brushes.Black, 20, 370 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-1"), Normal8, Brushes.Black, 140, 370 + contador); } catch { }
                    ev.Graphics.DrawString("Sello 2:", Negrita8, Brushes.Black, 20, 390 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-2"), Normal8, Brushes.Black, 140, 390 + contador); } catch { }
                    ev.Graphics.DrawString("Sello 3:", Negrita8, Brushes.Black, 20, 410 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-3"), Normal8, Brushes.Black, 140, 410 + contador); } catch { }
                    ev.Graphics.DrawString("Sello 4:", Negrita8, Brushes.Black, 20, 430 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "seal-4"), Normal8, Brushes.Black, 140, 430 + contador); } catch { }
                    ev.Graphics.DrawString("Ingreso Camión:", Negrita8, Brushes.Black, 20, 450 + contador);
                    try { ev.Graphics.DrawString(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(t => t.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 140, 450 + contador); } catch { }
                    ev.Graphics.DrawString("Salida Camión:", Negrita8, Brushes.Black, 20, 470 + contador);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 140, 470 + contador);
                    ev.Graphics.DrawString("Peso Contenedor (KG):", Negrita8, Brushes.Black, 20, 490 + contador);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "gross-weight"), Normal8, Brushes.Black, 150, 490 + contador); } catch { }
                    v_linea = "592";
                    ev.Graphics.DrawString("Deposito:", Negrita8, Brushes.Black, 20, 510 + contador);
                    ev.Graphics.DrawString(Deposito, Normal8, Brushes.Black, 140, 510 + contador);
                    int v_lineaActual = 530 + contador;
                    ev.Graphics.DrawString("Daños:", Negrita8, Brushes.Black, 20, 530 + contador);

                    for (int j = 0; j < danos.Count(); j++)
                    {
                        try { ev.Graphics.DrawString(danos[j], Normal6, Brushes.Black, 70, 530 + (j * 10) + contador); v_lineaActual = 530 + (j * 10) + contador; } catch { }
                    }

                    ev.Graphics.DrawString("Servicio combustible Gasolinera junto", Negrita8, Brushes.Black, 20, v_lineaActual + 20 + contador); v_linea = "620";
                    ev.Graphics.DrawString("a SENAE Av. 25 de Julio. 24 Horas!", Negrita8, Brushes.Black, 20, v_lineaActual + 30 + contador); v_linea = "621";
                    ev.Graphics.DrawString("NWT - Normal Wear and Tear", Negrita8, Brushes.Black, 20, v_lineaActual + 50 + contador); v_linea = "622";

                    contador = contador + 320;
                    v_linea = "626";
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirSalidaDeliveryImportReceiveExportFullReceiveEmpty - EventoImprimir - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
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
            var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
            var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction");

            CrearDirectorios();
            DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);
            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);
            DocumentoPdf.EstablecerFont("Verdana", 18, true);
            DocumentoPdf.EscribirTexto(Titulo, 20, 70);
            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            try { DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 20, 90); } catch { }
            DocumentoPdf.EstablecerFont("Verdana", 13, false);
            DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 110);
            try { DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 140, 110); } catch { }
            DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 130);
            try { DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 140, 130); } catch { }
            DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 150);
            try { DocumentoPdf.EscribirTexto($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", 140, 150); } catch { }

            var contador = 0;
            var contenedores = new StringBuilder();
            foreach (var transaccion in transacciones)
            {
                var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content");
                var xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", "");
                var danos = new List<string>();
                foreach (var dano in BaseXml.ObtenerEtiquetas(transaccion.ToString(), "damage"))
                    danos.Add($"{dano.Attributes().FirstOrDefault(x => x.Name == "description").Value} - {dano.Attributes().FirstOrDefault(x => x.Name == "type-description").Value} - {dano.Attributes().FirstOrDefault(x => x.Name == "component-description").Value}");

                DocumentoPdf.EscribirTexto("CONTENEDOR:", 20, 170 + contador);

                if (string.IsNullOrEmpty(contenedores.ToString()))
                    contenedores.Append($"{BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", "")}");
                else
                    contenedores.Append($"-{BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", "")}");
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "unitId").Replace("</>", "").Replace("<>", ""), 140, 170 + contador);
                try
                {
                    if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                    {
                        DocumentoPdf.EscribirTexto("Número Carga:", 20, 190 + contador);
                        try { DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "blNbr").Replace("</>", "").Replace("<>", ""), 140, 190 + contador); } catch { }
                    }
                    else
                    {
                        DocumentoPdf.EscribirTexto("Booking:", 20, 190 + contador);
                        try { DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "departure-order-nbr"), 140, 190 + contador); } catch { }
                    }
                }
                catch { }
                DocumentoPdf.EscribirTexto("Cliente:", 20, 210 + contador);
                if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranConsigneeName").Replace("</>", "").Replace("<>", ""), 140, 210 + contador);
                else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranShipper").Replace("</>", "").Replace("<>", ""), 140, 210 + contador);
                else
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "unit-flex-string-11").Replace("<>", ""), 140, 210 + contador);
                DocumentoPdf.EscribirTexto("Iso:", 20, 230 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "type"), 140, 230 + contador);
                DocumentoPdf.EscribirTexto("Línea Naviera:", 20, 250 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), 140, 250 + contador);
                DocumentoPdf.EscribirTexto("Nave:", 20, 270 + contador);
                if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranUnitFlexString12").Replace("</>", "").Replace("<>", ""), 140, 270 + contador);
                else
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
                DocumentoPdf.EscribirTexto("Ingreso Camión:", 20, 390 + contador);
                try { DocumentoPdf.EscribirTexto(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(t => t.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), 140, 390 + contador); } catch { }
                DocumentoPdf.EscribirTexto("Salida Camión:", 20, 410 + contador);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 140, 410 + contador);
                DocumentoPdf.EscribirTexto("Peso Contenedor (KG):", 20, 430 + contador);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "container", "gross-weight"), 170, 430 + contador);

                DocumentoPdf.EscribirTexto("Deposito:", 20, 450 + contador);
                DocumentoPdf.EscribirTexto(Deposito, 140, 450 + contador);

                DocumentoPdf.EscribirTexto("Daños:", 20, 470 + contador);

                int v_lineaActual = 470 + contador;
                for (int j = 0; j < danos.Count(); j++)
                {
                    v_lineaActual = 470 + (j * 10) + contador;
                    try { DocumentoPdf.EscribirTexto(danos[j], 140, v_lineaActual); } catch { }
                }

                DocumentoPdf.EscribirTexto("Servicio combustible Gasolinera junto", 20, v_lineaActual + 20 + contador);
                DocumentoPdf.EscribirTexto("a SENAE Av. 25 de Julio. 24 Horas!", 20, v_lineaActual + 30 + contador);
                DocumentoPdf.EscribirTexto("NWT - Normal Wear and Tear", 20, v_lineaActual + 50 + contador);


                contador = contador + 320;
            }
            if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6))
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\{contenedores.ToString()}-S.pdf");
            else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\EXPO\{contenedores.ToString()}-S.pdf");
            else if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\VACIO\{contenedores.ToString()}-S.pdf");
        }
    }

    internal class ImprimirSalidaDeliveryImportGuiaDesinfeccion : ImprimirTicket
    {
        internal ImprimirSalidaDeliveryImportGuiaDesinfeccion(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            try
            {
                var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
                var contenidoInterno = BaseXml.ObtenerEtiquetas(Xml, "content");
                string xmlInterno;
                try { xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); } catch { xmlInterno = ""; }
                string contenedor;
                try { contenedor = BaseXml.ObtenerDatoDeXml(Xml, "container", "eqid"); } catch { contenedor = ""; }

                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                ev.Graphics.DrawString("GUIA DE DESINFECCION", Negrita8, Brushes.Black, 20, 70);
                ev.Graphics.DrawString("Contenedor:", Negrita6, Brushes.Black, 20, 90);
                ev.Graphics.DrawString(contenedor, Normal6, Brushes.Black, 140, 90);
                ev.Graphics.DrawString("Provincia:", Negrita6, Brushes.Black, 20, 110);
                ev.Graphics.DrawString("Guayas", Normal6, Brushes.Black, 140, 110);
                ev.Graphics.DrawString("Cantón:", Negrita6, Brushes.Black, 20, 130);
                ev.Graphics.DrawString("Guayaquil", Normal6, Brushes.Black, 140, 130);
                ev.Graphics.DrawString("Parroquia:", Negrita6, Brushes.Black, 20, 150);
                ev.Graphics.DrawString("Ximena", Normal6, Brushes.Black, 140, 150);
                ev.Graphics.DrawString("Sitio:", Negrita6, Brushes.Black, 20, 170);
                ev.Graphics.DrawString("Puerto Libertador Simón Bolivar", Normal6, Brushes.Black, 140, 170);
                ev.Graphics.DrawString("Puerto de Origen:", Negrita6, Brushes.Black, 20, 190);
                try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "pointId").Replace("</>", "").Replace("<>", ""), Normal6, Brushes.Black, 140, 190); } catch { }
                ev.Graphics.DrawString("Placa:", Negrita6, Brushes.Black, 20, 210);
                try { ev.Graphics.DrawString(datosExtras.Item1.TRUCK_LICENCE, Normal6, Brushes.Black, 140, 210); } catch { }
                ev.Graphics.DrawString("No. Guía:", Negrita6, Brushes.Black, 20, 230);
                try
                {
                    if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                        try { ev.Graphics.DrawString(datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault(pgd => pgd.CONTAINERS.Any(c => c.NUMBER == contenedor)).ID.ToString(), Normal6, Brushes.Black, 140, 230); } catch { }
                    else
                        ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(Xml, "container", "unit-flex-string-6"), Normal6, Brushes.Black, 140, 230);
                }
                catch { }
                ev.Graphics.DrawString("Cédula Chofer:", Negrita6, Brushes.Black, 20, 250);
                try { ev.Graphics.DrawString(datosExtras.Item1.DRIVER_ID, Normal6, Brushes.Black, 140, 250); } catch { }
                ev.Graphics.DrawString("Mombre Chofer:", Negrita6, Brushes.Black, 20, 270);
                try { ev.Graphics.DrawString($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", Normal6, Brushes.Black, 140, 270); } catch { }
                ev.Graphics.DrawString("RECIBÍ CONFORME Y EN BUENAS CONDICIONES DE ACUERDO A LO", Normal6, Brushes.Black, 20, 290);
                ev.Graphics.DrawString("INDICADO EN ESTE DOCUMENTO POR PARTE DE CONTECON", Normal6, Brushes.Black, 20, 300);

                int v_lineaActual = 310;


                ev.Graphics.DrawString("Servicio combustible Gasolinera junto", Negrita8, Brushes.Black, 20, v_lineaActual + 10);
                ev.Graphics.DrawString("a SENAE Av. 25 de Julio. 24 Horas!", Negrita8, Brushes.Black, 20, v_lineaActual + 20);

            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirSalidaDeliveryImportGuiaDesinfeccion - EventoImprimir - Error:{0} - Source:{1}", ex.Message, ex.Source);
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
            var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
            var contenidoInterno = BaseXml.ObtenerEtiquetas(Xml, "content");
            var xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", "");
            var contenedor = BaseXml.ObtenerDatoDeXml(Xml, "container", "eqid");

            CrearDirectorios();
            DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);
            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);
            DocumentoPdf.EstablecerFont("Verdana", 18, true);
            DocumentoPdf.EscribirTexto("GUIA DE DESINFECCION", 20, 70);
            DocumentoPdf.EstablecerFont("Verdana", 13, false);
            DocumentoPdf.EscribirTexto("CONTENEDOR:", 20, 110);
            DocumentoPdf.EscribirTexto(contenedor, 140, 110);
            DocumentoPdf.EscribirTexto("PROVINCIA:", 20, 130);
            DocumentoPdf.EscribirTexto("GUAYAS", 140, 130);
            DocumentoPdf.EscribirTexto("CANTON:", 20, 150);
            DocumentoPdf.EscribirTexto("GUAYAQUIL", 140, 150);
            DocumentoPdf.EscribirTexto("PARROQUIA:", 20, 170);
            DocumentoPdf.EscribirTexto("XIMENA", 140, 170);
            DocumentoPdf.EscribirTexto("SITIO:", 20, 190);
            DocumentoPdf.EscribirTexto("PUERTO LIBERTADOR SIMON BOLIVAR", 140, 190);
            DocumentoPdf.EscribirTexto("PUERTO ORIGEN:", 20, 210);
            DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "pointId").Replace("</>", "").Replace("<>", ""), 140, 210);
            DocumentoPdf.EscribirTexto("PLACA:", 20, 230);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 140, 230);
            DocumentoPdf.EscribirTexto("No. GUIA:", 20, 250);
            if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault(pgd => pgd.CONTAINERS.Any(c => c.NUMBER == contenedor)).ID.ToString(), 140, 250);
            else
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(Xml, "container", "unit-flex-string-6"), 140, 250);
            DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 270);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 140, 270);
            DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 290);
            DocumentoPdf.EscribirTexto($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", 140, 290);
            DocumentoPdf.EscribirTexto("RECIBÍ CONFORME Y EN BUENAS CONDICIONES DE ACUERDO A LO INDICADO EN ESTE DOCUMENTO POR PARTE DE CONTECON", 20, 310);
            if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1))
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\{contenedor}-GUIA_DESINFECCION.pdf");
            else
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\VACIO\{contenedor}-GUIA_DESINFECCION.pdf");
        }
    }

    internal class ImprimirReceiveExportBrBkCfsBanano : ImprimirTicket // 4 5 8 9
    {
        internal string Titulo;

        internal ImprimirReceiveExportBrBkCfsBanano(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            try
            {
                var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
                var transaccion = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction").FirstOrDefault();

                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                ev.Graphics.DrawString("AISV:", Negrita8, Brushes.Black, 20, 170);
                ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr"), Negrita8, Brushes.Black, 120, 170);
                ev.Graphics.DrawString("DAE:", Negrita8, Brushes.Black, 20, 190);
                try { ev.Graphics.DrawString(datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault().DOCUMENT_ID ?? "", Normal8, Brushes.Black, 120, 190); } catch { }
                ev.Graphics.DrawString("NUMERO CAJAS:", Negrita8, Brushes.Black, 20, 210);
                try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "cargo", "item-quantity"), Normal8, Brushes.Black, 120, 210); } catch { }

                var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content").FirstOrDefault();
                string xmlInterno = "";
                try { xmlInterno = contenidoInterno.ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); } catch { }

                ev.Graphics.DrawString("LINEA:", Negrita8, Brushes.Black, 20, 230);
                try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 120, 230); } catch { }
                ev.Graphics.DrawString("NAVE:", Negrita8, Brushes.Black, 20, 250);
                try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvCvdCarrierVehicleName").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 120, 250); } catch { }
                ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 270);
                try { try { ev.Graphics.DrawString(datosExtras.Item1.TRUCK_LICENCE, Normal8, Brushes.Black, 120, 270); } catch { } } catch { }
                ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 290);
                try { ev.Graphics.DrawString(datosExtras.Item1.DRIVER_ID, Normal8, Brushes.Black, 125, 290); } catch { }
                ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 310);
                try { ev.Graphics.DrawString($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", Normal6, Brushes.Black, 125, 310); } catch { }

                if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                {
                    ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 330);
                    try { ev.Graphics.DrawString(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 330); } catch { }
                    ev.Graphics.DrawString("SALIDA CAMION:", Negrita8, Brushes.Black, 20, 350);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 350);
                }
                else
                {
                    ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 330);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 330);
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirReceiveExportBrBkCfsBanano - EventoImprimir - Error:{0} - Source:{1}", ex.Message, ex.Source);
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
            var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
            var transaccion = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction").FirstOrDefault();

            CrearDirectorios();
            DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);

            DocumentoPdf.EstablecerFont("Verdana", 18, true);
            DocumentoPdf.EscribirTexto(Titulo, 20, 70);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 20, 90);

            DocumentoPdf.EstablecerFont("Verdana", 13, false);
            DocumentoPdf.EscribirTexto("AISV:", 20, 110);
            DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr"), 140, 110);

            DocumentoPdf.EscribirTexto("DAE:", 20, 130);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault().DOCUMENT_ID ?? "", 140, 130);

            DocumentoPdf.EscribirTexto("NUMERO CAJAS:", 20, 150);
            DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "cargo", "item-quantity"), 140, 150);

            var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content").FirstOrDefault();
            var xmlInterno = contenidoInterno.ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", "");

            DocumentoPdf.EscribirTexto("LINEA:", 20, 170);
            DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), 140, 170);

            DocumentoPdf.EscribirTexto("NAVE:", 20, 190);
            DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvCvdCarrierVehicleName").Replace("</>", "").Replace("<>", ""), 140, 190);

            DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 210);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 140, 210);

            DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 230);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 140, 230);

            DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 250);
            DocumentoPdf.EscribirTexto($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", 140, 250);

            if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
            {
                DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 270);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), 150, 270);
                DocumentoPdf.EscribirTexto("SALIDA CAMION:", 20, 290);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 290);
                if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5))
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\EXPO\BRBK_CFS\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-S.pdf");
                else
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\BANANO\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-S.pdf");
            }
            else
            {
                DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 270);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 270);
                if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5))
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\EXPO\BRBK_CFS\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-I.pdf");
                else
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\BANANO\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-I.pdf");
            }
        }
    }

    internal class ImprimirDeliveryEmptyEntrada : ImprimirTicket
    {
        internal string Titulo;

        internal ImprimirDeliveryEmptyEntrada(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            try
            {
                var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 170);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 170);
                ev.HasMorePages = false;
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirDeliveryEmptyEntrada - EventoImprimir - Error:{0} - Source:{1}", ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }

        protected override void Pdf()
        {
        }
    }

    internal class ImprimirDeliveryEmptyEntradaSAV : ImprimirTicket
    {
        internal string Titulo;

        internal ImprimirDeliveryEmptyEntradaSAV(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            try
            {
                var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;

                if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                {
                    ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                    ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                    var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                    ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                    ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 170);
                    try { ev.Graphics.DrawString(datosExtras.Item1.TRUCK_LICENCE, Normal8, Brushes.Black, 120, 170); } catch { }
                    ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 190);
                    try { ev.Graphics.DrawString(datosExtras.Item1.DRIVER_ID, Normal8, Brushes.Black, 125, 190); } catch { }
                    ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 210);
                    try { ev.Graphics.DrawString(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 210); } catch { }

                    ev.Graphics.DrawString("PRE-GATES:", Negrita8, Brushes.Black, 20, 250);
                    try { ev.Graphics.DrawString(datosExtras.Item1.PRE_GATE_ID.ToString(), Normal8, Brushes.Black, 125, 250); } catch { }

                    ev.HasMorePages = false;
                }
                else
                {
                    var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction");
                    ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                    ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                    var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                    ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                    ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 170);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 170);
                    ev.Graphics.DrawString("NUM. BOOKING:", Negrita8, Brushes.Black, 20, 210);

                    foreach (var transaccion in transacciones)
                    {
                        var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content");
                        string xmlInterno = "";
                        try { xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); } catch { }
                        try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranEqoNbr").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 125, 210); } catch { }
                    }
                    ev.HasMorePages = false;
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirDeliveryEmptyEntradaSAV - EventoImprimir - Error:{0} - Source:{1}", ex.Message, ex.Source);
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
        }

        protected override void Pdf()
        {
            var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
            CrearDirectorios();
            if (datosExtras.Item1.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
            {
                DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);

                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);

                DocumentoPdf.EstablecerFont("Verdana", 18, true);
                DocumentoPdf.EscribirTexto(Titulo, 20, 70);

                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 20, 90);

                DocumentoPdf.EstablecerFont("Verdana", 13, false);

                DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 110);
                try { DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 180, 110); } catch { }
                DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 130);
                try { DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 180, 130); } catch { }

                DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 150);
                try { DocumentoPdf.EscribirTexto(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), 180, 150); } catch { }
                //}

                DocumentoPdf.EscribirTexto("PRE-GATES:", 20, 170);
                try { DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 180, 170); } catch { }
            }
            else
            {
                var transacciones = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction");
                DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);

                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);

                DocumentoPdf.EstablecerFont("Verdana", 18, true);
                DocumentoPdf.EscribirTexto(Titulo, 20, 70);

                DocumentoPdf.EstablecerFont("Verdana", 18, false);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 20, 90);

                DocumentoPdf.EstablecerFont("Verdana", 13, false);

                DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 110);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 140, 110);
                DocumentoPdf.EscribirTexto("NUM. BOOKING:", 180, 130);

                foreach (var transaccion in transacciones)
                {
                    var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content");
                    string xmlInterno = "";
                    try { xmlInterno = contenidoInterno.FirstOrDefault().ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); } catch { }
                    try { DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranEqoNbr").Replace("</>", "").Replace("<>", ""), 180, 110); } catch { }
                }

            }

            DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\VACIO\{datosExtras.Item1.PRE_GATE_ID.ToString() + "-" + DateTime.Now.ToString("HHmm")}-I.pdf");
        }
    }

    internal class ImprimirDeliveryImportBrBkCfs : ImprimirTicket // 2
    {
        internal string Titulo;

        internal ImprimirDeliveryImportBrBkCfs(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            try
            {
                int v_lineaActual = 0;
                var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
                var transaccion = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction").FirstOrDefault();

                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                if (datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE_ID == 18)
                {
                    ev.Graphics.DrawString("ORDEN:", Negrita8, Brushes.Black, 20, 170);
                    try { ev.Graphics.DrawString(datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault().REFERENCE_ID?.ToString(), Negrita8, Brushes.Black, 120, 170); } catch { }

                    ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 190);
                    try { ev.Graphics.DrawString(datosExtras.Item1.TRUCK_LICENCE, Normal8, Brushes.Black, 120, 190); } catch { }
                    ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 210);
                    try { ev.Graphics.DrawString(datosExtras.Item1.DRIVER_ID, Normal8, Brushes.Black, 125, 210); } catch { }
                    ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 230);
                    try { ev.Graphics.DrawString($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", Normal6, Brushes.Black, 125, 230); } catch { }
                    if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                    {
                        ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 250);
                        try { ev.Graphics.DrawString(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 250); } catch { }
                        ev.Graphics.DrawString("SALIDA CAMION:", Negrita8, Brushes.Black, 20, 270);
                        ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 270);
                    }
                    else
                    {
                        ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 250);
                        ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 250);
                    }
                    v_lineaActual = 260;
                }
                else
                {
                    ev.Graphics.DrawString("BL:", Negrita8, Brushes.Black, 20, 170);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr"), Negrita8, Brushes.Black, 120, 170); } catch { }

                    ev.Graphics.DrawString("NUM. BULTOS:", Negrita8, Brushes.Black, 20, 190);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "cargo", "item-quantity"), Normal8, Brushes.Black, 120, 190); } catch { }

                    var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content").FirstOrDefault();
                    string xmlInterno = "";
                    try { xmlInterno = contenidoInterno.ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", ""); } catch { }

                    ev.Graphics.DrawString("LINEA:", Negrita8, Brushes.Black, 20, 210);
                    if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                        try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 120, 210); } catch { }
                    ev.Graphics.DrawString("NAVE:", Negrita8, Brushes.Black, 20, 230);
                    try { ev.Graphics.DrawString(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvCvdCarrierVehicleName").Replace("</>", "").Replace("<>", ""), Normal8, Brushes.Black, 120, 230); } catch { }

                    ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 250);
                    try { ev.Graphics.DrawString(datosExtras.Item1.TRUCK_LICENCE, Normal8, Brushes.Black, 120, 250); } catch { }
                    ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 270);
                    try { ev.Graphics.DrawString(datosExtras.Item1.DRIVER_ID, Normal8, Brushes.Black, 125, 270); } catch { }
                    ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 290);
                    try { ev.Graphics.DrawString($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", Normal6, Brushes.Black, 125, 290); } catch { }
                    if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                    {
                        ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 310);
                        try { ev.Graphics.DrawString(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 310); } catch { }
                        ev.Graphics.DrawString("SALIDA CAMION:", Negrita8, Brushes.Black, 20, 330);
                        ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 330);
                    }
                    else
                    {
                        ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 310);
                        ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 310);
                    }
                    v_lineaActual = 340;
                }
                ev.Graphics.DrawString("Servicio combustible Gasolinera junto", Negrita8, Brushes.Black, 20, v_lineaActual + 10);
                ev.Graphics.DrawString("a SENAE Av. 25 de Julio. 24 Horas!", Negrita8, Brushes.Black, 20, v_lineaActual + 20);
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirDeliveryImportBrBkCfs - EventoImprimir - Error:{0} - Source:{1}", ex.Message, ex.Source);
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
            var datosExtras = (Tuple<PRE_GATE, string>)DatosExtras;
            var transaccion = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction").FirstOrDefault();

            CrearDirectorios();
            DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);

            DocumentoPdf.EstablecerFont("Verdana", 18, true);
            DocumentoPdf.EscribirTexto(Titulo, 20, 70);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 20, 90);

            DocumentoPdf.EstablecerFont("Verdana", 13, false);


            if (datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault().TRANSACTION_TYPE_ID == 18)
            {
                DocumentoPdf.EscribirTexto("ORDEN:", 20, 110);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_DETAILS.FirstOrDefault().REFERENCE_ID?.ToString(), 140, 110);

                DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 130);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 140, 130);

                DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 150);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 140, 150);

                DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 170);
                DocumentoPdf.EscribirTexto($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", 140, 170);

                if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                {
                    DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 190);
                    DocumentoPdf.EscribirTexto(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), 150, 190);
                    DocumentoPdf.EscribirTexto("SALIDA CAMION:", 20, 210);
                    DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 210);
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\BRBK_CFS\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-S.pdf");
                }
                else
                {
                    DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 190);
                    DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 190);
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\BRBK_CFS\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-I.pdf");
                }
            }
            else
            {
                DocumentoPdf.EscribirTexto("BL:", 20, 110);
                try { DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr"), 140, 110); } catch { }

                DocumentoPdf.EscribirTexto("NUM. BULTOS:", 20, 130);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "cargo", "item-quantity"), 140, 130);

                var contenidoInterno = BaseXml.ObtenerEtiquetas(transaccion.ToString(), "content").FirstOrDefault();
                var xmlInterno = contenidoInterno.ToString().Replace("<content><![CDATA[", "").Replace("]]></content>", "");

                DocumentoPdf.EscribirTexto("LINEA:", 20, 150);
                if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                    DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "tranLineId").Replace("</>", "").Replace("<>", ""), 140, 150);

                DocumentoPdf.EscribirTexto("NAVE:", 20, 170);
                DocumentoPdf.EscribirTexto(BaseXml.ObtenerValorEtiqueta(xmlInterno, "cvCvdCarrierVehicleName").Replace("</>", "").Replace("<>", ""), 140, 170);

                DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 190);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 140, 190);

                DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 210);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 140, 210);

                DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 230);
                DocumentoPdf.EscribirTexto($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", 140, 230);

                if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                {
                    DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 250);
                    DocumentoPdf.EscribirTexto(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), 150, 250);
                    DocumentoPdf.EscribirTexto("SALIDA CAMION:", 20, 270);
                    DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 270);
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\BRBK_CFS\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-S.pdf");
                }
                else
                {
                    DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 250);
                    DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 250);
                    DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\IMPO\BRBK_CFS\{BaseXml.ObtenerDatoDeXml(transaccion.ToString(), "bill-of-lading", "nbr")}-I.pdf");
                }
            }
        }
    }

    internal class ImprimirProveedores : ImprimirTicket
    {
        internal string Titulo;

        internal ImprimirProveedores(string codigoBarra, string xml, object datosExtras) : base(codigoBarra, xml, datosExtras)
        {
        }

        protected override void EventoImprimir(object sender, PrintPageEventArgs ev)
        {
            try
            {
                var datosExtras = (Tuple<PRE_GATE, string, string>)DatosExtras;
                ev.Graphics.DrawString("CONTECON GUAYAQUIL S.A.", Negrita12, Brushes.Black, 20, 20);
                ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 90, 50);
                ev.Graphics.DrawString(Titulo, Negrita8, Brushes.Black, 20, 70);
                var i = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "ImagenCodigoBarra");
                ev.Graphics.DrawImage(i, 2, 90, 2 * i.Width, i.Height);
                ev.Graphics.DrawString("PLACA CAMION:", Negrita8, Brushes.Black, 20, 170);
                try { ev.Graphics.DrawString(datosExtras.Item1.TRUCK_LICENCE, Normal8, Brushes.Black, 120, 170); } catch { }
                ev.Graphics.DrawString("CEDULA CHOFER:", Negrita8, Brushes.Black, 20, 190);
                try { ev.Graphics.DrawString(datosExtras.Item1.DRIVER_ID, Normal8, Brushes.Black, 125, 190); } catch { }
                ev.Graphics.DrawString("NOMBRE CHOFER:", Negrita8, Brushes.Black, 20, 210);
                try { ev.Graphics.DrawString($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", Normal6, Brushes.Black, 125, 210); } catch { }
                if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
                {
                    ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 230);
                    try { ev.Graphics.DrawString(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 230); } catch { }
                    ev.Graphics.DrawString("SALIDA CAMION:", Negrita8, Brushes.Black, 20, 250);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 250);
                    ev.Graphics.DrawString("PESO DE SALIDA:", Negrita8, Brushes.Black, 20, 270);
                    try { ev.Graphics.DrawString(datosExtras.Item3, Normal8, Brushes.Black, 125, 270); } catch { }
                }
                else
                {
                    ev.Graphics.DrawString("INGRESO CAMION:", Negrita8, Brushes.Black, 20, 230);
                    ev.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Normal8, Brushes.Black, 125, 230);
                    ev.Graphics.DrawString("PESO DE INGRESO:", Negrita8, Brushes.Black, 20, 250);
                    try { ev.Graphics.DrawString(datosExtras.Item3, Normal8, Brushes.Black, 125, 250); } catch { }
                }
                ev.HasMorePages = false;
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirProveedores - EventoImprimir - Error:{0} - Source:{1}", ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }

        }

        protected override void Pdf()
        {
            var datosExtras = (Tuple<PRE_GATE, string, string>)DatosExtras;
            var transaccion = BaseXml.ObtenerEtiquetas(Xml, "truck-transaction").FirstOrDefault();

            CrearDirectorios();
            DocumentoPdf.EscribirTexto("CONTECON GUAYAQUIL S.A.", 20, 20);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 90, 50);

            DocumentoPdf.EstablecerFont("Verdana", 18, true);
            DocumentoPdf.EscribirTexto(Titulo, 20, 70);

            DocumentoPdf.EstablecerFont("Verdana", 18, false);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.PRE_GATE_ID.ToString(), 20, 90);

            DocumentoPdf.EscribirTexto("PLACA CAMION:", 20, 110);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.TRUCK_LICENCE, 140, 110);

            DocumentoPdf.EscribirTexto("CEDULA CHOFER:", 20, 130);
            DocumentoPdf.EscribirTexto(datosExtras.Item1.DRIVER_ID, 140, 130);

            DocumentoPdf.EscribirTexto("NOMBRE CHOFER:", 20, 150);
            DocumentoPdf.EscribirTexto($"{datosExtras.Item2.Split(':')[1]} {datosExtras.Item2.Split(':')[2]}", 140, 150);

            if (datosExtras.Item1.STATUS == "I" || datosExtras.Item1.STATUS == "L")
            {
                DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 170);
                DocumentoPdf.EscribirTexto(datosExtras.Item1.KIOSK_TRANSACTIONS.FirstOrDefault(kt => kt.IS_OK).END_DATE.ToString("dd/MM/yyyy HH:mm"), 150, 170);
                DocumentoPdf.EscribirTexto("SALIDA CAMION:", 20, 190);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 190);
                DocumentoPdf.EscribirTexto("PESO DE SALIDA:", 20, 210);
                DocumentoPdf.EscribirTexto(datosExtras.Item3, 150, 210);
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\PROVEEDORES\{datosExtras.Item1.TRUCK_LICENCE}-S.pdf");
            }
            else
            {
                DocumentoPdf.EscribirTexto("INGRESO CAMION:", 20, 170);
                DocumentoPdf.EscribirTexto(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), 150, 170);
                DocumentoPdf.EscribirTexto("PESO DE INGRESO:", 20, 190);
                DocumentoPdf.EscribirTexto(datosExtras.Item3, 150, 190);
                DocumentoPdf.GuardarArchivo($@"{ConfigurationManager.AppSettings["RutaArchivosPdf"]}\{DateTime.Now.ToString("dd-MM-yyyy")}\PROVEEDORES\{datosExtras.Item1.TRUCK_LICENCE}-I.pdf");
            }
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

    internal class ImprimirEntradaDeliveryImportFull : ImprimirTicketEntrada
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10))
                {
                    v_linea = "1";
                    var titulo = viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) ? "ENTREGA DE CONTENEDOR IMPORTACION" : viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) ? "RECIBO DE CONTENEDOR EXPORTACION" : viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6) ? "DESESTIMIENTO - FCL" : "RECIBO DE CONTENEDOR VACIO";
                    titulo = viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19) ? "RECIBO DE CONTENEDOR EXPORTACION" : titulo;
                    v_linea = "2";
                    using (var impresion = new ImprimirDeliveryImportReceiveExportFullDrayOffReceiveEmpty(viewModel.DatosPreGate.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGate.PreGate, viewModel.DatoHuella)) { Titulo = titulo })
                    {
                        v_linea = "3";
                        impresion.Imprimir();
                        v_linea = "4";
                    }
                }
                else
                {
                    EstablecerSiguiente(new ImprimirEntradaDeliveryImportBrBkCfs());
                    v_linea = "5";
                    Siguiente.Procesar(viewModel);
                    v_linea = "6";
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirEntradaDeliveryImportFull - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal class ImprimirEntradaDeliveryImportBrBkCfs : ImprimirTicketEntrada
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 2) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 18))
                {
                    v_linea = "1";
                    using (var impresion = new ImprimirDeliveryImportBrBkCfs(viewModel.DatosPreGate.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGate.PreGate, viewModel.DatoHuella)) { Titulo = "ENTREGA DE CARGA SUELTA" })
                    {
                        v_linea = "2";
                        impresion.Imprimir();
                        v_linea = "3";
                    }
                }
                else
                {
                    v_linea = "4";
                    EstablecerSiguiente(new ImprimirEntradaReceiveExportBrBkCfsBanano());
                    v_linea = "5";
                    Siguiente.Procesar(viewModel);
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirEntradaDeliveryImportBrBkCfs - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal class ImprimirEntradaReceiveExportBrBkCfsBanano : ImprimirTicketEntrada
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 8) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 9))
                {
                    v_linea = "1";
                    var titulo = viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5) ? "RECEPCION DE CARGA SUELTA" : "RECEPCION DE CARGA BANANO";
                    v_linea = "2";
                    using (var impresion = new ImprimirReceiveExportBrBkCfsBanano(viewModel.DatosPreGate.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGate.PreGate, viewModel.DatoHuella)) { Titulo = titulo })
                    {
                        v_linea = "3";
                        impresion.Imprimir();
                    }
                }
                else
                {
                    v_linea = "4";
                    EstablecerSiguiente(new ImprimirEntradaEntregaVacio());
                    v_linea = "5";
                    Siguiente.Procesar(viewModel);
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirEntradaReceiveExportBrBkCfsBanano - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal class ImprimirEntradaEntregaVacio : ImprimirTicketEntrada
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                {
                    v_linea = "1";
                    if (viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16))
                    {
                        v_linea = "2";
                        var titulo = "ENTREGA DE VACIO SAV";
                        using (var impresion = new ImprimirDeliveryEmptyEntradaSAV(viewModel.DatosPreGate.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGate.PreGate, viewModel.DatoHuella)) { Titulo = titulo })
                        {
                            v_linea = "3";
                            impresion.Imprimir();
                        }
                    }
                    else if (viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17))
                    {
                        v_linea = "2";
                        var titulo = "ENTREGA DE CONTENEDOR CISE";
                        using (var impresion = new ImprimirDeliveryEmptyEntradaSAV(viewModel.DatosPreGate.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGate.PreGate, viewModel.DatoHuella)) { Titulo = titulo })
                        {
                            v_linea = "3";
                            impresion.Imprimir();
                        }
                    }
                    else
                    {
                        v_linea = "4";
                        var titulo = "ENTREGA DE VACIO";
                        using (var impresion = new ImprimirDeliveryEmptyEntrada(viewModel.DatosPreGate.PreGate.PRE_GATE_ID.ToString(), "", new Tuple<PRE_GATE, string>(viewModel.DatosPreGate.PreGate, viewModel.DatoHuella)) { Titulo = titulo })
                        {
                            v_linea = "5";
                            impresion.Imprimir();
                        }
                    }
                }
                else
                {
                    v_linea = "6";
                    EstablecerSiguiente(new ImprimirEntradaProveedores());
                    v_linea = "7";
                    Siguiente.Procesar(viewModel);
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirEntradaEntregaVacio - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal class ImprimirEntradaProveedores : ImprimirTicketEntrada
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 12) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 13) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 14) || viewModel.DatosPreGate.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 15))
                {
                    v_linea = "1";
                    var titulo = "INGRESO DE PROVEEDORES";
                    using (var impresion = new ImprimirProveedores(viewModel.DatosPreGate.PreGate.PRE_GATE_ID.ToString(), "", new Tuple<PRE_GATE, string, string>(viewModel.DatosPreGate.PreGate, viewModel.DatoHuella, viewModel.DatosPreGate.Peso.ToString())) { Titulo = titulo })
                    {
                        v_linea = "2";
                        impresion.Imprimir();
                    }
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirEntradaProveedores - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal abstract class ImprimirTicketSalida
    {
        protected ImprimirTicketSalida Siguiente { get; set; }

        protected void EstablecerSiguiente(ImprimirTicketSalida siguiente)
        {
            Siguiente = siguiente;
        }

        internal abstract void Procesar(VentanaPrincipalViewModel viewModel);
    }

    internal class ImprimirSalidaDeliveryImportFull : ImprimirTicketSalida
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 7) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 10) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19))
                {
                    v_linea = "1";
                    var titulo = viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) ? "ENTREGA DE CONTENEDOR IMPORTACION" : viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 3) ? "RECIBO DE CONTENEDOR EXPORTACION" : viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 6) ? "DESESTIMIENTO - FCL" : viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11) ? "ENTREGA DE CONTENEDOR VACIO" : viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 16) ? "ENTREGA DE CONTENEDOR VACIO SAV" : "RECIBO DE CONTENEDOR VACIO";
                    titulo = viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 17) ? "ENTREGA DE CONTENEDOR CISE" : titulo;
                    titulo = viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 19) ? "RECIBO DE CONTENEDOR EXPORTACION" : titulo;

                    string v_deposito = string.Empty;
                    string[] v_cadena;
                    try
                    {
                        v_linea = "2";
                        v_cadena = viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.Where(p => p.REFERENCE_ID == "I").FirstOrDefault().TRANSACTION_NUMBER.ToString().Split('-');

                        v_linea = "3";
                        v_deposito = viewModel.ObtenerDeposito(int.Parse(v_cadena[5]));
                    }
                    catch
                    {
                        v_deposito = string.Empty;
                    }

                    v_linea = "4";
                    using (var impresion = new ImprimirSalidaDeliveryImportReceiveExportFullReceiveEmpty(viewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGateSalida.PreGate, viewModel.DatoHuella)) { Titulo = titulo, Deposito = v_deposito })
                    {
                        v_linea = "5";
                        impresion.Imprimir();
                        v_linea = "6";
                        if (viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 1) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 11))
                        {
                            v_linea = "7";
                            var transacciones = BaseXml.ObtenerEtiquetas(viewModel.DatosN4.Xml, "truck-transaction");
                            foreach (var transaccion in transacciones)
                            {
                                v_linea = "8";
                                using (var impresionGuia = new ImprimirSalidaDeliveryImportGuiaDesinfeccion(viewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID.ToString(), transaccion.ToString(), new Tuple<PRE_GATE, string>(viewModel.DatosPreGateSalida.PreGate, viewModel.DatoHuella)))
                                {
                                    v_linea = "9";
                                    impresionGuia.Imprimir();
                                }
                            }
                        }
                    }
                }
                else
                {
                    v_linea = "10";
                    EstablecerSiguiente(new ImprimirSalidaDeliveryImportBrBkCfs());
                    v_linea = "11";
                    Siguiente.Procesar(viewModel);
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirSalidaDeliveryImportFull - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal class ImprimirSalidaDeliveryImportBrBkCfs : ImprimirTicketSalida
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 2) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 18))
                {
                    v_linea = "1";
                    using (var impresion = new ImprimirDeliveryImportBrBkCfs(viewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGateSalida.PreGate, viewModel.DatoHuella)) { Titulo = "ENTREGA DE CARGA SUELTA" })
                    {
                        v_linea = "2";
                        impresion.Imprimir();
                    }
                }
                else
                {
                    v_linea = "3";
                    EstablecerSiguiente(new ImprimirSalidaReceiveExportBrBkCfsBanano());
                    v_linea = "4";
                    Siguiente.Procesar(viewModel);
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirSalidaDeliveryImportBrBkCfs - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal class ImprimirSalidaReceiveExportBrBkCfsBanano : ImprimirTicketSalida
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 8) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 9))
                {
                    v_linea = "1";
                    var titulo = viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 4) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 5) ? "RECEPCION DE CARGA SUELTA" : "RECEPCION DE CARGA SUELTA BANANO";
                    using (var impresion = new ImprimirReceiveExportBrBkCfsBanano(viewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID.ToString(), viewModel.DatosN4.Xml, new Tuple<PRE_GATE, string>(viewModel.DatosPreGateSalida.PreGate, viewModel.DatoHuella)) { Titulo = titulo })
                    {
                        v_linea = "2";
                        impresion.Imprimir();
                    }
                }
                else
                {
                    v_linea = "3";
                    EstablecerSiguiente(new ImprimirSalidaProveedores());
                    v_linea = "4";
                    Siguiente.Procesar(viewModel);
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirSalidaReceiveExportBrBkCfsBanano - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }

    internal class ImprimirSalidaProveedores : ImprimirTicketSalida
    {
        internal override void Procesar(VentanaPrincipalViewModel viewModel)
        {
            string v_linea = "0";
            try
            {
                if (viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 12) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 13) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 14) || viewModel.DatosPreGateSalida.PreGate.PRE_GATE_DETAILS.All(pgd => pgd.TRANSACTION_TYPE_ID == 15))
                {
                    v_linea = "1";
                    var titulo = "SALIDA DE PROVEEDORES";
                    using (var impresion = new ImprimirProveedores(viewModel.DatosPreGateSalida.PreGate.PRE_GATE_ID.ToString(), "", new Tuple<PRE_GATE, string, string>(viewModel.DatosPreGateSalida.PreGate, viewModel.DatoHuella, viewModel.DatosPreGateSalida.Peso.ToString())) { Titulo = titulo })
                    {
                        v_linea = "2";
                        impresion.Imprimir();
                    }
                }
            }
            catch (Exception ex)
            {
                string v_error = string.Format("Transaction - PaginaImpresionViewModel - ImprimirSalidaProveedores - Linea:{0} - Error:{1} - Source:{2}", v_linea, ex.Message, ex.Source);
                PaginaImpresionViewModel.LogEventos(v_error);
                throw new ArgumentException(v_error);
            }
        }
    }



}