using Microsoft.Win32;

namespace StellaModLauncherNET.Scripts;

internal static class Secret
{
	public const string RegistryKeyPath = @"Software\Stella Mod Launcher";
	public static bool IsStellaPlusSubscriber = false;
	public static string BearerToken;

	public static string GetTokenFromRegistry()
	{
		using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
		{
			object value = registryKey?.GetValue("Secret");
			if (!(value is string token)) return null;

			Program.Logger.Info("Found a `Secret` token used by subscribers in the registry");
			return token;
		}
	}

	public static async Task<string> VerifyToken(string registrySecret)
	{
		try
		{
			List<KeyValuePair<string, string>> postData = new()
			{
				new("token", registrySecret),
				new("motherboardId", ComputerInfo.GetMotherboardSerialNumber()),
				new("cpuId", ComputerInfo.GetCpuSerialNumber()),
				new("diskId", ComputerInfo.GetHardDriveSerialNumber())
			};

			using (HttpClient httpClient = new())
			{
				httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(Program.UserAgent);

				FormUrlEncodedContent content = new(postData);
				HttpResponseMessage response = await httpClient.PostAsync($"{Program.WebApi}/genshin-stella-mod/access/launcher/verify", content);

				string json = await response.Content.ReadAsStringAsync();

				if (!response.IsSuccessStatusCode) Program.Logger.Error("Secret.VerifyToken(): " + json);

				return json;
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(
				$"A critical error occurred during the verification of your subscription. For some reason, your computer was unable to send a request to the API interface located in Warsaw, Poland. Please check your antivirus software or visit the status page at status.sefinek.net.\n\nThe application must be closed immediately. Below, you will find error details. If you are unsure about what to do in this situation, please contact the software developer. Subscribers are provided with continuous technical support. Good luck!\n\n{ex.InnerException ?? ex}",
				Program.AppNameVer, MessageBoxButtons.OK, MessageBoxIcon.Error);

			Log.ErrorAndExit(ex, false);
			return null;
		}
	}
}
