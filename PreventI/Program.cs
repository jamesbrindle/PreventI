using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
//for processes
using System.Diagnostics;
//for COM add-in
using System.Reflection;

namespace PreventI
{

    class Program
    {
        static void Main(string[] args)
        {
            Delay d = new Delay();
            DateTime start = DateTime.Now;
            Process[] processlist;
            bool willdelay = false;
            //loop forever
            while (true)
            {
                processlist = Process.GetProcesses();
                //add code here to look at running processes, if one is running that should trigger a delay, then delay
                //here we're always delaying the input timer no matter what
                willdelay = true;
                if (willdelay == true)
                {
                    d.delay();
                    Console.WriteLine("Time delayed at: " + DateTime.Now);
                }
                else
                {
                    Console.WriteLine("Do not delay timer.");
                }
                Console.WriteLine("Input Timer currently at: " + d.GetLastInputTime());
                //wait a few seconds
                System.Threading.Thread.Sleep(10000);
                //reset variable
                willdelay = false;
            }
        }
    }
    //here's the heart of it
    class Delay
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        [DllImport("user32.dll")]
        public static extern void keybd_event(
        byte bVk,
        byte bScan,
        uint dwFlags,
        uint dwExtraInfo
        );

        const int VK_CONTROL = 0x11;
        const uint KEYEVENTF_KEYUP = 0x2;
        bool ctrlPressed = false;

        public Delay()
        {

        }

        //we're actually triggering a keyboard event.
        public void delay()
        {
            keybd_event((byte)VK_CONTROL, 0, KEYEVENTF_KEYUP, 0);
        }

        //you can access the timer used to monitor input events.  This timer gets reset when an event is triggered and delays things like locking of the screen.
        public int GetLastInputTime()
        {
            int idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envTicks = Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                int lastInputTick = lastInputInfo.dwTime;
                idleTime = envTicks - lastInputTick;
            }
            return ((idleTime > 0) ? (idleTime / 1000) : 0);
        }
    }
}
[StructLayout(LayoutKind.Sequential)]
struct LASTINPUTINFO
{
    public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

    [MarshalAs(UnmanagedType.U4)]
    public int cbSize;
    [MarshalAs(UnmanagedType.U4)]
    //public UInt32 dwTime;
    public int dwTime;
}