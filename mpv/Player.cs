
using System;
using System.Runtime.InteropServices;

using static libmpv;

namespace radioZiner
{
    public class MpvPlayer
    {
        public IntPtr Handle { get; set; }

        public void Init(IntPtr handle)
        {
            Handle = mpv_create();

            if (Handle == IntPtr.Zero)
                throw new Exception("error mpv_create");
            mpv_error err = mpv_initialize(Handle);
            SetPropertyLong("wid", handle.ToInt64());
        }

        public void Destroy()
        {
            mpv_destroy(Handle);
        }

        public void Command(string command)
        {
            mpv_error err = mpv_command_string(Handle, command);
            if (err < 0)
                HandleError(err, "error executing command: " + command);
        }

        public void CommandV(params string[] args)
        {
            int count = args.Length + 1;
            IntPtr[] pointers = new IntPtr[count];
            IntPtr rootPtr = Marshal.AllocHGlobal(IntPtr.Size * count);

            for (int index = 0; index < args.Length; index++)
            {
                var bytes = GetUtf8Bytes(args[index]);
                IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                pointers[index] = ptr;
            }

            Marshal.Copy(pointers, 0, rootPtr, count);
            mpv_error err = mpv_command(Handle, rootPtr);

            foreach (IntPtr ptr in pointers)
                Marshal.FreeHGlobal(ptr);

            Marshal.FreeHGlobal(rootPtr);
            if (err < 0)
                HandleError(err, "error executing command: " + string.Join("\n", args));
        }

        public bool GetPropertyBool(string name)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_FLAG, out IntPtr lpBuffer);
            if (err < 0)
                HandleError(err, "error getting property: " + name);
            return lpBuffer.ToInt32() != 0;
        }

        public void SetPropertyBool(string name, bool value)
        {
            long val = value ? 1 : 0;
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_FLAG, ref val);
            if (err < 0)
                HandleError(err, $"error setting property: {name} = {value}");
        }

        public int GetPropertyInt(string name)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);
            if (err < 0)
            {
                HandleError(err, "error getting property: " + name);
            }
            return lpBuffer.ToInt32();
        }

        public void SetPropertyInt(string name, int value)
        {
            long val = value;
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref val);
            if (err < 0)
                HandleError(err, $"error setting property: {name} = {value}");
        }

        public void SetPropertyLong(string name, long value)
        {
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_INT64, ref value);
            if (err < 0)
                HandleError(err, $"error setting property: {name} = {value}");
        }

        public long GetPropertyLong(string name)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_INT64, out IntPtr lpBuffer);
            if (err < 0)
                HandleError(err, "error getting property: " + name);
            return lpBuffer.ToInt64();
        }

        public double GetPropertyDouble(string name, bool handleError = true)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_DOUBLE, out double value);
            if (err < 0 && handleError)
            {
                HandleError(err, "error getting property: " + name);
            }
            return value;
        }

        public void SetPropertyDouble(string name, double value)
        {
            double val = value;
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_DOUBLE, ref val);
            if (err < 0)
                HandleError(err, $"error setting property: {name} = {value}");
        }

        public string GetPropertyString(string name)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_STRING, out IntPtr lpBuffer);

            if (err == 0)
            {
                string ret = ConvertFromUtf8(lpBuffer);
                mpv_free(lpBuffer);
                return ret;
            }

            if (err < 0)
            {
                HandleError(err, "error getting property: " + name);
            }

            return "";
        }

        public void SetPropertyString(string name, string value)
        {
            byte[] bytes = GetUtf8Bytes(value);
            mpv_error err = mpv_set_property(Handle, GetUtf8Bytes(name), mpv_format.MPV_FORMAT_STRING, ref bytes);
            if (err < 0)
                HandleError(err, $"error setting property: {name} = {value}");
        }

        public string GetPropertyOsdString(string name)
        {
            mpv_error err = mpv_get_property(Handle, GetUtf8Bytes(name),
                mpv_format.MPV_FORMAT_OSD_STRING, out IntPtr lpBuffer);

            if (err == 0)
            {
                string ret = ConvertFromUtf8(lpBuffer);
                mpv_free(lpBuffer);
                return ret;
            }

            if (err < 0)
                HandleError(err, "error getting property: " + name);

            return "";
        }

        public void HandleError(mpv_error err, string msg)
        {
            //Terminal.WriteError(msg);
            //Terminal.WriteError(GetError(err));
        }
    }
}
