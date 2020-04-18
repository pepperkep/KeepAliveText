using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;




public class LoadText : MonoBehaviour
{

  [SerializeField] private TextAsset texts;
  public List<string> myTexts;


  public List<string> getMyTexts(){
  return this.myTexts;
}


    // Start is called before the first frame update
    void Start()
    {
      myTexts=TextAssetToList(texts);
      
    }

    public List<string> TextAssetToList(TextAsset ta)
      {
          var listToReturn = new List<string>();
          var arrayString = ta.text.Split('\n');
          foreach (var line in arrayString)
          {
              listToReturn.Add(line);
          }
          return listToReturn;
      }




    // Update is called once per frame
    void Update()
    {

    }
}
