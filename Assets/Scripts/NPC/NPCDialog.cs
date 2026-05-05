using Fungus;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public string ChatName;
    public GameObject pressEUI;

    private bool canChat = false;

    void Start()
    {
        if (pressEUI != null)
        {
            pressEUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canChat = true;

            if (pressEUI != null)
            {
                pressEUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canChat = false;

            if (pressEUI != null)
            {
                pressEUI.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (canChat && Input.GetKeyDown(KeyCode.E))
        {
            Say();
        }
    }

    void Say()
    {
        Flowchart flowChart = GameObject.Find("Flowchart").GetComponent<Flowchart>();

        if (flowChart.HasBlock(ChatName))
        {
            if (pressEUI != null)
            {
                pressEUI.SetActive(false);
            }

            flowChart.ExecuteBlock(ChatName);
        }
    }
}
