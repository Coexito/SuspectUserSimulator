using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteStateController : MonoBehaviour
{
    // Assigned from inspector!!
    [SerializeField] private List<Sprite> stateSprites; // 0-wander, 1-go, 2-work, 3-vote, 4-kill, 5-die
                                                        // 6-think, 7-alarm, 8-sabotage
    private Image stateIcon;
    
    private void Awake() 
    {
        stateIcon = transform.FindDeepChild("StateIcon").GetComponent<Image>();
    }

    public void SetStateIcon(string state)
    {
        // Changes the icon depending on the state passed as a string
        if(stateSprites.Count != 0)
        {
            switch(state)
            {
                case "wander": default:
                    stateIcon.sprite = stateSprites[0];
                    break;
                case "go":
                    stateIcon.sprite = stateSprites[1];
                    break;
                case "work":
                    stateIcon.sprite = stateSprites[2];
                    break;
                case "vote":
                    stateIcon.sprite = stateSprites[3];
                    break;
                case "kill":
                    stateIcon.sprite = stateSprites[4];
                    break;
                case "die":
                    stateIcon.sprite = stateSprites[5];
                    break;
                case "think":
                    stateIcon.sprite = stateSprites[6];
                    break;
                case "alarm":
                    stateIcon.sprite = stateSprites[7];
                    break;
                case "sabotage":
                    stateIcon.sprite = stateSprites[8];
                    break;
                
            }
        }
        
    }
}
