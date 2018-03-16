using OpenCvSharp;
using OpenCvSharp.Extensions;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using Reactive.Bindings.Extensions;
using ZXing;

namespace QRCodeTester.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            _barcodeReader = new ZXing.Presentation.BarcodeReader
            {
                AutoRotate = true,
                Options =
                {
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat>{ BarcodeFormat.QR_CODE }
                },
            };
            StartCommand = Capturing.Select(x => !x).ToReactiveCommand();
            StartCommand.Subscribe(_ => Start());
            StopCommand = Capturing.Select(x => x).ToReactiveCommand();
            StopCommand.Subscribe(_ =>
            {
                Stop();
                CaptureImageSource.Value = null;
            });
        }

        private readonly ZXing.Presentation.BarcodeReader _barcodeReader;

        public ReactiveProperty<bool> Capturing { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<string> ReadResult { get; } = new ReactiveProperty<string>(string.Empty);
        public ReactiveProperty<BitmapSource> CaptureImageSource { get; } = new ReactiveProperty<BitmapSource>();
        public ReactiveCommand StartCommand { get; }
        public ReactiveCommand StopCommand { get; }

        private void Start()
        {
            var capture = new VideoCapture(0);
            if (!capture.IsOpened()) return;
            Capturing.Value = true;
            ReadResult.Value = string.Empty;

            Observable.Interval(TimeSpan.FromMilliseconds(1))
                .Finally(() =>
                {
                    capture.Dispose();
                    Capturing.Value = false;
                })
                .TakeWhile(x => Capturing.Value)
                .Select(_ =>
                {
                    var frame = new Mat();
                    return capture.Read(frame) ? frame : null;
                })
                .Where(frame => frame != null)
                .ObserveOnUIDispatcher()     
                .Subscribe(frame =>
                {
                    using (frame)
                    {
                        var bitmapSource = frame.ToBitmapSource();
                        CaptureImageSource.Value = bitmapSource;
                        var result = _barcodeReader.Decode(bitmapSource);
                    
                        if (result == null) return;
                        Capturing.Value = false;
                        ReadResult.Value = result.Text;
                    }
                });
        }

        private void Stop() => Capturing.Value = false;
    }
}