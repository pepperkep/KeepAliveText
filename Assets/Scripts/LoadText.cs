using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadText : MonoBehaviour
{

  [SerializeField] private TextAsset texts;
  public List<string> myTexts;


  public List<string> GetMyTexts(){
    return this.myTexts;
  }


    // Start is called before the first frame update
    void Awake()
    {
      myTexts=TextAssetToList(texts);
      
    }

    public List<string> TextAssetToList(TextAsset ta)
    {
          var listToReturn = new List<string>();
          var arrayString = ta.text.Split('\n');
          foreach (var line in arrayString)
          {
              string nextText = line.Replace("\\n", "\n");
              listToReturn.Add(nextText);
          }
          return listToReturn;
    }
}
