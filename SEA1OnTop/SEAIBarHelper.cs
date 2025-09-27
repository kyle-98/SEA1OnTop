using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace SEA1OnTop
{
     public static class SEAIBarHelper
     {
          [StructLayout(LayoutKind.Sequential)]
          private struct SEAIBARDATA
          {
               public int cbSize;
               public IntPtr hWnd;
               public uint uCallbackMessage;
               public uint uEdge;
               public RECT rc;
               public int lParam;
          }

          [StructLayout(LayoutKind.Sequential)]
          private struct RECT
          {
               public int left, top, right, bottom;
          }

          [DllImport("shell32.dll", CharSet = CharSet.Auto)]
          private static extern uint SHAppBarMessage(uint dwMessage, ref SEAIBARDATA pData);

          [DllImport("user32.dll")]
          private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

          private static readonly IntPtr HWND_TOP = new IntPtr(0);
          private const uint SWP_SHOWWINDOW = 0x0040;
          private const uint SWP_NOACTIVATE = 0x0010;

          private const int ABM_NEW = 0x00000000;
          private const int ABM_REMOVE = 0x00000001;
          private const int ABM_QUERYPOS = 0x00000002;
          private const int ABM_SETPOS = 0x00000003;
          private const int ABE_TOP = 1;

          public static void RegisterBar(Window window, int heightInDips)
          {
               var helper = new WindowInteropHelper(window);
               SEAIBARDATA abd = new();
               abd.cbSize = Marshal.SizeOf(typeof(SEAIBARDATA));
               abd.hWnd = helper.Handle;
               abd.uEdge = ABE_TOP;

               var source = PresentationSource.FromVisual(window);
               if (source == null) return;
               Matrix transformToDevice = source.CompositionTarget.TransformToDevice;

               Point screenWidthInPixels = transformToDevice.Transform(new Point(SystemParameters.PrimaryScreenWidth, 0));
               Point heightInPixels = transformToDevice.Transform(new Point(0, heightInDips));

               abd.rc.left = 0;
               abd.rc.top = 0;
               abd.rc.right = (int)screenWidthInPixels.X;
               abd.rc.bottom = (int)heightInPixels.Y;

               SHAppBarMessage(ABM_NEW, ref abd);
               SHAppBarMessage(ABM_QUERYPOS, ref abd);
               SHAppBarMessage(ABM_SETPOS, ref abd);

               SetWindowPos(
                   abd.hWnd,
                   HWND_TOP,
                   abd.rc.left,
                   abd.rc.top,
                   abd.rc.right - abd.rc.left,
                   abd.rc.bottom - abd.rc.top,
                   SWP_NOACTIVATE | SWP_SHOWWINDOW
               );

               Matrix transformFromDevice = source.CompositionTarget.TransformFromDevice;

               Point finalTopLeftInDips = transformFromDevice.Transform(new Point(abd.rc.left, abd.rc.top));
               Point finalBottomRightInDips = transformFromDevice.Transform(new Point(abd.rc.right, abd.rc.bottom));

               window.Left = finalTopLeftInDips.X;
               window.Top = finalTopLeftInDips.Y;
               window.Width = finalBottomRightInDips.X - finalTopLeftInDips.X;
               window.Height = finalBottomRightInDips.Y - finalTopLeftInDips.Y;
          }

          public static void UnregisterBar(Window window)
          {
               var helper = new WindowInteropHelper(window);
               SEAIBARDATA abd = new()
               {
                    cbSize = Marshal.SizeOf(typeof(SEAIBARDATA)),
                    hWnd = helper.Handle
               };
               SHAppBarMessage(ABM_REMOVE, ref abd);
          }
     }
}