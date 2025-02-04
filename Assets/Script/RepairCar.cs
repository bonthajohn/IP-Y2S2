using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRepair : MonoBehaviour
{
    [System.Serializable]
    public class RepairItem
    {
        public string partName; // Name of the required part (keys, spoiler, etc.)
        public GameObject repairedVersion; // Object to enable when repaired
        public GameObject partToDisable; // Object to disable when repaired
    }

    public List<RepairItem> repairItems; // List of all repairable items

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CarPart")) // Ensure the collected part has the correct tag
        {
            CarPart part = other.GetComponent<CarPart>();
            if (part != null)
            {
                foreach (RepairItem repairItem in repairItems)
                {
                    if (part.partName == repairItem.partName)
                    {
                        GameManager.Instance.RepairPart(repairItem.partName);

                        // Destroy collected part
                        Destroy(other.gameObject);

                        // Enable repaired version if assigned
                        if (repairItem.repairedVersion != null)
                        {
                            repairItem.repairedVersion.SetActive(true);
                        }

                        // Disable old object if necessary
                        if (repairItem.partToDisable != null)
                        {
                            repairItem.partToDisable.SetActive(false);
                        }
                        break; // Exit loop once the correct part is found
                    }
                }
            }
        }
    }
}
