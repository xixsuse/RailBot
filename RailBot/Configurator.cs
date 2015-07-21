using System;
using System.IO;

namespace RailBot
{
    public static class Configurator
    {

        static class Constants
        {
            private static readonly string HomeDir = 
            (Environment.OSVersion.Platform == PlatformID.Unix
            || Environment.OSVersion.Platform == PlatformID.MacOSX)
            ? Environment.GetEnvironmentVariable("HOME")
            : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            private static readonly string ConfDir = ".railbot";

            public static readonly string ConfFileName = 
                Path.Combine(HomeDir, ConfDir, "update");

            public static readonly string TokenFileName = 
                Path.Combine(HomeDir, ConfDir, "token");
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
            if (File.Exists(Constants.ConfFileName))
            {
                var lines = File.ReadAllLines(Constants.ConfFileName);
                return lines[0].NullIfNullEmptyOrWhitespace();
            }
            return null;
        }

        public static string GetAuthTokenFromConfiguration()
        {
            if (File.Exists(Constants.TokenFileName))
            {
                var lines = File.ReadAllLines(Constants.TokenFileName);
                return lines[0].NullIfNullEmptyOrWhitespace();
            }
            return null;
        }

        private static string NullIfNullEmptyOrWhitespace(this string s)
        {
            return (!string.IsNullOrEmpty(s) && 
                !string.IsNullOrWhiteSpace(s) ? s : null);
        }

    }
}

