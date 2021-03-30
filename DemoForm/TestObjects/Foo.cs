using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEditorControl.TestObjects
{
    public class Foo
    {
        public enum EOO
        {
            Im_Not_A_Value = 1,
            Im_A_Value = 2,
            Filler1 = 3,
            Filler2 = 4,
            Filler3 = 5,
        }
        public int x = 0;
        public int y = 0;
        

        

        public Moo var_moo = new Moo();

        public int width = 30;
        public int height = 30;

        public Foo(int _x, int _y, int _w, int _h)
        {
            x = _x;
            y = _y;
            width = _w;
            height = _h;
        }
        public Rectangle GetBounds()
        {
            return new Rectangle(x, y, width, height);
        }

        public void Render(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.Black), GetBounds());
        }
    }
}
