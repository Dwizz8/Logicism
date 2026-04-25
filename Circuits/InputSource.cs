using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Circuits
{
    class InputSource : Gate
    {
        private bool _IsOn;

        public InputSource(int x, int y) : base(x, y)
        {
            //One output pin
            pins.Add(new Pin(this, false, 20));
            
            IsOn = false;

            MoveTo(x, y);
        }

        /// <summary>
        /// Read and write property to get or set whether the input source is on or off.
        /// </summary>
        public bool IsOn
        {
            get { return _IsOn; }
            set { _IsOn = value; }
        }

        public override void MoveTo(int x, int y)
        {
            //Debugging message
            Console.WriteLine("pins = " + pins.Count);

            //Set the position of the gate to the values passed in
            left = x;
            top = y;

            // must move the pins too
            pins[0].X = x + WIDTH + GAP;
            pins[0].Y = y + HEIGHT / 2;
        }

        /// <summary>
        /// Draws the input source properly.
        /// </summary>
        /// <param name="paper"></param>
        public override void Draw(Graphics paper)
        {
            Brush normalBrush = Brushes.Black;

            string IsOnDisplay;

            //Check if the gate has been selected
            if (selected)
            {
                brush = selectedBrush;
            }
            else
            {
                brush = normalBrush;
            }

            if (IsOn)
            {
                IsOnDisplay = "1";
            }
            else
            {
                IsOnDisplay = "0";
            }
                
            //Draw each of the pins
            foreach (Pin p in pins)
                p.Draw(paper);

            paper.FillRectangle(brush, left, top, WIDTH, HEIGHT);
            paper.DrawString(IsOnDisplay, myFont, Brushes.White, left, top);

            //Note: You can also use the images that have been imported into the project if you wish,
            //      using the code below.  You will need to space the pins out a bit more in the constructor.
            //      There are provided images for the other gates and selected versions of the gates as well.
            //paper.DrawImage(Properties.Resources.AndGate, Left, Top);
        }


        /// <summary>
        /// Returns the boolean value of the input source.
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            //Doesnt even bother with recursion as its an input source
            return IsOn;
        }

        /// <summary>
        /// Clones the input source
        /// </summary>
        /// <returns></returns>
        public override Gate Clone()
        {
            InputSource Inp = new InputSource(left, top);

            Inp.MoveTo(left + 10, top + 10);

            return Inp;
        }


    }
}
