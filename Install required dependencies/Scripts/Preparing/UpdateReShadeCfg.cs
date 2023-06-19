using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Download the ReShade files - ReShade.ini and ReShade.log.
    /// </summary>
    internal static class UpdateReShadeCfg
    {
        public static async Task Run()
        {
            string reshadeIniPath = Path.Combine(Program.GameDirGlobal, "ReShade.ini");
            string reshadeLogPath = Path.Combine(Program.GameDirGlobal, "ReShade.log");

            if (!Directory.Exists(Program.GameDirGlobal))
            {
                Console.WriteLine(@"You must configure some settings manually.");
                return;
            }

            // ReShade.ini
            File.Delete(reshadeIniPath);

            WebClient wbClient1 = new WebClient();
            wbClient1.Headers.Add("user-agent", Program.UserAgent);
            await wbClient1.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.ini", reshadeIniPath);

            // ReShade.log
            File.Delete(reshadeLogPath);

            WebClient wbClient2 = new WebClient();
            wbClient2.Headers.Add("user-agent", Program.UserAgent);
            await wbClient2.DownloadFileTaskAsync("https://cdn.sefinek.net/resources/v3/genshin-stella-mod/reshade/ReShade.log", reshadeLogPath);

            // Final
            if (!File.Exists(reshadeIniPath) || !File.Exists(reshadeLogPath))
                Log.ErrorAndExit(new Exception($"Something went wrong. Config or log file for ReShade was not found in: {Program.GameDirGlobal}"), false, false);

            Log.Output($"{Path.GetFileName(reshadeIniPath)} and {Path.GetFileName(reshadeLogPath)} was successfully downloaded.");

            string cache = Path.Combine(Program.ResourcesGlobal, "Cache");
            if (!Directory.Exists(cache))
            {
                Console.WriteLine(@"Creating cache folder...");
                Directory.CreateDirectory(cache);
            }

            Console.WriteLine(@"Configuring ReShade...");
            ConfigureReShade(Program.ResourcesGlobal);
        }

        private static void ConfigureReShade(string resourcesGlobal)
        {
            IniFile ini = new IniFile(Path.Combine(Program.GameDirGlobal, "ReShade.ini"));
            ini.WriteString("ADDON", "AddonPath", $"{Path.Combine(resourcesGlobal, "ReShade", "Addons")}");
            ini.WriteString("GENERAL", "EffectSearchPaths", Path.Combine(resourcesGlobal, "ReShade", "Shaders", "Effects"));
            ini.WriteString("GENERAL", "IntermediateCachePath", Path.Combine(resourcesGlobal, "ReShade", "Cache"));
            ini.WriteString("GENERAL", "PresetPath", Path.Combine(resourcesGlobal, "ReShade", "Presets", "3. Preset by Sefinek - Medium settings [Default].ini"));
            ini.WriteString("GENERAL", "TextureSearchPaths", $"{Path.Combine(resourcesGlobal, "ReShade", "Shaders", "Textures")}");
            ini.WriteString("SCREENSHOT", "SavePath", Path.Combine(resourcesGlobal, "Screenshots"));
            ini.WriteString("SCREENSHOT", "SoundPath", Path.Combine(Program.AppPath, "data", "sounds", "screenshot.wav"));
            ini.Save();
        }
    }
}
