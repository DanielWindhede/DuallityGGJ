using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> {

    private State<T> currentlyRunningState;
    private State<T> previousState;


    public State<T> CurrentlyRunningState {
        get {
            return currentlyRunningState;
        }
    }

    public void ChangeState(State<T> newState) {

        if (this.currentlyRunningState != null)
            this.currentlyRunningState.Exit();

        this.previousState = this.currentlyRunningState;

        this.currentlyRunningState = newState;

        this.currentlyRunningState.Enter();
    }

    public void Update() {
        if (currentlyRunningState != null)
            currentlyRunningState.Execute();
    }

    public void FixedUpdate() {
        if (currentlyRunningState != null)
            currentlyRunningState.FixedExecute();
    }

    public void SwitchToPreviousState() {
        if (this.currentlyRunningState != null)
            this.currentlyRunningState.Exit();

        this.currentlyRunningState = previousState;
        this.currentlyRunningState.Enter();
    }
}
