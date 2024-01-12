using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace radioZiner
{
    public class VideoRecorder
    {
        public event EventHandler BeforeRecordingAdded;
        public event EventHandler RecordingAdded;
        public event EventHandler RecordingRemoved;

        private string recordingPath = "";
        public string formatString = "";

        public Dictionary<string, StreamRecorder> recordings = new Dictionary<string, StreamRecorder>();

        public VideoRecorder(string recordingPath)
        {
            this.recordingPath = recordingPath;
        }

        public StreamRecorder StartRecording(M3u.TvgChannel channel, int source = 0)
        {
            StreamRecorder rec = null;
            if (!recordings.ContainsKey(channel.id))
            {
                string fileName = Path.Combine(recordingPath, channel.id + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".mkv"); //.mkv
                rec = new StreamRecorder(fileName);
                BeforeRecordingAdded?.Invoke(rec, EventArgs.Empty);
                recordings.Add(channel.id, rec);
                rec.Start(channel, source);
                RecordingAdded?.Invoke(rec, EventArgs.Empty);
            }
            return rec;
        }

        public void StopRecording(string channelID)
        {
            if (channelID != null && recordings.ContainsKey(channelID))
            {
                var rec = recordings[channelID];
                rec.Stop();
                recordings.Remove(channelID);
                RecordingRemoved?.Invoke(rec, EventArgs.Empty);
            }
        }

        public void StopAllRecordings()
        {
            foreach (var rec in recordings) rec.Value.Stop();
            recordings.Clear();
        }
    }
}
