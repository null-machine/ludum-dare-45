using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

internal class Crafter : MonoBehaviour {
  
  [SerializeField]
  private Slot[] slots;
  [SerializeField]
  private Result result;
  [SerializeField]
  private Transform inventory;
  [SerializeField]
  private Chara chara;
  [SerializeField]
  private GameObject eventSystem;
  
  private List<Recipe> recipes;
  
  private bool CompareContents<T>(IEnumerable<T> list1, IEnumerable<T> list2) {
    var dict = new Dictionary<T, int>();
    foreach (T item in list1) {
      if (item != null) {
        if (dict.ContainsKey(item)) {
          dict[item]++;
        } else {
          dict.Add(item, 1);
        }
      }
    }
    foreach (T item in list2) {
      if (item != null) {
        if (dict.ContainsKey(item)) {
          dict[item]--;
        } else {
          return false;
        }
      }
    }
    return dict.Values.All(count => count == 0);
  }
  
  internal Item Craft() {
    Item[] inputs = new Item[] {
      slots[0].item, slots[1].item, slots[2].item, slots[3].item
    };
    Item result = null;
    for (int i = 0; i < recipes.Count; i++) {
      if (CompareContents(inputs, recipes[i].inputs)) {
        result = recipes[i].result;
      }
    }
    for (int i = recipes.Count - 1; i >= 0; i--) {
      if (result == recipes[i].result) {
        recipes.RemoveAt(i);
      }
    }
    if (result != null) {
      StartCoroutine(DelayCollect(result));
    }
    return result;
  }
  
  internal void AddInput(Item item) {
    for (int i = 0; i < 4; i++) {
      if (slots[i].item == null) {
        slots[i].SetItem(item);
        Craft();
        break;
      }
    }
  }
  
  // turn into event
  private IEnumerator DelayCollect(Item item) {
    eventSystem.SetActive(false);
    yield return new WaitForSeconds(1f/3f);
    result.SetItem(item);
    yield return new WaitForSeconds(1.5f);
    Chara chara = Instantiate(this.chara, inventory);
    chara.item = item;
    chara.crafter = this;
    for (int i = 0; i < 4; i++) {
      slots[i].OnClick();
    }
    eventSystem.SetActive(true);
  }
  
  private void Start() {
    recipes = Resources.LoadAll("Recipes").Cast<Recipe>().ToList();
  }
}