using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuits
{
    class OutputLamp : Gate
    {
        private bool _Voltage;
        public OutputLamp(int x, int y) : base(x, y)
        {
            //One input pin
            pins.Add(new Pin(this, true, 20));

            _Voltage = false;

            MoveTo(x, y);
        }

        public override void MoveTo(int x, int y)
        {
            //Debugging message
            Console.WriteLine("pins = " + pins.Count);

            //Set the position of the gate to the values passed in
            left = x;
            top = y;

            // must move the pins too
            pins[0].X = x - GAP;
            pins[0].Y = y + HEIGHT / 2;
        }

        public override void Draw(Graphics paper)
        {
            Brush normalBrush = Brushes.Black;

            Pen pen = new Pen(Color.Black, 5);

            //Check if the gate has been selected
            if (selected)
            {
                brush = selectedBrush;
            }
            else
            {
                brush = normalBrush;
            }

            if (_Voltage)
            {
                pen.Color = Color.Yellow;
            }
            else
            {
                pen.Color = Color.Black;
            }

            //Draw each of the pins
            foreach (Pin p in pins)
                p.Draw(paper);

            // AND is simple, so we can use a circle plus a rectangle.
            // An alternative would be to use a bitmap.
            paper.FillRectangle(brush, left, top, WIDTH, HEIGHT);
            paper.DrawRectangle(pen, left, top, WIDTH, HEIGHT);

            //Note: You can also use the images that have been imported into the project if you wish,
            //      using the code below.  You will need to space the pins out a bit more in the constructor.
            //      There are provided images for the other gates and selected versions of the gates as well.
            //paper.DrawImage(Properties.Resources.AndGate, Left, Top);
        }

        /// <summary>
        /// Evaluates the output lamp by checking the voltage of the input pin
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            //Ensures that the gate is connected to something before attempting to use recursion to evaluate it
            if (pins[0].InputWire == null)
                return false;

            Gate gateA = pins[0].InputWire.FromPin.Owner;
            _Voltage = gateA.Evaluate();

            return _Voltage;
        }

        /// <summary>
        /// Clones the output lamp and offsets it slightly
        /// </summary>
        /// <returns></returns>
        public override Gate Clone()
        {
            OutputLamp Out = new OutputLamp(left, top);

            Out.MoveTo(left + 10, top + 10);

            return Out;
        }
    }
}
