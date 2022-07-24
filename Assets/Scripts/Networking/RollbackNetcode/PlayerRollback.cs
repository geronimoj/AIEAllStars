using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// Simulates rollback on a player
/// </summary>
public abstract class PlayerRollback<T> : MonoBehaviour where T : BehaviourState
{
}
/// <summary>
/// Stores all the general information that everyone cares about
/// </summary>
public sealed class CoreRollbackInfo : BehaviourState
{
    /// <summary>
    /// The players current health
    /// </summary>
    public readonly float m_health;
    /// <summary>
    /// Players move vector 
    /// </summary>
    public readonly Vector3 m_velocity;
    /// <summary>
    /// Players world position
    /// </summary>
    public readonly Vector3 m_worldPosition;
    /// <summary>
    /// Players rotation
    /// </summary>
    public readonly Quaternion m_rotation;
    /// <summary>
    /// Remaining number of air charges
    /// </summary>
    public readonly byte m_airCharges;
    /// <summary>
    /// Is the player dashing
    /// </summary>
    public bool IsDashing => m_remainingDashTime > 0;
    /// <summary>
    /// The remaining time for a dash
    /// </summary>
    public readonly float m_remainingDashTime;
    /// <summary>
    /// Does the player have IFrames
    /// </summary>
    public bool HasIFrames => m_remainingIFrames > 0;
    /// <summary>
    /// The remaining IFrames
    /// </summary>
    public readonly float m_remainingIFrames;
    /// <summary>
    /// The amount of stun the player has
    /// </summary>
    public readonly byte m_stunned;
    /// <summary>
    /// The movement input by the player
    /// </summary>
    public readonly sbyte m_moveInput;
    /// <summary>
    /// Is the player pressing the jump input
    /// </summary>
    public readonly bool m_jumping;

    public CoreRollbackInfo(float health, Vector3 velocity, Vector3 pos, Quaternion rotation, byte airActionsRemaining, float remainingDashDuration, float remainingIFrames, byte stunAmount, sbyte moveInput, bool jumping)
    {
        m_health = health;
        m_velocity = velocity;
        m_worldPosition = pos;
        m_rotation = rotation;
        m_airCharges = airActionsRemaining;
        m_remainingDashTime = remainingDashDuration;
        m_remainingIFrames = remainingIFrames;
        m_stunned = stunAmount;
        m_moveInput = moveInput;
        m_jumping = jumping;
    }
}
/// <summary>
/// Information about the players current attack state
/// </summary>
public sealed class AttackRollbackInfo : BehaviourState
{
    /// <summary>
    /// Is the player currently attacking
    /// </summary>
    public readonly bool m_attacking;
    /// <summary>
    /// The progress through the animation (0 - 1)
    /// </summary>
    public readonly float m_attackProgress;
    /// <summary>
    /// The attack that is being performed (0, 1 or 2) for standard (3+) for special
    /// </summary>
    public readonly byte m_attack;

    public AttackRollbackInfo(bool attacking, float attackProgress, byte attack)
    {
        m_attacking = attacking;
        m_attackProgress = attackProgress;
        m_attack = attack;
    }
}
/// <summary>
/// Stores information about a projectile
/// </summary>
public sealed class ProjectileRollbackInfo : BehaviourState
{
    /// <summary>
    /// Information about the projectiles current state
    /// </summary>
    public readonly ProjectileState m_state;
    /// <summary>
    /// Players move vector 
    /// </summary>
    public readonly Vector3 m_velocity;
    /// <summary>
    /// Players world position
    /// </summary>
    public readonly Vector3 m_worldPosition;
    /// <summary>
    /// Players rotation
    /// </summary>
    public readonly Quaternion m_rotation;
    /// <summary>
    /// How long the projectile has to live
    /// </summary>
    public readonly float m_remainingLife;

    public ProjectileRollbackInfo(ProjectileState state, Vector3 velocity, Vector3 pos, Quaternion rotation, float remainingLifeTime)
    {
        m_state = state;
        m_velocity = velocity;
        m_worldPosition = pos;
        m_rotation = rotation;
        m_remainingLife = remainingLifeTime;
    }
}
/// <summary>
/// The state of a projectile
/// </summary>
public enum ProjectileState
{
    Spawned = 0,
    Live = 1,
    Dead = 2
}
/// <summary>
/// Stores information about a players input
/// </summary>
public struct InputInfo
{
    /// <summary>
    /// Don't read directly!
    /// </summary>
    private byte moveInput;
    /// <summary>
    /// The players current move input. Should always be range (-1, 1)
    /// </summary>
    public int MoveInput
    {
        get => (int)moveInput - 1;
        set => moveInput = (byte)(value + 1);
    }
    /// <summary>
    /// Did the player press jump at this frame
    /// </summary>
    public bool jump;
    /// <summary>
    /// Did the player press dash at this frame
    /// </summary>
    public bool dash;
    /// <summary>
    /// Did the player press skill at this frame
    /// </summary>
    public bool skill;
    /// <summary>
    /// Did the player press attack at this frame
    /// </summary>
    public bool attack;
    /// <summary>
    /// Converts the InputInfo to a byte for easy networking
    /// </summary>
    /// <returns></returns>
    public byte ToByte()
    {
        byte ret = moveInput;

        byte j = (byte)(jump ? 0b_0001_0000 : 0b_0000);
        byte d = (byte)(dash ? 0b_0010_0000 : 0b_0000);
        byte s = (byte)(skill ? 0b_0100_0000 : 0b_0000);
        byte a = (byte)(attack ? 0b_1000_0000 : 0b_0000);
        //Squash them all together
        ret |= j |= d |= s |= a;

        return ret;
    }
    /// <summary>
    /// Converts a byte to an InputInfo struct. Primarily for serialization
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static InputInfo FromByte(byte data)
    {
        InputInfo ret;
        //Get Attack
        ret.attack = (byte)(data & 0b_1000_0000) > 0;
        //Skill
        ret.skill = (byte)(data & 0b_0100_0000) > 0;
        //Dash
        ret.dash = (byte)(data & 0b_0010_0000) > 0;
        //Jump
        ret.jump = (byte)(data & 0b_0001_0000) > 0;
        //Move input        This will flush the first 3 bits of the byte
        ret.moveInput = (byte)(data ^ 0b_1111_0000);

        return ret;
    }
}