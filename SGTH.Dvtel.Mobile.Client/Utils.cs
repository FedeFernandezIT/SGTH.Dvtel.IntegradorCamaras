using System;
using System.IO;
using System.Reflection;

namespace SGTH.Dvtel.Mobile.Client
{
    public class Utils
    {
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static Consts.Speeds IntToSpeeds(int notch)
        {
            switch (notch)
            {
                case 0: return Consts.Speeds.Pause;
                case 1: return Consts.Speeds.SlowForward25;
                case 2: return Consts.Speeds.SlowForward50;
                case 3: return Consts.Speeds.Play;
                case 4: return Consts.Speeds.FastForward200;
                case 5: return Consts.Speeds.FastForward400;
                case 6: return Consts.Speeds.FastForward800;
                case 7: return Consts.Speeds.FastForward1600;
                case 8: return Consts.Speeds.FastForward3200;
                case 9: return Consts.Speeds.FastForward6400;
                case -1: return Consts.Speeds.SlowRewind25;
                case -2: return Consts.Speeds.SlowRewind50;
                case -3: return Consts.Speeds.ReversePlay;
                case -4: return Consts.Speeds.FastRewind200;
                case -5: return Consts.Speeds.FastRewind400;
                case -6: return Consts.Speeds.FastRewind800;
                case -7: return Consts.Speeds.FastRewind1600;
                case -8: return Consts.Speeds.FastRewind3200;
                case -9: return Consts.Speeds.FastRewind6400;
            }
            return Consts.Speeds.Pause;
        }

        public static void Trace(string message, Exception ex = null)
        {
            System.Diagnostics.Trace.WriteLine(ex == null ?
                string.Format("FLIR.SDK.Samples.RESTClient: {0}", message) :
                string.Format("FLIR.SDK.Samples.RESTClient: {0}\n{1}\n{2}", message, ex.Message, ex.StackTrace));
        }
    }
}
