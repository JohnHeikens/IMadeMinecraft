using System.Runtime.InteropServices;
using System.Drawing;
using System;
using System.IO;
using System.Windows.Forms;
//Add the methods to your class with P/Invoke
public class drs
{
    [DllImport("User32.dll")]
    public static extern IntPtr GetDC(IntPtr hwnd);
    [DllImport("User32.dll")]
    public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
    //Get a Graphics object for the entire screen and draw a rectangle with it:
    public static void Drawonscreen(Bitmap b)
    {
        IntPtr desktopPtr = GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromHdc(desktopPtr))
            {
                g.DrawImage(b, 0,0,1920,1080);
            g.Dispose();
        }  //g is disposed
        ReleaseDC(IntPtr.Zero, desktopPtr);
    }
}