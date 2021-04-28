using UnityEngine;
using UnityEngine.EventSystems;


//overide standard input module, allow for interaction with world space UI
//quickly switch the cursor back on, complete the procees, then disable curson again
public class CustomInputModule : StandaloneInputModule
{
	protected override MouseState GetMousePointerEventData(int id)
	{
		Cursor.lockState = CursorLockMode.None;
		var mouseState = base.GetMousePointerEventData(id);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		return mouseState;
	}

	protected override void ProcessDrag(PointerEventData pointerEvent)
	{
		Cursor.lockState = CursorLockMode.None;
		base.ProcessDrag(pointerEvent);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	protected override void ProcessMove(PointerEventData pointerEvent)
	{
		Cursor.lockState = CursorLockMode.None;
		base.ProcessMove(pointerEvent);
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	
}