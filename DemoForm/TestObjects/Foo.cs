using Editor.Options.Attributes;    
using System.Drawing;


namespace MyEditorControl.TestObjects
{
    [TypeSafeLock(true)]
    public class Foo
    {

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
