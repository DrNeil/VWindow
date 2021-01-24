using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace VWindow
{
	internal class Settings
	{
		const string SelectedCameraID = "CameraID";

		public void SaveCameraSelection(string cameraId)
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			roamingSettings.Values[SelectedCameraID] = cameraId;
		}

		public string LoadLastSelectedCamera()
		{
			ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;
			string camera = roamingSettings.Values[SelectedCameraID]?.ToString();
			return camera;
		}
	}
}
