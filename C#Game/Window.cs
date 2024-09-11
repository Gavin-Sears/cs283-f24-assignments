// Open a simple window for drawing in C#
// September 2024
// Aline Normoyle

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

public class Window : System.Windows.Forms.Form
{
    private static int _windowHeight = 480;
    public static int height 
    {
        get { return _windowHeight; }
    }

    private static int _windowWidth = 640;
    public static int width 
    {
        get { return _windowWidth; }
    }

    private static System.Random rng = new System.Random();
    public static int RandomRange(int start, int end)
    {
        return rng.Next(start, end);
    }

    private Game _game = new Game();
    private Timer _timer = new Timer(); 
    private DateTime _lastTime;

    public Window()
    {
        InitializeComponent();
        CenterToScreen();
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    private void InitializeComponent()
    {
        this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
        this.DoubleBuffered = true;
        this.ClientSize = new System.Drawing.Size(Window.width, Window.height);
        this.Text = "CS283: Assignment 2";
        this.Resize += new System.EventHandler(this.ResizeCb);
        this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawCb);
        this.KeyDown += new KeyEventHandler(KeyDownCb);
        this.MouseClick += new MouseEventHandler(MouseClickCb);

        _game.Setup();
        _lastTime = DateTime.Now;
        _timer.Tick += TickCb;
        _timer.Interval = 100;
        _timer.Start();
    }

    private void TickCb(object sender, EventArgs args)
    {
        // GameLoop is implemented here
        DateTime current = DateTime.Now;
        TimeSpan dt = current - _lastTime;
        _game.Update((float) dt.TotalSeconds);
        Refresh();
        _lastTime = current;
    }

    private void DrawCb(object sender, System.Windows.Forms.PaintEventArgs e)
    {
        _game.Draw(e.Graphics); 
    }

    private void ResizeCb(object sender, System.EventArgs e)
    {
        Control control = (Control)sender;
        _windowHeight = control.Size.Height;
        _windowWidth = control.Size.Width;
    }

    private void KeyDownCb(object sender, KeyEventArgs e)
    {
        _game.KeyDown(e);
    }

    private void MouseClickCb(object sender, MouseEventArgs e)
    {
        _game.MouseClick(e);
    }

    [STAThread]
    static void Main()
    {
        Application.Run(new Window());
    }
}