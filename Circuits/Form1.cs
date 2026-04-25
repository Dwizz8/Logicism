using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Circuits
{
    /// <summary>
    /// The main GUI for the COMPX102 digital circuits editor.
    /// This has a toolbar, containing buttons called buttonAnd, buttonOr, etc.
    /// The contents of the circuit are drawn directly onto the form.
    /// +
    /// 
    /// Luka Jones 1672688
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// The (x,y) mouse position of the last MouseDown event.
        /// </summary>
        protected int startX, startY;

        /// <summary>
        /// If this is non-null, we are inserting a wire by
        /// dragging the mouse from startPin to some output Pin.
        /// </summary>
        protected Pin startPin = null;

        /// <summary>
        /// The (x,y) position of the current gate, just before we started dragging it.
        /// </summary>
        protected int currentX, currentY;

        /// <summary>
        /// The set of gates in the circuit
        /// </summary>
        protected List<Gate> gatesList = new List<Gate>();

        /// <summary>
        /// The set of connector wires in the circuit
        /// </summary>
        protected List<Wire> wiresList = new List<Wire>();

        /// <summary>
        /// The gates that will get removed
        /// </summary>
        protected List<Gate> gatesRemoveList = new List<Gate>();

        /// <summary>
        /// The currently selected gate, or null if no gate is selected.
        /// </summary>
        protected Gate current = null;

        /// <summary>
        /// The new gate that is about to be inserted into the circuit
        /// </summary>
        protected Gate newGate = null;

        /// <summary>
        /// The compound gate that will be copied
        /// </summary>
        protected Compound newCompound = null;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        /// <summary>
        /// Finds the pin that is close to (x,y), or returns
        /// null if there are no pins close to the position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>The pin that has been selected</returns>
        public Pin findPin(int x, int y)
        {
            foreach (Gate g in gatesList)
            {
                foreach (Pin p in g.Pins)
                {
                    if (p.isMouseOn(x, y))
                        return p;
                }
            }
            return null;
        }

        /// <summary>
        /// Handles all events when the mouse is moving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (startPin != null)
            {
                Console.WriteLine("wire from " + startPin + " to " + e.X + "," + e.Y);
                currentX = e.X;
                currentY = e.Y;
                this.Invalidate();  // this will draw the line
            }
            else if (startX >= 0 && startY >= 0 && current != null)
            {
                Console.WriteLine("mouse move to " + e.X + "," + e.Y);
                current.MoveTo(currentX + (e.X - startX), currentY + (e.Y - startY));
                this.Invalidate();
            }
            else if (newGate != null)
            {
                currentX = e.X;
                currentY = e.Y;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles all events when the mouse button is released.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (startPin != null)
            {
                // see if we can insert a wire
                Pin endPin = findPin(e.X, e.Y);
                if (endPin != null)
                {
                    Console.WriteLine("Trying to connect " + startPin + " to " + endPin);
                    Pin input, output;
                    if (startPin.IsOutput)
                    {
                        input = endPin;
                        output = startPin;
                    }
                    else
                    {
                        input = startPin;
                        output = endPin;
                    }
                    if (input.IsInput && output.IsOutput)
                    {
                        if (input.InputWire == null)
                        {
                            Wire newWire = new Wire(output, input);
                            input.InputWire = newWire;
                            wiresList.Add(newWire);
                        }
                        else
                        {
                            MessageBox.Show("That input is already used.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error: you must connect an output pin to an input pin.");
                    }
                }
                startPin = null;
                this.Invalidate();
            }
            // We have finished moving/dragging
            startX = -1;
            startY = -1;
            currentX = 0;
            currentY = 0;
        }

        /// <summary>
        /// This will create a new And gate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAnd_Click(object sender, EventArgs e)
        {
            newGate = new AndGate(0, 0);
        }

        /// <summary>
        /// Redraws all the graphics for the current circuit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Draw all of the gates
            foreach (Gate g in gatesList)
            {
                g.Draw(e.Graphics);
            }
            //Draw all of the wires
            foreach (Wire w in wiresList)
            {
                w.Draw(e.Graphics);
            }

            if (startPin != null)
            {
                e.Graphics.DrawLine(Pens.White,
                    startPin.X, startPin.Y,
                    currentX, currentY);
            }
            if (newGate != null)
            {
                // show the gate that we are dragging into the circuit
                newGate.MoveTo(currentX, currentY);
                newGate.Draw(e.Graphics);
            }
        }

        /// <summary>
        /// Creates a new Or Gate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonOr_Click(object sender, EventArgs e)
        {
            newGate = new OrGate(0, 0);
        }

        /// <summary>
        /// Creates a new Not Gate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonNot_Click(object sender, EventArgs e)
        {
            newGate = new NotGate(0, 0);
        }

        /// <summary>
        /// Creates a new Input Source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonInput_Click(object sender, EventArgs e)
        {
            newGate = new InputSource(0, 0);
        }

        /// <summary>
        /// Creates a new Output Lamp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonOutput_Click(object sender, EventArgs e)
        {
            newGate = new OutputLamp(0, 0);
        }

        /// <summary>
        /// Figures out what the circuit is actually telling it and essentially turns on a lamp or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEvaluate_Click(object sender, EventArgs e)
        {
            //Goes through all the gates in the gate list
            foreach (Gate g in gatesList)
            {
                //only evaluates if it is a lamp as it will work backwards *To be Changed*
                if (g is OutputLamp lamp)
                {
                    lamp.Evaluate();
                }
            }

            //Invalidate to constantly display the correct output
            Invalidate();
        }

        /// <summary>
        /// Clones the currently selected gate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonClone_Click(object sender, EventArgs e)
        {
            //Creates a new list for clone gates that are to be cloned if they are currently selected
            List<Gate> CloneGates = new List<Gate>();

            foreach (Gate g in gatesList)
            {
                //Adds it to the list whether its a compound or whatever
                if (g.Selected == true)
                {
                    CloneGates.Add(g.Clone());
                }
            }

            //For every gate in clone gates, adds it to the form
            foreach (Gate g in CloneGates)
            {
                gatesList.Add(g);
            }

            //Updates the form
            Invalidate();
        }

        //Creates a new compound to have gates added into it
        private void toolStripButtonStart_Click(object sender, EventArgs e)
        {
            newCompound = new Compound(this.Width, this.Height);
        }

        /// <summary>
        /// Ends the compound, grouping all the gate selected into a compound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEnd_Click(object sender, EventArgs e)
        {
            //Makes the current gate selected the newCompound
            current = newCompound;

            //Adds the newCompound to the gates list
            gatesList.Add(current);

            //Then makes the new Compound null, so essentially makes the program not think were still adding gates into a compound
            newCompound = null;

            //Goes through each gate in the removal list, essentially the ones that have been selected while our new compound object is not null
            foreach (Gate g in gatesRemoveList)
            {
                //for all the gates that they have in common, remove them from the gates list, this means that only the compound gate remains in the gate list rather than the internal gates as well
                if (gatesList.Contains(g))
                {
                    gatesList.Remove(g);
                }
            }

            //Essentially this takes the pin from the gates list of the gates that are being added into the compound and adds it into the Compound pin list,
            //but it is a copy of the pin that is originally connected to the internal gate, so it's owner is still the internal gate, allowing all the gates to work properly with evaluate.

            //Loops through and finds the compounds in the gates list
            foreach (Gate g in gatesList)
            {
                if (g is Compound d)
                {
                    //Loops through and finds the Gates in the compound
                    foreach (Gate c in d.CompoundGates)
                    {
                        //Loops through and finds all the pins that are connected to the internal Gates
                        foreach (Pin pin in c.Pins)
                        {
                            //Checks to see if the pin is connected to anything (has a wire that connects it), if it doesn't, add it into the pin list inside the compound.
                            if (pin.IsInput && pin.InputWire == null)
                            {
                                //This was an attempt to make the compound have its own internal and external pins, but it doesn't work as intended right now.
                                //Pin ExternalPin = new Pin(d, true, 20);

                                //d.InternalPins.Add(pin);
                                //d.ExternalPins.Add(ExternalPin);

                                //d.Pins.Add(ExternalPin);

                                d.Pins.Add(pin);
                            }

                            //This checks the output pins that aren't connected to anything, and adds them into the compound pin list
                            else if (pin.IsOutput && pin.InputWire == null)
                            {
                                bool isConnected = false;

                                // Check if this output pin is used by any wire in the circuit, this was the only I could find to ensure that the program takes only the unconnected output wires.
                                foreach (Wire w in wiresList)
                                {
                                    if (w.FromPin == pin)
                                    {
                                        isConnected = true;
                                        break;
                                    }
                                }

                                //Adds the pin if it isn't connected to anything
                                if (!isConnected)
                                {
                                    //This was an attempt to make the compound have its own internal and external pins, but it doesn't work as intended right now.
                                    //Pin ExternalPin = new Pin(d, false, 20);

                                    //d.InternalPins.Add(pin);
                                    //d.ExternalPins.Add(ExternalPin);

                                    //d.Pins.Add(ExternalPin);

                                    d.Pins.Add(pin);
                                }
                            }
                        }
                    }
                }
            }

            //Removes all the items in the gate remove list so I can use it later
            gatesRemoveList.Clear();

            //Refreshes and displays the changes in the compound (pins)
            this.Invalidate();
        }

        /// <summary>
        /// Handles events while the mouse button is pressed down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (current == null)
            {
                // try to start adding a wire
                startPin = findPin(e.X, e.Y);
            }
            else if (current.IsMouseOn(e.X, e.Y))
            {
                // start dragging the current object around
                startX = e.X;
                startY = e.Y;
                currentX = current.Left;
                currentY = current.Top;
            }
        }

        /// <summary>
        /// Handles all events when a mouse is clicked in the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //Check if we are adding a compound
            if (newCompound != null)
            {
                //for every gate in the gateslist
                foreach (Gate g in gatesList)
                {
                    //if a gate gets selected, also checks that the gate isn't already inside the compound.
                    if (g.Selected == true && !gatesRemoveList.Contains(g))
                    {
                        //add it to both the list inside the newcompound, and also add it to the remove list to be removed from the main gates list
                        newCompound.AddGate(g);
                        gatesRemoveList.Add(g);
                    }
                }
            }
            //Check if a gate is currently selected
            if (current != null)
            {
                //Unselect the selected gate
                current.Selected = false;
                current = null;
                this.Invalidate();
            }
            // See if we are inserting a new gate
            if (newGate != null)
            {
                newGate.MoveTo(e.X, e.Y);
                gatesList.Add(newGate);
                newGate = null;
                this.Invalidate();
            }
            else
            {
                // search for the first gate under the mouse position
                foreach (Gate g in gatesList)
                {
                    if (g.IsMouseOn(e.X, e.Y))
                    {

                        //selects the gate and makes the current gate selected
                        g.Selected = true;

                        //If an input source gate is clicked, then change the input IsOn function to either on or off (the opposite of what it was)
                        if (g is InputSource input)
                        {
                            input.IsOn = !input.IsOn;
                        }

                        current = g;


                        //Invalidates and breaks the foreach loop as there is no need to keep going as there is only one gate selected at once
                        this.Invalidate();
                        break;
                    }
                }
            }
        }




        //QUESTIONS

        //1 Is it a better idea to fully document the Gate class or the AndGate subclass? Can you inherit comments?

        //It is a better idea to fully document the Gate class as it is the superclass, and all the other subclasses inherit from it, this makes it easier so that I don't have
        //to document the same thing multiple times in each subclass, as they all share the same methods and properties from the superclass.
        //You can inherit comments using the /// <inheritdoc /> tag.

        //2 What is the advantage of making a method abstract in the superclass rather than just writing a virtual method with no code in the body of the method?
        //there any disadvantage to an abstract method?

        //The advantage of making a method abstract in the superclass is that it forces all subclasses to implement the method, making sure that all the subclasses of the 
        //superclass have the same methods, this is useful for polymorphism.

        //The disadvantage of an abstract method is that it cannot have any implementation in the superclass, so if there is some common code that all subclasses need to use
        //then we can use something like a virtual method with a default implementation that can be overridden in the subclasses if needed.\

        //3 If a class has an abstract method in it, does the class have to be abstract?

        //Yes, if a class has an abstract method in it, the class has to be abstract as well, the entire point of an abstract method is that the subclass has to implement it
        //so the superclass has to be abstract as well.

        //4 What would happen in your program if one of the gates added to your Compound Gate is another Compound Gate? Is your design robust enough to cope with this situation?

        //It would still work, as the Compound gate is a subclass of the Gate superclass, so it can be added to the Compound gate, and it would still work as expected.
        //So in short answer yes it would still work.


    }
}
