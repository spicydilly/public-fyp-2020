namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    ///  
    /// This is an edit of the original <see cref="XRSimpleInteractable"/> script, this edit is intended to be attached to all 
    /// interactive objects on the map, for much more efficient roll out. This way I do not need to manually edit the component
    /// for each interactive point that is placed on the map
    /// 
    /// </summary>
    public class XRMapInteractable : XRBaseInteractable
    {
        // when object is selected
        protected override void OnSelectEntering(XRBaseInteractor interactor)
        {
            base.OnSelectEntering(interactor);
            Debug.Log("Building '" + this.gameObject.name + "' selected");

            // the map controller is the parents, parent game object, pass object to function on controller
            transform.parent.transform.parent.GetComponent<MapController>().InteractedWith(this.gameObject);
        }
    }
}
