using System.Windows.Forms;
using openview;
using System;
using System.Drawing;
using System.Diagnostics;
using Generating;
namespace Screen {
    class Viewscreen:Form {
        public Gui.Object view=new Gui.Object();
//        public PictureBox curview=new PictureBox();
        public Viewscreen() {
            DoubleBuffered = true;
            WindowState = FormWindowState.Maximized;
            FormBorderStyle = FormBorderStyle.None;
            Show();
            view.Height = Height;
            view.Width = Width;
            view.Left = 0;
            view.Top = 0;
            BackColor = Color.Black;
            BackgroundImageLayout = ImageLayout.Stretch;
            MouseDown += new MouseEventHandler(Callclick);
            MouseMove += new MouseEventHandler(Mousemove);
            FormClosing += Viewscreen_FormClosing;
            KeyDown += Viewscreen_KeyDown;
            KeyUp += Viewscreen_KeyUp;
        }
        private void Viewscreen_KeyUp(object sender, KeyEventArgs e) => view.Releasekey(e.KeyCode);
        private void Viewscreen_KeyDown(object sender, KeyEventArgs e) => view.Presskey(e.KeyCode);
        private void Mousemove(object sender, MouseEventArgs e) => view.Clickonform(e.X, e.Y, e.Button);
        private void Viewscreen_FormClosing(object sender, FormClosingEventArgs e) => e.Cancel = !view.Close();
        private void Callclick(object sender, MouseEventArgs e) => view.Clickonform(e.X, e.Y,e.Button);
        public void Dothings() {
            BackgroundImage = view.Draw();
            //drs.Drawonscreen(view.Draw());
            Application.DoEvents();
        }
    }
}