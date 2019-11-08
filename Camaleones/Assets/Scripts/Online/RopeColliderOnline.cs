using Photon.Pun;

public class RopeColliderOnline : RopeCollider {

    private PhotonView photonView;

    protected void Awake () {
        photonView = GetComponent<PhotonView> ();
    }

    protected new void FixedUpdate () {
        if (photonView.IsMine) {
            base.FixedUpdate ();
        }
    }

}