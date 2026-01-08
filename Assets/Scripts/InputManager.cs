using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InputManager : MonoSingleton<InputManager>
{    
    public bool MoveForwardInput => false;

    Vector2 moveDirectionVec = new Vector2();
    public Vector2 MoveDirectionInput{
        get {
            moveDirectionVec.x = Input.GetAxisRaw("Horizontal");
            moveDirectionVec.y = Input.GetAxisRaw("Vertical");
            return moveDirectionVec;
        } 
    }

    public bool BrakeInput => false;

    public bool FireInput => Input.GetMouseButton(0);

    public bool HelpInput => Input.GetKeyDown(KeyCode.H);
    public bool UpgradeInput => Input.GetKeyDown(KeyCode.U);
    public bool EscapeInput => Input.GetKeyDown(KeyCode.Escape);

    public bool RInput => Input.GetKeyDown(KeyCode.R);
}
