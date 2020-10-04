using System;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Microsoft.Graphics.Canvas;

namespace ScreenRecorder
{
    internal class Recorder
    {
        private readonly CanvasDevice _device;
        private GraphicsCaptureItem _captureItem;
        private Direct3D11CaptureFramePool _framePool;
        private StorageFolder _outputFolder;
        private GraphicsCaptureSession _session;

        public Recorder()
        {
            _device = new CanvasDevice();
        }

        public void Start(GraphicsCaptureItem captureItem, StorageFolder outputFolder)
        {
            _captureItem = captureItem;
            _captureItem.Closed += CaptureItemOnClosed;
            _framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(
                _device,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                1,
                _captureItem.Size);
            _framePool.FrameArrived += OnFrameArrived;
            _outputFolder = outputFolder;
            _session = _framePool.CreateCaptureSession(_captureItem);
            _session.StartCapture();
        }

        private void CaptureItemOnClosed(GraphicsCaptureItem sender, object args)
        {
            Stop();
        }

        public void Stop()
        {
            _captureItem = null;
            _framePool.Dispose();
            _framePool = null;
            _session.Dispose();
            _session = null;
        }

        private async void OnFrameArrived(Direct3D11CaptureFramePool sender, object args)
        {
            using (var frame = sender.TryGetNextFrame())
            {
                var sbmp = await CreateSoftwareBitmapFromSurface(frame.Surface);
                var frameFile =
                    await _outputFolder.CreateFileAsync("frame.jpg", CreationCollisionOption.GenerateUniqueName);
                SaveSoftwareBitmapToFile(sbmp, frameFile);
            }
        }

        private async void SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, IStorageFile outputFile)
        {
            using (var stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();
            }
        }

        private async Task<SoftwareBitmap> CreateSoftwareBitmapFromSurface(IDirect3DSurface surface)
        {
            return await SoftwareBitmap.CreateCopyFromSurfaceAsync(surface);
        }

    }
}