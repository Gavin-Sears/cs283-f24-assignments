using System;
using System.Collections.Specialized;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Principal;
using System.Windows.Forms;
using static Game;
using static Physics2D;
using static Physics2D.Vector2;

/*
 * Stephen Gavin Sears
 * Professor Aline Normoyle
 * Game Programming
 * September 12, 2024
 * 
 * Ball bouncing game
*/

public class Game
{
    public Player p1;
    public CollisionRect[] rects;
    public Block[] blocks;

    public float deathLine = -500.0f;
    public Block theGoal;
    public bool win = false;

    // Drawing tools
    Color red = Color.FromArgb(100, 250, 0, 0); // red
    Brush playerBrush;

    Color gren = Color.FromArgb(100, 0, 255, 0); // green
    Brush goalBrush;

    public class Player
    {
        public PhysicsSphere _PS;
        //public Image mImg;

        public Player()
        {
            this._PS = new PhysicsSphere(
                    7,
                    1,
                    new Vector2(Window.width / 2.0f, Window.height / 2.0f),
                    new Vector2(0.0f, 19.6f),
                    new Vector2(0.0f, 0.0f)
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
    }

    public void Setup()
    {
        // Setting up brushes
        playerBrush = new SolidBrush(red);
        goalBrush = new SolidBrush(gren);

        p1 = new Player();
        Block b1 = new Block(
                100,
                250,
                new Vector2((Window.width / 2.0f) - 50, Window.height - 125)
            );
        Block b2 = new Block(
                100,
                250,
                new Vector2((Window.width / 2.0f) + 50, Window.height - 250)
            );
        Block b3 = new Block(
                100,
                250,
                new Vector2((Window.width / 2.0f) + 50, Window.height - 700)
            );
        Block b4 = new Block(
                100,
                750,
                new Vector2((Window.width / 2.0f) + -150, Window.height - 500)
            );

        blocks = [b1, b2, b3, b4];

        theGoal = new Block(
                50,
                50,
                new Vector2((Window.width / 2.0f) + 50, Window.height - 950)
            );
    }

    public void CollideBlocks()
    {
        foreach (Block block in blocks)
        {
            p1._PS.CollideRect(block._CR);
        }
    }

    public void CheckForDeath(Vector2 ppos)
    {
        if ((deathLine + ppos.Y) > 0.0f)
        {
            Console.WriteLine("You Lose!");
            Setup();
        }
    }

    public void CheckForVictory(Vector2 ppos)
    {
        if (p1._PS.IntersectRect(theGoal._CR))
        {
            win = true;
        }
    }

    public void Update(float dt)
    {
        p1.Update(dt);
        CollideBlocks();
        Vector2 ppos = p1.getPosition();
        CheckForDeath(ppos);
        CheckForVictory(ppos);
    }

    public void Draw(Graphics g)
    {
        Vector2 ppos = p1.getPosition();
        g.FillEllipse(playerBrush, (Window.width / 2.0f), (Window.height / 2.0f), p1._PS._radius * 2, p1._PS._radius * 2);
        Vector2 conversion = new Vector2(-ppos.X + (Window.width / 2.0f), -ppos.Y + (Window.height / 2.0f));

        Vector2 gpos = theGoal._position;
        g.FillRectangle(goalBrush, gpos.X + conversion.X, gpos.Y + conversion.Y, theGoal._length, theGoal._height);

        DrawBlocks(g, conversion);

        if (win)
        {
            // font
            Font comicSans = new Font("Comic Sans MS", 32);
            // color and brush
            Color mag = Color.FromArgb(255, 250, 0, 250); // magenta
            Brush winBrush = new SolidBrush(mag);
            // String format
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;

            g.DrawString(
                    "You are Incredible!\ngame by Gavin Sears",
                    comicSans,
                    winBrush,
                    (Window.width / 2.0f),
                    (Window.height / 2.0f) - 50,
                    drawFormat
                );
        }
    }

    public void DrawBlocks(Graphics g, Vector2 conversion)
    {
        Color blue = Color.FromArgb(100, 0, 0, 250);
        Brush brush = new SolidBrush(blue);
        foreach (Block block in blocks)
        {
            Vector2 bpos = block._position;
            g.FillRectangle(brush, bpos.X + conversion.X, bpos.Y + conversion.Y, block._length, block._height);
        }

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
            p1._PS.ApplyForce(new Vector2(4.0f, 0.0f));
        }
        else if (key.KeyCode== Keys.S || key.KeyCode == Keys.Down)
        {
            p1._PS.ApplyForce(new Vector2(0.0f, 6.0f));
        }
        else if (key.KeyCode == Keys.A || key.KeyCode == Keys.Left)
        {
            p1._PS.ApplyForce(new Vector2(-4.0f, 0.0f));
        }
        else if (key.KeyCode == Keys.W || key.KeyCode == Keys.Up)
        {
        }
    }
}
