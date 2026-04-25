using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuits
{
    /// <summary>
    /// This class implements an AND gate with two inputs
    /// and one output.
    /// </summary>
    public class AndGate : Gate
    {
        /// <summary>
        /// Initialises the Gate.
        /// </summary>
        /// <param name="x">The x position of the gate</param>
        /// <param name="y">The y position of the gate</param>
        public AndGate(int x, int y) : base(x,y)
        {
            //Add the two input pins to the gate
            pins.Add(new Pin(this, true, 20));
            pins.Add(new Pin(this, true, 20));
            //Add the output pin to the gate
            pins.Add(new Pin(this, false, 20));
            //move the gate and the pins to the position passed in
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
            pins[0].Y = y + GAP;
            pins[1].X = x - GAP;
            pins[1].Y = y + HEIGHT - GAP;
            pins[2].X = x + WIDTH + GAP;
            pins[2].Y = y + HEIGHT / 2;
        }

        /// <summary>
        /// Specifically draws the AND Gate
        /// </summary>
        /// <param name="paper"></param>
        public override void Draw(Graphics paper)
        {
            //Check if the gate has been selected

            if (selected)
            {
                brush = selectedBrush;
            }
            else
            {
                brush = normalBrush;
            }

            //Draw each of the pins
            foreach (Pin p in pins)
                p.Draw(paper);

            // AND is simple, so we can use a circle plus a rectangle.
            // An alternative would be to use a bitmap.
            paper.FillEllipse(brush, left, top, WIDTH, HEIGHT);
            paper.FillRectangle(brush, left, top, WIDTH / 2, HEIGHT);

            //Note: You can also use the images that have been imported into the project if you wish,
            //      using the code below.  You will need to space the pins out a bit more in the constructor.
            //      There are provided images for the other gates and selected versions of the gates as well.
            //paper.DrawImage(Properties.Resources.AndGate, Left, Top);
        }

        /// <summary>
        /// Evaluates the AND gate by evaluating both input gates
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            //Ensures that the gate is connected to something before attempting to use recursion to evaluate it
            if (pins[0].InputWire == null || pins[1].InputWire == null)
                return false;

            Gate gateA = pins[0].InputWire.FromPin.Owner;
            Gate gateB = pins[1].InputWire.FromPin.Owner;

            return gateA.Evaluate() && gateB.Evaluate();
        }

        /// <summary>
        /// Clones the AND gate and offsets it slightly
        /// </summary>
        /// <returns></returns>
        public override Gate Clone()
        {
            AndGate Andg = new AndGate(left, top);

            Andg.MoveTo(left + 10, top + 10);

            return Andg;
        }
    }
}
