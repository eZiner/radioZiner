using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace radioZiner
{
    public class StreamChecker
    {
        public object Tag { get; set; }
        private int _streamState = 2; //-1=waiting; 0=started; 1=rec; 2=ready;
        public int StreamState
        {
            get { return _streamState; }
            private set { _streamState = value; StateChanged?.Invoke(this, EventArgs.Empty); }
        }
        public string checkedFile = "";
        private bool streamPlayRequest = false;
        private long streamSize = 0;
        private long streamSizeBefore = 0;
        private long mSecsSinceLastChange = 0;
        private long mSecsSinceLastStart = 0;

        public event EventHandler StateChanged;
        public event EventHandler PlayRecordingRequest;

        public long mSecsWaitBeforeStart = 7000;

        private Timer timer = new Timer();


        public StreamChecker(string f)
        {
            checkedFile = f;
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = 500;
            timer.Enabled = true;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            CheckState(timer.Interval);
            if ((PlayRecordingRequest != null) && PlayRequest())
            {
                PlayRecordingRequest?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Start(long wait = 7000)
        {
            mSecsWaitBeforeStart = wait;

            StreamState = ((streamSizeBefore = GetSize()) == 0) ? 0 : -1;
            streamPlayRequest = true;
        }

        public bool PlayRequest()
        {
            if (streamPlayRequest && StreamState > 0 && mSecsSinceLastStart >= mSecsWaitBeforeStart)
            {
                streamPlayRequest = false;
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public int CheckState(long mSecs)
        {
            GetSize();

            switch (StreamState)
            {
                case -1:
                    if (streamSize == 0 || streamSize < streamSizeBefore)
                    {
                        StreamState = 0;
                        mSecsSinceLastChange = 0;
                        streamSizeBefore = streamSize;
                        return (StreamState);
                    }
                    break;
                case 0:
                    if (streamSize > streamSizeBefore)
                    {
                        StreamState = 1;
                        mSecsSinceLastChange = 0;
                        mSecsSinceLastStart = 0;
                        streamSizeBefore = streamSize;
                        return (StreamState);
                    }
                    break;
                case 1:
                    mSecsSinceLastStart += mSecs;
                    if (streamSize > streamSizeBefore)
                    {
                        mSecsSinceLastChange = 0;
                        streamSizeBefore = streamSize;
                        return (StreamState);
                    }
                    else if (mSecsSinceLastChange > 10000)
                    {
                        StreamState = 2;
                    }
                    break;
                case 2:
                    if (streamSize > streamSizeBefore)
                    {
                        StreamState = 1;
                        mSecsSinceLastChange = 0;
                        streamSizeBefore = streamSize;
                        return (StreamState);
                    }
                    break;
            }
            mSecsSinceLastChange += mSecs;
            return (StreamState);
        }

        public long GetSize()
        {
            if (File.Exists(checkedFile))
            {
                FileInfo fi = new FileInfo(checkedFile);
                streamSize = fi.Length;
            }
            else
            {
                streamSize = 0;
            }
            return (streamSize);
        }
    }
}
