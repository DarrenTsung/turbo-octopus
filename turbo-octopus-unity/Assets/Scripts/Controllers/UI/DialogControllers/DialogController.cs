using UnityEngine;
using System.Collections;

public class DialogController : MonoBehaviour {
	protected string dialogToShow;
	protected int characterIndex;
	protected bool dirtyTextMesh;

	protected Timer characterAddTimer;
	protected tk2dTextMesh textMesh;
	protected GameObject textMeshObject;

	// X characters ('a', 'b', etc) every second
	public const float DIALOG_CHARACTER_ANIMATION_SPEED = 8;

	protected virtual void Start () {
		characterIndex = 0;
		dirtyTextMesh = true;
		dialogToShow = "Well I daresay - you must be quite the fighter to survived that pit of slimes.";

		textMeshObject = transform.Find ("Text").gameObject;
		textMesh = textMeshObject.GetComponent<tk2dTextMesh> ();

		characterAddTimer = TimerManager.Instance.MakeTimer();
		characterAddTimer.TimerFinished += HandleCharacterAdd;
		characterAddTimer.SetTime(1.0f / DIALOG_CHARACTER_ANIMATION_SPEED);
	}

	void StartAnimationWithDialog (string dialog) {
		dialogToShow = dialog;
		characterIndex = 0;
		dirtyTextMesh = true;
	}

	void HandleCharacterAdd() {
		if (characterIndex < dialogToShow.Length) {
			characterIndex++;
			dirtyTextMesh = true;
			characterAddTimer.SetTime(1.0f / DIALOG_CHARACTER_ANIMATION_SPEED);
		}
	}
	
	void Update () {
		if (dirtyTextMesh) {
			textMesh.text = dialogToShow.Substring(0, characterIndex);
		}
	}
}
