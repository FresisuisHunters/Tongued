using Photon.Pun;

public class HookInputOnline : HookInput {

    private PhotonView photonView;

    private new void Awake () {
        base.Awake ();
        photonView = GetComponent<PhotonView> ();
    }

    protected new void Update () {
        if (photonView.IsMine) {
            base.Update ();
        }
    }

}