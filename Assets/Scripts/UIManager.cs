using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject SubmitForm;

    [SerializeField] GameObject ResubmitForm;

    [SerializeField] ARSession session;

    private void Start()
    {
        ResubmitForm.SetActive(false);

        SubmitForm.SetActive(true);
    }

    public void OnSubmitForm()
    {
        SubmitForm.SetActive(false);
        ResubmitForm.SetActive(true);

        session.Reset();
    }

    public void OnResubmit()
    {
        ResubmitForm.SetActive(false);
        SubmitForm.SetActive(true);
    }
}
