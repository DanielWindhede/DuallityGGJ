using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T> {

    protected T owner;

    public State() {
    }

    public State(T owner) {
        this.owner = owner;
    }

    virtual public void Enter() {

        //
    }

    virtual public void Execute() {
    }

    virtual public void FixedExecute() {
    }

    virtual public void Exit() {
    }

}
