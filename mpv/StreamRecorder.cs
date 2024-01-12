using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace radioZiner
{
    public class StreamRecorder : EventArgs
    {
        public int streamerProgram = 0; // 0=ffmpeg 1=streamLink
        public string transCode = " -c copy";
        public string map = " ";
        public string progressFile = ""; // @"-progress rec_progress.txt";

        public string ffMpegExeFile = Application.StartupPath + @"\ffmpeg.exe";

        public Process streamProcess = null;

        public string inputURL = "";
        public string outputURL = "";

        public StreamChecker streamChecker = null;
        public M3u.TvgChannel channel;

        public event EventHandler RecordingStarted;
        public event EventHandler RecordingStopped;

        public StreamRecorder(string output)
        {
            outputURL = output;
            streamChecker = new StreamChecker(outputURL);
        }

        public int State()
        {
            int state = streamChecker?.StreamState ?? -2;
            return state;
        }

        public void Start(M3u.TvgChannel channel, int source = 0)
        {
            this.channel = channel;
            Start(channel.url, source);
        }

        public void Start(string input, int source = 0, string output = "")
        {
            if (output == "") output = outputURL;
            //if (output == outputURL) try { ffMpeg.Kill(); } catch { Debug.WriteLine("Kill CATCH"); };
            if (streamProcess != null && streamProcess.StartInfo.FileName != string.Empty) try { streamProcess.Kill(); } catch { Debug.WriteLine("Kill CATCH"); };

            inputURL = input;
            outputURL = output;

            streamChecker.checkedFile = outputURL;
            streamChecker.Start(streamChecker.mSecsWaitBeforeStart);

            string whiteList = " "; //whiteList = " -protocol_whitelist \"file,http,https,tcp,tls\" ";
            string fastStart = " "; //-movflags +faststart

            switch (source)
            {
                case 0:
                    map = " ";
                    transCode = " -c copy"; // IPTV
                    break;
                case 1:
                    map = " -map 0:v -map 0:a";
                    transCode = " -vf bwdif -c:v libx264 -preset slow -c:a copy"; // Sat-SD (deinterlace)
                    break;
                case 2:
                    map = " -map 0:v -map 0:a";
                    transCode = " -c:v libx264 -preset fast -c:a copy"; //Sat-HD
                    break;
                case 3:
                    map = " -map 0:v -map 0:a";
                    transCode = " -vf bwdif -c:v libx264 -preset slow -crf 22 -c:a copy"; // VDR-Record
                    break;
                case 4:
                    map = " -map 0:v -map 0:a";
                    transCode = " -c copy";
                    break;
                case 5:
                    map = " ";
                    transCode = " -c copy"; // IPTV (YouTube, MediaThek, ...)
                    streamerProgram = 1;
                    break;
            }
            switch (streamerProgram)
            {
                case 0:
                    streamProcess = ConsoleUtil.StartProgram(ffMpegExeFile, progressFile + whiteList + "-i " + inputURL + map + transCode + fastStart + " -y \"" + outputURL +"\""); // + " -movflags frag_keyframe+empty_moov" //.mkv
                    break;
                case 1:
                    streamProcess = ConsoleUtil.StartProgram("streamlink", "-f -o \"" + outputURL + "\" \"" + inputURL + "\" best");
                    break;
            }
            RecordingStarted?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            switch (streamerProgram)
            {
                case 0:
                    if (streamProcess.StartInfo.RedirectStandardInput) streamProcess.StandardInput.WriteLine("q");
                    break;
                case 1:
                    ConsoleUtil.StopProgram(streamProcess);
                    break;
            }
            RecordingStopped?.Invoke(this, EventArgs.Empty);
        }
    }
}
