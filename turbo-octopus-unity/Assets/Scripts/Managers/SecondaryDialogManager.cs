using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecondaryDialogManager : Singleton<SecondaryDialogManager> {

	List<GameObject> dialogs;
	List<GameObject> dialogsToAdd;

	private const float DIALOG_PADDING = 0.1f;

	public void SpawnDialog(string text) {
		GameObject dialog = Instantiate(PrefabManager.Instance.SecondaryDialog) as GameObject;
		dialog.transform.parent = UIManager.Instance.BottomLeftAnchor.transform;
		dialog.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		DialogController dialogController = dialog.GetComponent<DialogController> ();
		dialogController.StartAnimationWithDialog(text);

		dialogsToAdd.Add (dialog);
	}

	protected void Awake() {
		dialogs = new List<GameObject>();
		dialogsToAdd = new List<GameObject>();
	}

	protected void Update() {
		dialogs.AddRange(dialogsToAdd);
		dialogsToAdd.Clear();

		float currentY = 0;
		int dialogIndex = 0;
		for (int i=dialogs.Count - 1; i >= 0; i--) {
			// remove dead objects
			if (!dialogs[i]) {
				dialogs.RemoveAt(i);
				continue;
			}

			Vector3 oldPosition = dialogs[i].transform.localPosition;
			dialogs[i].transform.localPosition = new Vector3(oldPosition.x, currentY, oldPosition.z);

			SecondaryDialogController secondaryDialogController = dialogs[i].GetComponent<SecondaryDialogController> ();
			secondaryDialogController.SetIndex(dialogIndex);
			currentY += secondaryDialogController.ScaledHeight();
			currentY += DIALOG_PADDING;
			dialogIndex++;
		}
	}
}
