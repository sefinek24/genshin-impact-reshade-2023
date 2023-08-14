using System;
using System.IO;
using IWshRuntimeLibrary;
using PrepareStella.Properties;

namespace PrepareStella.Scripts.Preparing
{
    /// <summary>
    ///     Provides functionality to create a desktop icon for the Stella Launcher.
    /// </summary>
    internal static class DesktopIcon
    {
        public static void Run()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                string shortcutPath = Path.Combine(desktopPath, "Stella Mod Launcher.lnk");

                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.Description = Resources.Utils_RunOfficialLauncherForStellaModMadeBySefinek;
                shortcut.WorkingDirectory = Program.AppPath;
                shortcut.TargetPath = Path.Combine(Program.AppPath, "Stella Mod Launcher.exe");

                shortcut.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
