using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Capture;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ScreenRecorder
{
    internal class MainPageViewModel : ReactiveObject
    {
        private readonly DispatcherTimer _elapsedUpdateTimer;
        private readonly Recorder _recorder;
        private readonly Stopwatch _sw;
        private StorageFolder _recorderOutput;

        public MainPageViewModel()
        {
            _recorder = new Recorder();
            _sw = new Stopwatch();
            _elapsedUpdateTimer = new DispatcherTimer();
            _elapsedUpdateTimer.Interval = TimeSpan.FromMilliseconds(1);
            _elapsedUpdateTimer.Tick += ElapsedUpdateTimerOnTick;

            ToggleRecordingCommand = ReactiveCommand.CreateFromTask(ToggleRecording);
        }

        [Reactive] public bool IsRecording { get; private set; }

        [Reactive] public TimeSpan RecordingTime { get; private set; }

        public ICommand ToggleRecordingCommand { get; }

        private void ElapsedUpdateTimerOnTick(object sender, object e)
        {
            RecordingTime = _sw.Elapsed;
        }

        private async Task ToggleRecording()
        {
            if (IsRecording)
            {
                await StopRecording();
            }
            else
            {
                await StartRecording();
            }
        }

        private static async Task ClearFolder(IStorageFolder folder)
        {
            var files = await folder.GetFilesAsync();

            foreach (var storageFile in files)
            {
                await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        private async Task StartRecording()
        {
            Debug.Assert(!IsRecording);

            var pick = new GraphicsCapturePicker();
            var captureItem = await pick.PickSingleItemAsync();

            if (captureItem != null)
            {
                _recorderOutput = ApplicationData.Current.TemporaryFolder;

                await ClearFolder(_recorderOutput);

                _sw.Restart();
                _elapsedUpdateTimer.Start();

                _recorder.Start(captureItem, _recorderOutput);
            }

            IsRecording = true;
        }

        private async Task StopRecording()
        {
            _sw.Stop();
            _elapsedUpdateTimer.Stop();

            RecordingTime = TimeSpan.Zero;
            _recorder.Stop();

            IsRecording = false;

            var folderPicker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop
            };
            folderPicker.FileTypeFilter.Add("*");

            var outputFolder = await folderPicker.PickSingleFolderAsync();

            if (outputFolder == null)
            {
                return;
            }

            var existingFiles = await outputFolder.GetFilesAsync();

            if (existingFiles.Count != 0)
            {
                var messageDialog = new MessageDialog("The selected folder is not empty. The content will be cleared.");

                var okCommand = new UICommand("Ok");
                var cancelCommand = new UICommand("Cancel");

                messageDialog.Commands.Add(okCommand);

                messageDialog.Commands.Add(cancelCommand);

                var command = await messageDialog.ShowAsync();

                if (command == cancelCommand)
                {
                    return;
                }
            }

            await ClearFolder(outputFolder);

            var frames = await _recorderOutput.GetFilesAsync();

            foreach (var storageFile in frames)
            {
                await storageFile.CopyAsync(outputFolder);
            }

            await Launcher.LaunchFolderAsync(outputFolder);
        }
    }
}