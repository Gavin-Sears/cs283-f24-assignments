using System;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Principal;
using System.Windows.Forms;
using static Physics2D;
using static Physics2D.Vector2;

public class Game
{
    public Player p1;
    public Block b1;

    public class Player
    {
        public PhysicsSphere _PS;
        //public Image mImg;

        public Player()
        {
            this._PS = new PhysicsSphere(
                    5,
                    1,
                    new Vector2(Window.width / 2.0f, Window.height / 2.0f),
                    new Vector2(0.0f, 19.6f),
                    new Vector2(0.0f, 0.0f),
                    new Vector2(0.0f, 0.0f),
                    0.0f
                );
        }

        public Vector2 getPosition()
        {
            return this._PS._position;
        }

        public float getRadius()
        {
            return this._PS._radius;
        }

        public void Update(float dt)
        {
            this._PS.Update(dt);
        }
    }

    public class Block
    {
        public CollisionRect _CR;
        //public Image mImg;

        public float _height { get; }
        public float _length { get; }
        public Vector2 _position { get; }

        public Block(float l, float h, Vector2 p)
        {
            this._height = h;
            this._length = l;

            this._position = p;

            this._CR = new CollisionRect(
                    l,
                    h,
                    p
                );
        }

        public void Update(float dt)
        {
        }
    }

    public void Setup()
    {
        p1 = new Player();
        b1 = new Block(
                100,
                20,
                new Vector2(Window.width / 2.0f, Window.height * (3.0f / 4.0f))
            );
    }

    public void Update(float dt)
    {
        p1._PS.CollideRect(b1._CR);
        Console.WriteLine(p1._PS.IntersectRect(b1._CR));
        p1.Update(dt);
    }

    public void Draw(Graphics g)
    {
        Color c = Color.FromArgb(100, 250, 0, 0); // red
        Brush brush = new SolidBrush(c);
        Vector2 ppos = p1.getPosition();
        g.FillEllipse(brush, ppos.X, ppos.Y, 10, 10);

        Color blue = Color.FromArgb(100, 0, 0, 250);
        Brush brush2 = new SolidBrush(blue);
        Vector2 bpos = b1._position;
        g.FillRectangle(brush2, bpos.X, bpos.Y, b1._length, b1._height);
    }

    public void MouseClick(MouseEventArgs mouse)
    {
        if (mouse.Button == MouseButtons.Left)
        {
            System.Console.WriteLine(mouse.Location.X + ", " + mouse.Location.Y);
        }
    }

    public void KeyDown(KeyEventArgs key)
    {
        if (key.KeyCode == Keys.D || key.KeyCode == Keys.Right)
        {
        }
        else if (key.KeyCode== Keys.S || key.KeyCode == Keys.Down)
        {
        }
        else if (key.KeyCode == Keys.A || key.KeyCode == Keys.Left)
        {
        }
        else if (key.KeyCode == Keys.W || key.KeyCode == Keys.Up)
        {
        }
    }
}
