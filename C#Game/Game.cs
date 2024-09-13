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
using static System.Net.Mime.MediaTypeNames;

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

    public Vector2 startPos = new Vector2(
        Window.width / 2.0f, 
        Window.height / 2.0f
    );

    // Drawing tools
    Color sky = Color.FromArgb(100, 70, 155, 155); // cyanish color
    Brush backgroundBrush;

    Color gren = Color.FromArgb(100, 0, 255, 0); // green
    Brush goalBrush;

    Color purple = Color.FromArgb(100, 230, 0, 250);
    Brush blockBrush;

    Color yellow = Color.FromArgb(255, 255, 255, 0);
    Brush starBrush;

    System.Drawing.Image characterImg = System.Drawing.Image.FromFile("character.png");
    System.Drawing.Image starImg = System.Drawing.Image.FromFile("star.png");

    public class Player
    {
        public PhysicsSphere _PS;

        public Player(Vector2 sPos)
        {
            this._PS = new PhysicsSphere(
                    7,
                    1,
                    sPos,
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
        backgroundBrush = new SolidBrush(sky);
        goalBrush = new SolidBrush(gren);
        blockBrush = new SolidBrush(purple);
        starBrush = new SolidBrush(yellow);

        p1 = new Player(startPos);
        Block b1 = new Block(
                100,
                250,
                new Vector2(
                    (Window.width / 2.0f) - 50, 
                    Window.height - 125
                )
            );
        Block b2 = new Block(
                100,
                500,
                new Vector2(
                    (Window.width / 2.0f) + 50, 
                    Window.height - 250
                )
            );
        Block b3 = new Block(
                100,
                750,
                new Vector2(
                    (Window.width / 2.0f) + 150, 
                    Window.height - 500
                )
            );
        Block b4 = new Block(
                100,
                750,
                new Vector2(
                    (Window.width / 2.0f) - 150, 
                    Window.height - 500
                )
            );
        Block b5 = new Block(
                100,
                1500,
                new Vector2(
                    (Window.width / 2.0f) - 250, 
                    Window.height - 1000
                )
            );
        Block secret1 = new Block(
                100,
                100,
                new Vector2(
                    (Window.width / 2.0f) - 350, 
                    Window.height - 1000
                )
            );
        Block secret2 = new Block(
                100,
                350,
                new Vector2(
                    (Window.width / 2.0f) - 450, 
                    Window.height - 1250
                )
            );
        Block ceiling = new Block(
                5000,
                750,
                new Vector2(
                    (Window.width / 2.0f) - 450, 
                    Window.height - 2000
                )
            );
        Block beginning1 = new Block(
                400,
                750,
                new Vector2(
                    (Window.width / 2.0f) + 250, 
                    Window.height - 700
                )
            );
        Block beginning2 = new Block(
                400,
                750,
                new Vector2(
                    (Window.width / 2.0f) + 650, 
                    Window.height - 800
                )
            );
        Block cliffs1 = new Block(
                100,
                750,
                new Vector2(
                    (Window.width / 2.0f) + 1050, 
                    Window.height - 900
                )
            );
        Block cliffs2 = new Block(
                100,
                750,
                new Vector2(
                    (Window.width / 2.0f) + 1350,
                    Window.height - 1000
                )
            );
        Block end1 = new Block(
                100,
                750,
                new Vector2(
                    (Window.width / 2.0f) + 1750, 
                    Window.height - 1100
                )
            );
        Block end2 = new Block(
                100,
                1000,
                new Vector2(
                    (Window.width / 2.0f) + 1850, 
                    Window.height - 1250
                )
            );

        blocks = [
            b1, 
            b2, 
            b3, 
            b4, 
            b5, 
            secret1, 
            secret2, 
            ceiling, 
            beginning1, 
            beginning2, 
            cliffs1, 
            cliffs2, 
            end1, 
            end2
        ];

        theGoal = new Block(
                100,
                150,
                new Vector2((Window.width / 2.0f) + 1750, Window.height - 1250)
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
            // For those who are frustrated, I made a checkpoint to save time
            startPos = new Vector2(
                (Window.width / 2.0f) + 850, 
                Window.height - 1000
            );
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
        // background
        g.FillRectangle(
            backgroundBrush, 
            0.0f, 
            0.0f,
            Window.width, 
            Window.height
        );

        // player
        Vector2 ppos = p1.getPosition();
        g.DrawImage(
            characterImg, 
            (Window.width / 2.0f), 
            (Window.height / 2.0f), 
            p1._PS._radius * 2, 
            p1._PS._radius * 2
        );
        Vector2 conversion = new Vector2(
            -ppos.X + (Window.width / 2.0f), 
            -ppos.Y + (Window.height / 2.0f)
        );

        Vector2 gpos = theGoal._position;
        g.FillRectangle(
            goalBrush, 
            gpos.X + conversion.X, 
            gpos.Y + conversion.Y, 
            theGoal._length, 
            theGoal._height
        );

        // secret
        g.DrawImage(
            starImg,
            (Window.width / 2.0f) - 275 + conversion.X,
            Window.height - 1150 + conversion.Y,
            64,
            64
        );

        // level geometry
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
        foreach (Block block in blocks)
        {
            Vector2 bpos = block._position;
            g.FillRectangle(
                blockBrush, 
                bpos.X + conversion.X, 
                bpos.Y + conversion.Y,
                block._length, 
                block._height
            );
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
            p1._PS.ApplyForce(new Vector2(0.0f, 5.0f));
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
