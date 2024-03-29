﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
/// <summary>
/// Controls Rollback
/// </summary>
public static class RollbackMaster
{
    /// <summary>
    /// The time to simulate each step for
    /// </summary>
    private const float TIME_STEP = 0.01f;
    /// <summary>
    /// Stores information about all objects, etc
    /// </summary>
    private static readonly Dictionary<int, PlayerStateInfo> _playerTimeInfo = new Dictionary<int, PlayerStateInfo>();
    /// <summary>
    /// Clears any stored rollback data
    /// </summary>
    public static void Clear()
    {
        _playerTimeInfo.Clear();
    }

    private class PlayerStateInfo
    {
        /// <summary>
        /// The components that have rollback applied
        /// </summary>
        public IPlayerRollback[] components = null;
        /// <summary>
        /// Information about important state changes
        /// </summary>
        public List<TimeFrame> timeFrames = new List<TimeFrame>();

        public PlayerStateInfo(GameObject target)
        {   //Get all components that use rollback
            var behaviours = target.GetComponentsInChildren<MonoBehaviour>(true);
            List<IPlayerRollback> rollbacks = new List<IPlayerRollback>();

            foreach (var behavoiur in behaviours)
                if (behavoiur is IPlayerRollback roll)
                {
                    rollbacks.Add(roll);
                }

            components = rollbacks.ToArray();
            //Null it just in case we want to do more after this in future.
            rollbacks = null;
        }

        public void CreatePlayerState(float time)
        {   //Store the players state at this time frame
            TimeFrame t = new TimeFrame();
            t.time = time;

            foreach (var comp in components)
                t.timeInfo.Add(comp, comp.CreateState());

            int i;
            for (i = 0; i < timeFrames.Count; i++)
                //Found time
                if (timeFrames[i].time > time)
                    break;
            //Store the time frames in order of time.
            timeFrames.Insert(i, t);
        }

        public IReadOnlyList<TimeFrame> GetTimeFrames()
        {
            return timeFrames.AsReadOnly();
        }
    }

