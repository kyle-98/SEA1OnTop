using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace SEA1OnTop.Utils
{
     public static class SEAIBarHelper
     {
          [StructLayout(LayoutKind.Sequential)]
          private struct SEAIBARDATA
          {
               public int cbSize;
               public nint hWnd;
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
          private static extern int SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

          private static readonly nint HWND_TOP = new nint(0);
          private const uint SWP_SHOWWINDOW = 0x0040;
          private const uint SWP_NOACTIVATE = 0x0010;

          private const int ABM_NEW = 0x00000000;
          private const int ABM_REMOVE = 0x00000001;
          private const int ABM_QUERYPOS = 0x00000002;
          private const int ABM_SETPOS = 0x00000003;
          private const int ABE_TOP = 1;

          public static void RegisterBar(Window window, int heightInDips, int leftPx, int topPx, int widthPx)
          {
               var helper = new WindowInteropHelper(window);
               SEAIBARDATA abd = new();
               abd.cbSize = Marshal.SizeOf(typeof(SEAIBARDATA));
               abd.hWnd = helper.Handle;
               abd.uEdge = ABE_TOP;

               abd.rc.left = leftPx;
               abd.rc.top = topPx;
               abd.rc.right = leftPx + widthPx;

               var source = PresentationSource.FromVisual(window);
               Matrix transform = source?.CompositionTarget.TransformToDevice ?? Matrix.Identity;
               abd.rc.bottom = abd.rc.top + (int)(heightInDips * transform.M22);

               SHAppBarMessage(ABM_NEW, ref abd);
               SHAppBarMessage(ABM_QUERYPOS, ref abd);
               SHAppBarMessage(ABM_SETPOS, ref abd);

               SetWindowPos(abd.hWnd, HWND_TOP, abd.rc.left, abd.rc.top, abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, SWP_NOACTIVATE | SWP_SHOWWINDOW);

               Matrix transformFromDevice = source?.CompositionTarget.TransformFromDevice ?? Matrix.Identity;
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