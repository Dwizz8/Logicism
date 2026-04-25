using System;
using System.Drawing;

namespace Circuits
{
    public class NotGate : Gate
    {

        /// <summary>
        /// Initialises the Gate.
        /// </summary>
        /// <param name="x">The x position of the gate</param>
        /// <param name="y">The y position of the gate</param>
        public NotGate(int x, int y ) : base(x, y)
        {
            //One input pin
            pins.Add(new Pin(this, true, 20));

            //One output pin
            pins.Add(new Pin(this, false, 20));

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
            pins[1].X = x + WIDTH + GAP;
            pins[1].Y = y + HEIGHT / 2;
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

            Point[] topTriangle = {
                new Point(left, top),          // top-left
                new Point(left + WIDTH, top + HEIGHT / 2),   // output tip (middle right)
                new Point(left, top + HEIGHT / 2)            // middle-left
            };

            paper.FillPolygon(brush, topTriangle);

            // Bottom triangle
            Point[] bottomTriangle = {
                new Point(left, top + HEIGHT / 2),           // middle-left
                new Point(left + WIDTH, top + HEIGHT / 2),   // output tip
                new Point(left, top + HEIGHT)  // bottom-left
            };
            paper.FillPolygon(brush, bottomTriangle);

            int circleSize = 10;  // diameter of the NOT bubble
            int circleRadius = circleSize / 2;

            // Center of the output tip
            int tipX = left + WIDTH;
            int tipY = top + HEIGHT / 2;

            // Draw circle centered on tip
            paper.FillEllipse(brush, tipX - circleRadius, tipY - circleRadius, circleSize, circleSize);

            //Note: You can also use the images that have been imported into the project if you wish,
            //      using the code below.  You will need to space the pins out a bit more in the constructor.
            //      There are provided images for the other gates and selected versions of the gates as well.
            //paper.DrawImage(Properties.Resources.OrGate, Left, Top);
        }

        /// <summary>
        /// Evaluates the NOT gate
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            //Ensures that the gate is connected to something before attempting to use recursion to evaluate it
            if (pins[0].InputWire == null)
                return false;

            Gate gateA = pins[0].InputWire.FromPin.Owner;

            return !gateA.Evaluate();
        }

        /// <summary>
        /// Clones the NOT gate, offsetting it slightly so the user can see both gates
        /// </summary>
        /// <returns></returns>
        public override Gate Clone()
        {
            NotGate Notg = new NotGate(left, top);

            Notg.MoveTo(left + 10, top + 10);

            return Notg;
        }
    }
}
