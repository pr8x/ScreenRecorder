using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Capture;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ScreenRecorder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

       
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task StartRecordingAsync()
        {
            GraphicsCapturePicker pick = new GraphicsCapturePicker() ;
            var recordedfield = await pick.PickSingleItemAsync();
            CoreWindow window = CoreApplication.MainView.CoreWindow;

            await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await new ScreenRecord().Record(recordedfield);
            });

        

    }
        async Task StopRecoringAsync()
        {

        }
       

        private void StartRecording(object sender, RoutedEventArgs e)
        {
            StartRecordingAsync();
        }

        private void StopRecoring(object sender, RoutedEventArgs e)
        {
            StopRecoringAsync();
        }
    }
}
