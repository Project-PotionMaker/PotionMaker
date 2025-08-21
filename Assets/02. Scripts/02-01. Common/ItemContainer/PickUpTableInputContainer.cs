using UnityEngine;

public class PickUpTableInputContainer : IInputContainer<Furniture>
{
    public bool ServerTryInput(Furniture furniture, int tid, EInputType inputType, GameObject inputObject = null)
    {
        if (furniture.InputObject == null)
        {

            switch (inputType)
            {
                case EInputType.Ingredient:
                {
                    furniture.InputObject = CraftItemFactory.Instance.CreateObject(inputType, furniture.InputPosition.position, Quaternion.identity);
                    furniture.InputObject.GetComponent<IngredientItem>().ServerUpdateIngredientData(tid);
                    break;
                }
                case EInputType.Output:
                {
                    furniture.InputObject = CraftItemFactory.Instance.CreateObject(inputType, furniture.InputPosition.position, Quaternion.identity);
                    furniture.InputObject.GetComponent<OutputItem>().ServerUpdateOutputData(EInputType.Output, tid);
                    break;
                }
                case EInputType.FailureOutput:
                {
                    furniture.InputObject = CraftItemFactory.Instance.CreateObject(EInputType.Output, furniture.InputPosition.position, Quaternion.identity);
                    furniture.InputObject.GetComponent<OutputItem>().ServerUpdateOutputData(EInputType.FailureOutput, 10000);
                    break;
                }
                case EInputType.Potion:
                {
                    furniture.InputObject = CraftItemFactory.Instance.CreateObject(inputType, furniture.InputPosition.position, Quaternion.identity);
                    furniture.InputObject.GetComponent<PotionItem>().ServerUpdatePotionData(tid);
                    if (GridManager.Instance.PickupTableForCustomerList.Contains(furniture.netIdentity))
                    {
                        CustomerManager.Instance.CmdPlaceOnTable(tid, furniture.netId);
                        CustomerManager.Instance.CmdServePotion(tid, furniture.netId);
                    }
                    break;
                }
            }
            furniture.InputObject.transform.position = furniture.InputPosition.position;
            return true;
        }
        return false;
    }
}
