using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the code for performing rollbacks
/// </summary>
public class Rollback
{
    #region Statics&Constants
    /// <summary>
    /// The current time/frame the local player is at
    /// </summary>
    protected static float CurrentTime => Time.time - GameManager.s_instance.m_startTime;
    /// <summary>
    /// The maximum time between an input and current time an input is considered new.
    /// Inputs with a time dfference beyond this are cleaned up.
    /// </summary>
    protected const float MaxInputDif = 1;
    /// <summary>
    /// The length of a timestep when simulating a rollback
    /// </summary>
    protected const float StepSize = 0.01f;
    #endregion
    #region Variables
    /// <summary>
    /// Struct for storing the inputs made for a period of time
    /// </summary>
    internal struct Input
    {   
        public Input(float t, sbyte moveInput, bool didJump, bool didDash)
        {
            time = t;
            leftRight = moveInput;
            jump = didJump;
            dash = didDash;
            position = Vector3.zero;
            velocity = Vector3.zero;
        }
        /// <summary>
        /// The time the input was made
        /// </summary>
        public float time;
        //Character input data
        #region InputData
        /// <summary>
        /// The leftRight input
        /// </summary>
        public sbyte leftRight;
        /// <summary>
        /// The jump input
        /// </summary>
        public bool jump;
        /// <summary>
        /// The dash input
        /// </summary>
        public bool dash;
        #endregion
        //Data about how they are at the time of the input
        #region How/Where
        /// <summary>
        /// The position of the character when the input was made
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// The velocity of the character when the input was made
        /// </summary>
        public Vector3 velocity;
        #endregion
    }
    /// <summary>
    /// The target character for the rollback
    /// </summary>
    protected Player _target = null;
    /// <summary>
    /// The inputs the player has made along side their time stamps
    /// </summary>
    private List<Input> _inputs = null;
    #endregion

    public Rollback(Player target)
    {
        _target = target;
        //Instance the list and setup the first frame to have no inputs
        _inputs = new List<Input>();
        Input start = new Input(0, 0, false, false)
        {
            position = target.transform.position
        };
        _inputs.Add(start);
    }
    /// <summary>
    /// Set the time when a jump input was made.
    /// </summary>
    /// <param name="time">The time the jump input was made</param>
    public void SetJump(float time) 
    {   //Copy the last input that was made
        Input newInput = _inputs[_inputs.Count - 1];
        //Fill in the updated data
        newInput.time = time;
        newInput.jump = true;
        //Add the new input to the rollback queue
        _inputs.Insert(GetIndexForInput(time), newInput);
        //Rollback to when the input was made
        RollbackTime(CurrentTime - time);
    }
    /// <summary>
    /// Set the time when a dash input was made
    /// </summary>
    /// <param name="time">The time the dash input was made</param>
    public void SetDash(float time)
    {   //Copy the last input that was made
        Input newInput = _inputs[_inputs.Count - 1];
        //Fill in the updated data
        newInput.time = time;
        newInput.dash = true;
        //Add the new input to the rollback queue
        _inputs.Insert(GetIndexForInput(time), newInput);
        //Rollback to when the input was made
        RollbackTime(CurrentTime - time);
    }
    /// <summary>
    /// Set the time a move input was made
    /// </summary>
    /// <param name="time">The time the input was made</param>
    /// <param name="input">The input that was made</param>
    public void SetMove(float time, sbyte input)
    {   //Create a new input
        Input newInput = new Input(time, input, false, false);
        //Add the new input to the rollback queue
        _inputs.Insert(GetIndexForInput(time), newInput);
        //Rollback to when the input was made
        RollbackTime(CurrentTime - time);
    }
    /// <summary>
    /// Rolls back a fixed amount of time
    /// </summary>
    /// <param name="delta">The time to rollback</param>
    private void RollbackTime(float delta) 
    {   //Calculate when the rollback started
        float start = CurrentTime - delta;

        if (start > MaxInputDif)
            Debug.LogError("Cannot rollback. The rollback is to far. The input is too old. There is too much lag");

        //Simulate back to present time
        Simulate(delta);
    }
    /// <summary>
    /// Simulates the character for a fixed amount of time
    /// </summary>
    /// <param name="delta">The time to simulate for</param>
    private void Simulate(float delta) { }
    /// <summary>
    /// Cleans up the inputs, merging duplicate time stamps and erasing old inputs, storing the simulation data
    /// </summary>
    private void CleanInputs()
    {   //Allocate memory for the coming for loops
        short i;
        short old = 0;
        //Check for duplicate time stamps
        for (i = 0; i < _inputs.Count; i++)
        {   //Check for old inputs
            if (CurrentTime - _inputs[i].time > MaxInputDif)
                old++;
            //Indexer validation
            if (i + 1 >= _inputs.Count)
                break;
            //Compare the current & next input timestamps
            if (_inputs[i].time == _inputs[i + 1].time)
            {   //We have duplicate time stamps so merge them
                Input temp = _inputs[i];
                //The merge the trigger data
                temp.jump = _inputs[i].jump || _inputs[i + 1].jump;
                temp.dash = _inputs[i].dash || _inputs[i + 1].dash;
                //Currently there is only 1 way a double time stamp can be hit
                //If a player jumps and changes their move input simultaniously.
                //Because of how jump inputs are logged, the latest input will always
                //have the correct moveInput/leftRight value.
                temp.leftRight = _inputs[i + 1].leftRight;
                //Store it back
                _inputs[i] = temp;
                //Remove the second input and decrement i to avoid skipping a comparison
                _inputs.RemoveAt(i + 1);
                i--;
            }
        }
        //No old inputs require cleanup
        if (old <= 1)
            return;
        //Get time to simulate for
        float delta;
        float step;
        float t = 0;
        //We leave 1 old input to act as a reference point for the initial conditions.
        //Now we simulate up until that last old input and store the position and velocity of the player
        for (i = 0; i < old; i++)
        {
            //Check for dashing
                //If its a ground dash, increase speed
                //If its an air dash, the player must wait for the dash to finish


            //Check for move input change
            //Update move input
            delta = _inputs[i].time - _inputs[i + 1].time;
            //Apply movement at a fixed rate. (Avoid overshooting into next input)
            //Use an internal fixed update while loop or similar to simulate movement.
            while (t < delta)
            {   //Clamp the step to not overshoot into the next input
                if (delta - t < StepSize)
                    step = delta - t;
                else
                    step = StepSize;

                //Increase the time
                t += step;
            }
        }
    }
    /// <summary>
    /// Finds the index an input should be placed at in _inputs to keep things sorted by time
    /// </summary>
    /// <param name="time">The time the input should be sorted into</param>
    /// <returns>Returns the index to insert at. If none is found, returns the lenth of input</returns>
    private int GetIndexForInput(float time)
    {
        for (int i = 0; i < _inputs.Count; i++)
            //If input time is larger, then we need to insert at its location, putting this value in front of us
            if (_inputs[i].time > time)
                return i;
        //If no slot could be found, place at the end of _inputs
        return _inputs.Count;
    }
}
