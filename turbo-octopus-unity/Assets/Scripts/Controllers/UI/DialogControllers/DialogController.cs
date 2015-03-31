using UnityEngine;
using System.Collections;

public enum DialogState {
	Opening,
	Typing,
	FinishedTyping,
	Closing
};

public class DialogController : MonoBehaviour {
	protected string dialogToShow;
	protected int characterIndex;
	protected bool dirtyTextMesh;

	protected Timer characterAddTimer, closeDialogTimer;
	protected tk2dTextMesh textMesh;
	protected GameObject textMeshObject;

	protected Animator animator;

	DialogState currentState;

	// X characters ('a', 'b', etc) every second
	public const float DIALOG_CHARACTER_ANIMATION_SPEED = 20;
	public const float DIALOG_CLOSE_DELAY = 0.5f;

	protected virtual void Start () {
		currentState = DialogState.Opening;
		characterIndex = 0;
		dirtyTextMesh = true;
		dialogToShow = "Well I daresay - you must be quite the fighter.";

		textMeshObject = transform.Find ("Text").gameObject;
		textMesh = textMeshObject.GetComponent<tk2dTextMesh> ();

		animator = GetComponent<Animator> ();
	}

	void StartAnimationWithDialog (string dialog) {
		dialogToShow = dialog;
		characterIndex = 0;
		dirtyTextMesh = true;
	}

	protected virtual void HandleCharacterAdd() {
		if (characterIndex < dialogToShow.Length) {
			characterIndex++;
			dirtyTextMesh = true;
			characterAddTimer.SetTime(1.0f / DIALOG_CHARACTER_ANIMATION_SPEED);
		} else {
			if (currentState == DialogState.Typing) {
				currentState = DialogState.FinishedTyping;
				HandleDialogFinish();
			}
		}
	}

	protected virtual void StartClosingDialog() {
		currentState = DialogState.Closing;
		animator.SetBool("Closing", true);
	}
	
	protected virtual void Update () {
		if (dirtyTextMesh) {
			textMesh.text = dialogToShow.Substring(0, characterIndex);
		}
	}

	public virtual void HandleDialogFinish() {
		closeDialogTimer = TimerManager.Instance.MakeTimer();
		closeDialogTimer.TimerFinished += StartClosingDialog;
		closeDialogTimer.SetTime(DIALOG_CLOSE_DELAY);
	}

	public virtual void HandleDialogClose() {
		Destroy (gameObject);
	}

	public virtual void HandleDialogOpen() {
		currentState = DialogState.Typing;
		characterAddTimer = TimerManager.Instance.MakeTimer();
		characterAddTimer.TimerFinished += HandleCharacterAdd;
		characterAddTimer.SetTime(1.0f / DIALOG_CHARACTER_ANIMATION_SPEED);
	}
}
