using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuits
{
    public class OrGate : Gate
    {
        /// <summary>
        /// Initialises the Gate.
        /// </summary>
        /// <param name="x">The x position of the gate</param>
        /// <param name="y">The y position of the gate</param>
        public OrGate(int x, int y) : base(x, y)
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
        /// Specifically draws the OR Gate
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

            //Sets an offset to make it look more OR like
            int curveOffset = WIDTH / 4;

            Point[] topTriangle = {
                new Point(left - curveOffset, top),          // top-left
                new Point(left + WIDTH, top + HEIGHT / 2),   // output tip (middle right)
                new Point(left, top + HEIGHT / 2)            // middle-left
            };
            
            paper.FillPolygon(brush, topTriangle);

            // Bottom triangle
            Point[] bottomTriangle = {
                new Point(left, top + HEIGHT / 2),           // middle-left
                new Point(left + WIDTH, top + HEIGHT / 2),   // output tip
                new Point(left - curveOffset, top + HEIGHT)  // bottom-left
            };
            paper.FillPolygon(brush, bottomTriangle);

            //Note: You can also use the images that have been imported into the project if you wish,
            //      using the code below.  You will need to space the pins out a bit more in the constructor.
            //      There are provided images for the other gates and selected versions of the gates as well.
            //paper.DrawImage(Properties.Resources.OrGate, Left, Top);
        }

        /// <summary>
        /// Recursively evaluates the output of the OR gate based on its inputs.
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            //Ensures that the gate is connected to something before attempting to use recursion to evaluate it
            //Arguably the more annoying one to deal with, as if one input is not connected, it should be false
            if (pins[0].InputWire == null && pins[1].InputWire != null)
            {
                Gate gateB = pins[1].InputWire.FromPin.Owner;
                return false || gateB.Evaluate();
            }
            else if (pins[1].InputWire == null && pins[0].InputWire != null)
            {
                Gate gateA = pins[0].InputWire.FromPin.Owner;
                return gateA.Evaluate() || false;
            }
            else if (pins[0].InputWire == null && pins[1].InputWire == null)
            {
                return false;
            }
            else
            {
                Gate gateA = pins[0].InputWire.FromPin.Owner;
                Gate gateB = pins[1].InputWire.FromPin.Owner;

                return gateA.Evaluate() || gateB.Evaluate();
            }
        }

        /// <summary>
        /// Clones the OR gate and offsets it slightly so the user can see it.
        /// </summary>
        /// <returns></returns>
        public override Gate Clone()
        {
            OrGate Org = new OrGate(left, top);

            Org.MoveTo(left + 10, top + 10);

            return Org;
        }
    }
}
