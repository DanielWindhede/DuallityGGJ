using UnityEngine;

public class GlobalState {
    public GlobalState() {}
}

public class GlobalStateObject : MonoBehaviour
{
    private static GlobalState _state;
    public static GlobalState state {
        get {
            if (_state == null)
                _state = new GlobalState();
            return _state;
        }
    }
}
