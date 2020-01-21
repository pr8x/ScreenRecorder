using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;

namespace ScreenRecorder
{
    class ScreenRecord
    {
        private GraphicsCaptureItem field;
        private Direct3D11CaptureFramePool framePool;
        private CanvasDevice canvasDevice;
        private GraphicsCaptureSession session;
        public async Task Record(GraphicsCaptureItem _field)
        {
            canvasDevice = new CanvasDevice();
            field = _field;
            framePool = Direct3D11CaptureFramePool.Create(
                canvasDevice, // D3D device
                DirectXPixelFormat.A8P8, // Pixel format
                2, // Number of frames
                field.Size); // Size of the buffers
            session = framePool.CreateCaptureSession(field);


        }
        public void StopRecord()
        {
            field = null;
            framePool.Dispose();
            canvasDevice.Dispose();
            session.Dispose();
        }
    }
}
