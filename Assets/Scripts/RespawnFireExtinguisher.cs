using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnFireExtinguisher : MonoBehaviour
{
    [SerializeField] private FireExtinguisher fireExtinguisher;
    [SerializeField] private Transform fireExtinguisherPrefab;
    [SerializeField] private Storage storage;

    [SerializeField] private List<GameObject> stoves;

    private void Update()
    {
        if (fireExtinguisher == null && storage.GetItem() == null){
            Transform fireExtinguisherObject = Instantiate(fireExtinguisherPrefab);
            storage.SetItem(fireExtinguisherObject.GetComponent<FireExtinguisher>());
            fireExtinguisher = fireExtinguisherObject.GetComponent<FireExtinguisher>();
            foreach (GameObject stove in stoves){
                stove.GetComponent<Stove>().GetBreakable().SetRequiredHoldable(fireExtinguisher);
            }
        }
    }
}