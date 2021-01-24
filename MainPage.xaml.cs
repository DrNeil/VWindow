using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VWindow
{
	public sealed partial class MainPage : Page
	{
		MediaCapture mediaCapture;
		public MainPage()
		{
			this.InitializeComponent();
			Loaded += MainPage_Loaded;
		}

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			CameraList.ItemsSource = await GetAllDevices();
			Settings settings = new Settings();
			string cameraID = settings.LoadLastSelectedCamera();
			if (!string.IsNullOrEmpty(cameraID))
			{
				await SetCameraView(cameraID);
			}
		}

		MediaFrameSourceInfo colorSourceInfo = null;
		
		public async Task<List<string>> GetAllDevices()
		{
			List<String> deviceNames = new List<string>();
			var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
			foreach(var group in frameSourceGroups)
			{
				deviceNames.Add(group.DisplayName);
			}	

			return deviceNames;
		}

		private async void CameraList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string selected = e.AddedItems[0].ToString();
			await SetCameraView(selected);
			Settings settings = new Settings();
			settings.SaveCameraSelection(selected);
		}

		private async Task SetCameraView(string selected)
		{
			var device = await GetMediaDeviceGroup(selected);
			MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings
			{
				SourceGroup = device,
				SharingMode = MediaCaptureSharingMode.SharedReadOnly ,
				MemoryPreference = MediaCaptureMemoryPreference.Auto,
				StreamingCaptureMode = StreamingCaptureMode.Video
			};


			mediaCapture = new MediaCapture();
			await mediaCapture.InitializeAsync(settings);
			previewElement.Source = mediaCapture;
			await mediaCapture.StartPreviewAsync();
		}

		public async Task<MediaFrameSourceGroup> GetMediaDeviceGroup(string deviceName)
		{
			var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();

			MediaFrameSourceGroup selectedGroup = null;
			colorSourceInfo = null;

			foreach (var sourceGroup in frameSourceGroups)
			{
				if (deviceName == sourceGroup.DisplayName)
				{
					foreach (var sourceInfo in sourceGroup.SourceInfos)
					{
						if (sourceInfo.MediaStreamType == MediaStreamType.VideoPreview
							|| sourceInfo.MediaStreamType == MediaStreamType.VideoRecord
							)
						{
							colorSourceInfo = sourceInfo;
							break;
						}
					}
					if (colorSourceInfo != null)
					{
						selectedGroup = sourceGroup;
						break;
					}
				}
			}
			return selectedGroup;
		}
	}
}
