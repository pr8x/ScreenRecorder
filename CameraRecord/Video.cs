using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace ScreenRecorder.CameraRecord
{
 
    class Video
    {
        LowLagMediaRecording recorder;
    MediaCapture md;
   

        async Task Start()
        {
            md = new MediaCapture();
            await md.InitializeAsync();

            var video = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Videos);
            StorageFile file = await video.SaveFolder.CreateFileAsync("video.mp4", CreationCollisionOption.GenerateUniqueName);
            recorder = await md.PrepareLowLagRecordToStorageFileAsync(
        MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), file);
            await recorder.StartAsync();
        }
        async Task Stop()
        {
            await recorder.StopAsync();
            await recorder.FinishAsync();
        }
    }
}
