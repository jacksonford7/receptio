using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ControlesAccesoQR.ViewModels.ControlesAccesoQR;

namespace ControlesAccesoQR.Views.ControlesAccesoQR
{
    public partial class VistaSalidaFinal : Page
    {
        private readonly StringBuilder _qrBuffer = new StringBuilder();
        private readonly DispatcherTimer _qrTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        private string _lastScan = string.Empty;
        private DateTime _lastScanTime = DateTime.MinValue;
        private CancellationTokenSource _qrCts;
        private bool _autoSubmitting;

        public VistaSalidaFinal()
        {
            InitializeComponent();
            _qrTimer.Tick += QrTimer_Tick;
            Loaded += VistaSalidaFinal_Loaded;
            Unloaded += VistaSalidaFinal_Unloaded;

            Loaded += (_, __) => { QrInput?.Focus(); };
            DataContextChanged += (_, __) =>
            {
                if (DataContext is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged -= OnVmPropertyChanged;
                    npc.PropertyChanged += OnVmPropertyChanged;
                }
            };
        }

        private void OnVmPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VistaSalidaFinalViewModel.NumeroPaseSalida) || e.PropertyName == "NumeroPaseSalida")
            {
                DebouncedAutoProcesar(200);
            }
        }

        private void DebouncedAutoProcesar(int delayMs)
        {
            _qrCts?.Cancel();
            _qrCts = new CancellationTokenSource();
            var token = _qrCts.Token;

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delayMs, token);
                    if (token.IsCancellationRequested) return;

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (_autoSubmitting) return;
                        _autoSubmitting = true;
                        try
                        {
                            var vm = DataContext;
                            var cmd = (vm?.GetType().GetProperty("ProcesarSalidaCommand")?.GetValue(vm)) as ICommand;
                            var param = vm?.GetType().GetProperty("NumeroPaseSalida")?.GetValue(vm);
                            if (cmd != null && cmd.CanExecute(param))
                                cmd.Execute(param);
                        }
                        finally
                        {
                            _autoSubmitting = false;
                        }
                    });
                }
                catch (TaskCanceledException) { }
            }, token);
        }

        private void VistaSalidaFinal_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                window.PreviewKeyDown += Window_PreviewKeyDown;
        }

        private void VistaSalidaFinal_Unloaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                window.PreviewKeyDown -= Window_PreviewKeyDown;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.FocusedElement is TextBox)
                return;

            if (e.Key == Key.Enter)
            {
                _qrTimer.Stop();
                var text = _qrBuffer.ToString();
                _qrBuffer.Clear();
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var now = DateTime.UtcNow;
                if (text == _lastScan && (now - _lastScanTime).TotalMilliseconds < 200)
                    return;

                _lastScan = text;
                _lastScanTime = now;

                if (DataContext is VistaSalidaFinalViewModel vm)
                    vm.SubmitPass(text, "qr");

                e.Handled = true;
            }
            else
            {
                char c = KeyToChar(e.Key);
                if (c != '\0')
                {
                    _qrBuffer.Append(c);
                    _qrTimer.Stop();
                    _qrTimer.Start();
                }
            }
        }

        private void QrTimer_Tick(object sender, EventArgs e)
        {
            _qrTimer.Stop();
            _qrBuffer.Clear();
        }

        private static char KeyToChar(Key key)
        {
            if (key >= Key.D0 && key <= Key.D9)
                return (char)('0' + (key - Key.D0));
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
                return (char)('0' + (key - Key.NumPad0));
            if (key >= Key.A && key <= Key.Z)
                return (char)('A' + (key - Key.A));
            return '\0';
        }
    }
}

