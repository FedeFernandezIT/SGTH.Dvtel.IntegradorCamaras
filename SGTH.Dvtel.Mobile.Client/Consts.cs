namespace SGTH.Dvtel.Mobile.Client
{
    public class Consts
    {
        public enum CompressionType
        {
            Mjpeg, // Over http only
            H264  // Over rtsp only
        }

        public enum Schemes
        {
            Http,
            Rtsp
        }

        public enum Speeds
        {
            FastRewind6400 = -6400,
            FastRewind3200 = -3200,
            FastRewind1600 = -1600,
            FastRewind800 = -800,
            FastRewind400 = -400,
            FastRewind200 = -200,
            ReversePlay = -100,
            SlowRewind50 = -50,
            SlowRewind25 = -25,
            Pause = 0,
            SlowForward25 = 25,
            SlowForward50 = 50,
            Play = 100,
            FastForward200 = 200,
            FastForward400 = 400,
            FastForward800 = 800,
            FastForward1600 = 1600,
            FastForward3200 = 3200,
            FastForward6400 = 6400
        }

        public const int MaxFastForwardMjpegNotch = 9;
        public const int MaxFastForwardH264Notch = 6;
        public const int MaxFastReveseMjpegNotch = -9;
        public const int MaxFastReveseH264Notch = 0;
        public const int NormalPlaybackNotch = 3;
    }
}
