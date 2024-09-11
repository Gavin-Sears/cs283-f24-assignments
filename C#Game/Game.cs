using System;
using System.Drawing;
using System.Windows.Forms;

public class Game
{
    public void Setup()
    {
    }

    public void Update(float dt)
    {
    }

    public void Draw(Graphics g)
    {
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
