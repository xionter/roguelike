using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private IdleEvent idleEvent;

    private void Awake() 
    { 
        rigidBody2D = GetComponent<Rigidbody2D>(); 
        idleEvent = GetComponent<IdleEvent>(); 
    } 
    private void OnEnable() 
    { 
        // добавить подписчика на ивент 
        idleEvent.OnIdle += IdleEvent_OnIdle; 
    } 
    private void OnDisable() 
    { 
        // снять подписчика с ивента 
        idleEvent.OnIdle -= IdleEvent_OnIdle; 
    } 
    private void IdleEvent_OnIdle (IdleEvent idleEvent) 
    { 
        MoveRigidBody(); 
    } 

    private void MoveRigidBody() 
    { 
        //убедиться, что рб детекция коллизий установлена на continous
        rigidBody2D.linearVelocity = Vector2.zero; 
    }
}
