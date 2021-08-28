using System.Collections;
using System.Collections.Generic;
using Script.EntityPlayer;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandsScript : MonoBehaviour
{
    // ------------ SerializedField ------------
    
    [Header("Texte des boutons")]
    [SerializeField] private TMP_Text Forward;
    [SerializeField] private TMP_Text Backward;
    [SerializeField] private TMP_Text Left;
    [SerializeField] private TMP_Text Right;
    [SerializeField] private TMP_Text Jump;
    [SerializeField] private TMP_Text Sprint;
    [SerializeField] private TMP_Text Crouch;
    [SerializeField] private TMP_Text Sit;
    [SerializeField] private TMP_Text design;

    // ------------ Attributs ------------
    
    private Event keyEvent;
    private KeyCode newKey;
    private bool waitingForKey;
    private TouchesClass touches;
    
    private Dictionary<TypeTouche, TMP_Text> dict;
    
    // ------------ Constructeurs ------------
    void Start()
    {
        touches = TouchesClass.Instance;
        
        waitingForKey = false;

        dict = new Dictionary<TypeTouche, TMP_Text>();
        dict.Add(TypeTouche.Avancer, Forward);
        dict.Add(TypeTouche.Reculer, Backward);
        dict.Add(TypeTouche.Droite, Right);
        dict.Add(TypeTouche.Gauche, Left);
        dict.Add(TypeTouche.Sprint, Sprint);
        dict.Add(TypeTouche.Jump, Jump);
        dict.Add(TypeTouche.Accroupi, Crouch);
        dict.Add(TypeTouche.Assoir, Sit);
        //dict.Add(TypeTouche.ChangerDesign, design);

        // afficher les string des KeyCode
        foreach (KeyValuePair<TypeTouche, TMP_Text> e in dict)
        {
            e.Value.text = touches.ToString(e.Key);
        }
    }

    // ------------ Méthodes ------------
    void OnGUI()
    {
        keyEvent = Event.current;
        if (keyEvent.isKey && waitingForKey)
        {
            newKey = keyEvent.keyCode;
            waitingForKey = false;
        }
    }

    public void StartAssignment(int typeTouche)
    {
        if (!waitingForKey)
        {
            StartCoroutine(AssignKey((TypeTouche)typeTouche));
        }
    }

    private IEnumerator WaitForKey()
    {
        while (!keyEvent.isKey)
            yield return null;
    }

    private IEnumerator AssignKey(TypeTouche toucheAssigné)
    {
        waitingForKey = true;
        yield return WaitForKey();
        
        foreach (TypeTouche toucheReset in touches.GetSameTouches(newKey))
        {
            SetTouche(toucheReset, TouchesClass.GetNullKeyCode());
        }
            
        SetTouche(toucheAssigné, newKey);

        yield return null;

        void SetTouche(TypeTouche toucheChangé, KeyCode key)
        {
            touches.SetKey(toucheChangé, key);
            dict[toucheChangé].text = touches.ToString(toucheChangé);
            PlayerPrefs.SetString(touches.GetStrSauvegarde(toucheChangé), touches.GetKey(toucheChangé).ToString());
        }
    }
}
