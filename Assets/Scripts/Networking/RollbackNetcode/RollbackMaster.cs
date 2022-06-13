using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
/// <summary>
/// Controls Rollback
/// </summary>
public class RollbackMaster : MonoBehaviour
{
    private const float TIME_STEP = 0.1f;

    private Dictionary<int, PlayerStateInfo> playerTimeInfo = new Dictionary<int, PlayerStateInfo>();
    /// <summary>
    /// Clears any stored rollback data
    /// </summary>
    public void Clear()
    {
        playerTimeInfo.Clear();
    }

    private class PlayerStateInfo
    {   
        /// <summary>
        /// The components that have rollback applied
        /// </summary>
        public PlayerRollback[] components = null;
        /// <summary>
        /// Information about important state changes
        /// </summary>
        public List<TimeFrame> timeFrames = new List<TimeFrame>();

        public PlayerStateInfo(GameObject target)
        {   //Get all components that use rollback
            var behaviours = target.GetComponentsInChildren<MonoBehaviour>(true);
            List<PlayerRollback> rollbacks = new List<PlayerRollback>();

            foreach(var behavoiur in behaviours)
                if (behavoiur is PlayerRollback roll)
                {
                    rollbacks.Add(roll);
                }

            components = rollbacks.ToArray();
            //Null it just in case we want to do more after this in future.
            rollbacks = null;
        }

        public void CreatePlayerState()
        {   //Store the players state at this time frame
            TimeFrame t = new TimeFrame();
            t.time = Time.time - GameManager.s_instance.m_startTime;

            foreach (var comp in components)
                t.timeInfo.Add(comp, comp.CreateState());
        }
    }

    private class TimeFrame
    {
        /// <summary>
        /// The time this behaviour exists for
        /// </summary>
        public float time = 0;
        /// <summary>
        /// The component & its state
        /// </summary>
        public Dictionary<PlayerRollback, BehaviourState> timeInfo = new Dictionary<PlayerRollback, BehaviourState>();
    }
}

/// <summary>
/// Stores information about the current state of the player
/// </summary>
public class BehaviourState
{
}
/// <summary>
/// Interface for player rollback
/// </summary>
public interface PlayerRollback
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
}
