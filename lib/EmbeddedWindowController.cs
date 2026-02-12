using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EmbedForm
{
    /// <summary>
    /// Embedded Window Controller - Intercepts and blocks move/resize messages
    /// </summary>
    public class EmbeddedWindowController : NativeWindow
    {
        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        // Window Messages
        private const int WM_WINDOWPOSCHANGING = 0x0046;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        private const int WM_NCRBUTTONDOWN = 0x00A4;
        private const int WM_MOVING = 0x0216;
        private const int WM_SIZING = 0x0214;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;

        private Panel container;
        private bool isLocked = true;

        public EmbeddedWindowController(IntPtr handle, Panel containerPanel)
        {
            container = containerPanel;
            if (handle != IntPtr.Zero)
            {
                AssignHandle(handle);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (isLocked)
            {
                // Intercept window position changing message
                if (m.Msg == WM_WINDOWPOSCHANGING)
                {
                    WINDOWPOS pos = (WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));

                    // Force maintain position and size
                    pos.x = 0;
                    pos.y = 0;
                    pos.cx = container.Width;
                    pos.cy = container.Height;
                    pos.flags &= ~SWP_NOMOVE;
                    pos.flags &= ~SWP_NOSIZE;

                    Marshal.StructureToPtr(pos, m.LParam, true);
                }
                // Intercept moving and sizing messages
                else if (m.Msg == WM_MOVING || m.Msg == WM_SIZING)
                {
                    return; // Directly ignore
                }
                // Intercept non-client area mouse operations (title bar dragging, etc.)
                else if (m.Msg == WM_NCLBUTTONDOWN || m.Msg == WM_NCLBUTTONDBLCLK || m.Msg == WM_NCRBUTTONDOWN)
                {
                    return; // Directly ignore
                }
            }

            base.WndProc(ref m);
        }

        public void UpdateSize()
        {
            if (Handle != IntPtr.Zero && container != null)
            {
                MoveWindow(Handle, 0, 75, container.Width, container.Height - 75, true);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }
    }
}