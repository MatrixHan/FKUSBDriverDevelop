using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
//---------------------------------------------------------------------
namespace FKUsbTracer
{
    public class FKBufferedListView : ListView
    {
        public FKBufferedListView()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public void SuspendDrawing()
        {
            SendMessage(Handle, WM_SETREDRAW, false, 0);
        }

        public void ResumeDrawing()
        {
            SendMessage(Handle, WM_SETREDRAW, true, 0);
            Refresh();
        }

        public string CopyContents(bool onlySelectedRows)
        {
            StringBuilder buffer = new StringBuilder();

            // 头部
            for (int i = 0; i < Columns.Count; i++)
            {
                buffer.Append(Columns[i].Text);
                buffer.Append("\t");
            }
            buffer.Append("\r\n");

            // 行
            for (int i = 0; i < Items.Count; i++)
            {
                if (!onlySelectedRows || Items[i].Selected)
                {
                    for (int j = 0; j < Columns.Count; j++)
                    {
                        buffer.Append(Items[i].SubItems[j].Text);
                        buffer.Append("\t");
                    }
                    buffer.Append("\r\n");
                }
            }

            return buffer.ToString();
        }

        public void CopyToClipboard(bool onlySelectedRows)
        {
            Clipboard.SetText(CopyContents(onlySelectedRows).ToString());
        }
    }
}
