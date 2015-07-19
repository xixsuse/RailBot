using System;
using System.IO;

namespace RailBot
{
    public static class Configurator
    {

        static class Constants
        {
            public static readonly string ConfFileName = 
                Path.Combine((Environment.OSVersion.Platform == PlatformID.Unix 
                    || Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%"),
                    ".railBotConf");
        }

        public static void WriteConfiguration(int offset)
        {
            WriteConfiguration(offset.ToString());
        }

        private static void WriteConfiguration(string offset)
        {
            File.WriteAllText(Constants.ConfFileName, offset);
        }

        public static string GetOffsetFromConfiguration()
        {
            int offset = 0;
            if (int.TryParse(InternalGetOffsetFromConfiguration(), out offset))
                return (offset + 1).ToString();
            return null;
        }

        private static string InternalGetOffsetFromConfiguration()
        {
            var lines = File.ReadAllLines(Constants.ConfFileName);
            return (!string.IsNullOrEmpty(lines[0]) &&
                !string.IsNullOrWhiteSpace(lines[0]) ? lines[0] : null);
        }
    }
}

