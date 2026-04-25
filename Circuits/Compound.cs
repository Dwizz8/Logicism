using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuits
{
    public class Compound : Gate
    {
        //Stores the list of gates inside the compound in this list
        private List<Gate> _compoundGates = new List<Gate>();

        //Creates two lists for the internally connected pins that get linked to the externally connected pins in the evaluate method.
        //private List<Pin> _externalPins = new List<Pin>();
        //private List<Pin> _internalPins = new List<Pin>();


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Compound(int x, int y) : base(x, y)
        {
            
        }

        /// <summary>
        /// Makes the list a read property to be accessed from the form
        /// </summary>
        public List<Gate> CompoundGates
        {
            get { return _compoundGates; }
        }

        ///This was an attempt at making the pins easier to manage by giving the compound ownership of them, but I found it was easier 
        ///to just manage them through the gates themselves.
        //public List<Pin> ExternalPins 
        //{ 
        //    get { return _externalPins; } 
        //}

        //public List<Pin> InternalPins
        //{
        //    get { return _internalPins; }
        //}

        /// <summary>
        /// Adds a gate to the compound gate list
        /// </summary>
        /// <param name="gate"></param>
        public void AddGate(Gate gate)
        {
            //adds the gate to the compound gate list
            _compoundGates.Add(gate);

            //This ensures there is a top left corner of the compound and moves it properly
            if (gate.Left < Left)
            {
                left = gate.Left;
            }

            if (gate.Top < Top)
            {
                top = gate.Top;
            }
        }

        /// <summary>
        /// Move's the compound as a whole
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void MoveTo(int x, int y)
        {
            //Moves the compound gates and pins the correct distance
            foreach (Gate gate in _compoundGates)
            {
                int xDistance = x - left;
                int yDistance = y - top;

                gate.MoveTo(gate.Left + xDistance, gate.Top + yDistance);
            }

            foreach (Pin p in Pins)
            {
                int xDistance = x - left;
                int yDistance = y - top;

                p.X = p.X + xDistance;
                p.Y = p.Y + yDistance;
            }

            //Resets the left and top values to the coordinates there at now
            left = x;
            top = y;
        }

        /// <summary>
        /// Draws the compound, including the gates and the pins.
        /// </summary>
        /// <param name="paper"></param>
        public override void Draw(Graphics paper)
        {   
            //goes through the list and essentially just temporarily makes it selected for it to be drawn in a correct color
            foreach (Gate gate in _compoundGates)
            {
                if (selected)
                {
                    gate.Selected = true;
                    gate.Draw(paper);
                    gate.Selected = false;
                }
                else
                {
                    gate.Draw(paper);
                }
            }

            //Moves the external compound pin locations to the correct spot in the compound.
            //for (int i = 0; i < ExternalPins.Count; i++)
            //{
            //    ExternalPins[i].X = InternalPins[i].X;
            //    ExternalPins[i].Y = InternalPins[i].Y;
            //}

            //Don't actually need to redraw the pins, As they are already externally associated with the compound and internally with the gates in the compound.
            //This was more for a visual check to ensure the code was working as it should
            //foreach (Pin p in Pins)
            //{
            //    p.Draw(paper);
            //}
        }

        /// <summary>
        /// The evaluate method to ensure that all the "circuitry" part of the circuits is working.
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            //I never call evaluate on the compound itself, only on the gates inside it.
            return false;
        }

        /// <summary>
        /// Clones the current compound gate, excluding the wires
        /// </summary>
        /// <returns></returns>
        public override Gate Clone()
        { 
            //Creates the Cloned Gate
            Compound CompClone = new Compound(left, top);

            foreach (Gate c in _compoundGates)
            {
                //Adds all the gates inside the compound to the cloned compound
                Gate Clonegate = c.Clone();
                CompClone.AddGate(Clonegate);

                //Loops through all the pins in the clone gates, and adds them to the clone compound so as the evaluate method works properly.
                foreach (Pin p in Clonegate.Pins)
                {
                    CompClone.Pins.Add(p);
                }
            }

            //Offsets the clone slightly so the user can see both compounds
            CompClone.MoveTo(left, top);

            return CompClone;
        }
    }
}
