using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecondaryDialogManager : Singleton<SecondaryDialogManager> {

	List<GameObject> dialogs;
	List<GameObject> dialogsToAdd;

	protected const float MAX_DIALOGS = 2.0f;
	protected const float DIALOG_SPACING = 0.1f;

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

	protected void LateUpdate() {
		dialogs.AddRange(dialogsToAdd);
		dialogsToAdd.Clear();

		float currentY = 0;
		float dialogIndex = 0;
		for (int i=dialogs.Count - 1; i >= 0; i--) {
			// remove dead objects
			if (!dialogs[i]) {
				dialogs.RemoveAt(i);
				continue;
			}

			SecondaryDialogController secondaryDialogController = dialogs[i].GetComponent<SecondaryDialogController> ();

			Vector3 oldPosition = dialogs[i].transform.localPosition;
			dialogs[i].transform.localPosition = new Vector3(oldPosition.x, currentY, oldPosition.z);

			secondaryDialogController.SetIndex(dialogIndex);

			// scale dialogs so that any dialogs that are above 2 dialogs in height get squished down into nothing
			Vector3 oldScaling = dialogs[i].transform.lossyScale;
			float percentHeight = secondaryDialogController.PercentHeight();
			if (dialogIndex + percentHeight > MAX_DIALOGS) {
				// round off the percent height so that it can only fit into 2 dialogs height
				percentHeight -= dialogIndex + percentHeight - MAX_DIALOGS;
				dialogs[i].transform.localScale = new Vector3(oldScaling.x, percentHeight, oldScaling.z);
				if (percentHeight <= 0) {
					secondaryDialogController.HandleDialogClose();
					continue;
				} else if (percentHeight <= 1.0f) {
					secondaryDialogController.StartClosingDialogIfNecessary();
				}
			}

			// increment the position pointer and scaling pointer
			dialogIndex += secondaryDialogController.PercentHeight();
			currentY += secondaryDialogController.ScaledHeight() + DIALOG_SPACING;
		}
	}
}
