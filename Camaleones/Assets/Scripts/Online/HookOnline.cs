using Photon.Pun;

public class HookOnline : Hook {

    private PhotonView photonView;

    protected new void Awake () {
        base.Awake ();
        photonView = GetComponent<PhotonView> ();
    }

    protected new void FixedUpdate () {
        if (photonView.IsMine) {
            base.FixedUpdate ();
        }
    }

    protected new void OnDrawGizmos () {
        if (photonView.IsMine) {
            base.OnDrawGizmos ();
        }
    }
}