    public class TimeFrame
    {
        /// <summary>
        /// The time this behaviour exists for
        /// </summary>
        public float time = 0;
        /// <summary>
        /// The component & its state
        /// </summary>
        public Dictionary<IPlayerRollback, BehaviourState> timeInfo = new Dictionary<IPlayerRollback, BehaviourState>();
        /// <summary>
        /// Rolls the player back to this time frame
        /// </summary>
        public void Apply()
        {
            foreach (var keyValue in timeInfo)
                keyValue.Key.SetState(keyValue.Value);
        }
        /// <summary>
        /// Simulate this time frame by delta time
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Simulate(float deltaTime)
        {
            foreach (var keys in timeInfo.Keys)
                keys.Simulate(deltaTime);
        }
        /// <summary>
        /// Causes the timeFrame to refresh its state
        /// </summary>
        public void UpdateSelf()
        {   //Refresh the players state
            foreach (var keyValue in timeInfo)
                keyValue.Key.RefreshState(keyValue.Value);
        }
    }
    /// <summary>
    /// Start tracking an object
    /// </summary>
    /// <param name="id"></param>
    /// <param name="toTrack"></param>
    public static void TrackObject(int id, GameObject toTrack, bool createTimeStamp)
    {   //Start tracking the gameObject
        var info = new PlayerStateInfo(toTrack);
        _playerTimeInfo.Add(id, info);

        if (createTimeStamp)
        {
            float time = GameManager.GameTime;
            info.CreatePlayerState(time);
        }
    }
    /// <summary>
    /// Applies the rollback logic to all characters
    /// </summary>
    /// <param name="time">The time to create the new time stamp</param>
    /// <param name="applyChanges">Applies the changes. Will be invoked once simulated time == time.</param>
    public static void ApplyRollback(float time, Action applyChanges)
    {
        IReadOnlyList<TimeFrame>[] times = new IReadOnlyList<TimeFrame>[_playerTimeInfo.Count];
        //Gather the times
        int start = 0;
        foreach (var value in _playerTimeInfo.Values)
        {
            times[start] = value.GetTimeFrames();
            start++;
            //Prepare for simulation
            foreach (var comp in value.components)
                comp.SimulateStart();
        }

        try
        {
            start = 0;
            //We are going to use temp to calculate the time steps.
            //All TimeFrame arrays should always have equal length with equal timestamps
            var temp = times[0];
            //Find starting TimeFrame index
            //There should always be a frame before this one to start the rollback from.
            int maxStart = temp.Count - 1;

            if (maxStart < 0)
            {   //There are no previous rollbacks so just create a state now.
                foreach (var value in _playerTimeInfo.Values)
                    value.CreatePlayerState(time);

                End();
                return;
            }

            while (start < maxStart && temp[start].time < time)
                start++;
            //Calculate the total time to simulate
            float deltaTotal = GameManager.GameTime - temp[start].time;

            //Prepare
            foreach (var t in times)
                t[start].Apply();

            float tempDelta = time - temp[start].time;

            Debug.LogError("Input Time: " + time + " Last Input Time: " + temp[start].time);
            Debug.LogError("Rollback Start: " + times[0][start].time + "\nRollback Amount: " + deltaTotal + "\nTo Current: " + tempDelta);
            float total = 0f;
            //Reduce the total time to rollback by, by the amount we are about to as this float value will be 0 after the while loop
            deltaTotal -= tempDelta;
            //Apply rollback until we reach where we should create the new start time
            while (tempDelta > 0f)
            {
                float dif;
                if (tempDelta > TIME_STEP)
                    dif = TIME_STEP;
                else
                    dif = tempDelta;
                //Simulate
                foreach (var t in times)
                    t[start].Simulate(dif);

                total += dif;
                tempDelta -= dif;
            }

            start++;
            Debug.LogError("Time To Input: " + total + "\n Remaining: " + deltaTotal);
            //Apply the changes
            applyChanges?.Invoke();
            //We have reached the time we want to create the rollback for
            foreach (var value in _playerTimeInfo.Values)
                value.CreatePlayerState(time);

            //Continue simulation up until current time
            while (deltaTotal > 0.0001f)
            {
                bool hasNext = start < temp.Count - 1;
                float delta = hasNext ? temp[start + 1].time - temp[start].time : deltaTotal;
                Debug.LogError("Forward Rollback: " + deltaTotal);
                //Set to state. Nothing should change since we called UpdateSelf previously
                //This should only update the inputs to reflect the change
                foreach (var t in times)
                    t[start].Apply();

                total = 0f;
                //Apply rollback over this period of time
                while (delta > 0f)
                {   //Calculate time step
                    float dif;
                    if (delta > TIME_STEP)
                        dif = TIME_STEP;
                    else
                        dif = delta;

                    //Simulate
                    foreach (var t in times)
                        t[start].Simulate(dif);

                    total += dif;
                    delta -= dif;
                    deltaTotal -= dif;
                }
                Debug.LogError("Delta Applied: " + total);

                start++;
                //Refresh timeframe data's as the previous change may have changed and we don't want rolling back
                //specifically to this time to be different than if we simulated from the previous timeFrame
                if (start < temp.Count)
                    foreach (var t in times)
                        t[start].UpdateSelf();
            }
        }
        catch (Exception e)
        { Debug.LogException(e); }//If something goes wrong, make sure to end the simulation otherwise the player will remain frozen forever

        End();
        //Calls SimulateEnd on every player
        void End()
        {
            try
            {
                //Simulate has finished.
                foreach (var value in _playerTimeInfo.Values)
                    foreach (var comp in value.components)
                        comp.SimulateEnd();
            }
            catch (Exception e)
            { Debug.LogException(e); }
        }
    }
}

/// <summary>
/// Stores information about the current state of the player
/// </summary>
public abstract class BehaviourState
{
}
/// <summary>
/// Interface for player rollback
/// </summary>
public interface IPlayerRollback
{
    /// <summary>
    /// Set the player to a given state
    /// </summary>
    /// <param name="state">The state to set the player to</param>
    void SetState(BehaviourState state);
    /// <summary>
    /// Simulate the player given their current state for delta amount of time
    /// </summary>
    /// <param name="delta">The amount of time to simulate for</param>
    void Simulate(float delta);
    /// <summary>
    /// Creates a state for the player
    /// </summary>
    /// <returns>The players current state</returns>
    BehaviourState CreateState();
    /// <summary>
    /// Update important rollback data to reflect the current state. Don't update inputs or input related data.
    /// </summary>
    /// <param name="state"></param>
    /// <remarks>This should update the given behaviour state such that, calling SetState(state) results in the same data as 
    /// calling SetState(previous) then Simulate(delta) to the time this behaviourState represents</remarks>
    void RefreshState(BehaviourState state);
    /// <summary>
    /// Called before starting simulation
    /// </summary>
    void SimulateStart();
    /// <summary>
    /// Called after simulation
    /// </summary>
    void SimulateEnd();
}
