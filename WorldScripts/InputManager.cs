using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    bool keySet = false;
    bool controllerSet = true;

    public string jump = "c_Jump";
    public string dash = "c_Dash";
    public string swap = "c_Swap";
    public string run = "c_Run";
    public string changeX = "c_changeX";
    public string changeY = "c_changeY";
    public string horizontal = "c_Horizontal";
    public string vertical = "c_Vertical";
    public string previewX = "c_previewX";
    public string previewY = "c_previewY";
    public string useItem = "c_UseItem";

    private void Update()
    {
        if(controllerSet && SwitchKeyboard())
        {
            //Debug.Log("Controller setting");
            controllerSet = false;
            SetKeyboard();
        }
        else if(keySet && SwitchController())
        {
            //Debug.Log("keyboard setting");
            keySet = false;
            SetController();
        }
    }

    private bool SwitchKeyboard()
    {
        //Debug.Log("Calling SwitchKeyboard()");

        bool isKey = false;

        if(Input.anyKey)
        {
            isKey = true;
        }

        //Debug.Log(isKey);

        return isKey;
    }

    private bool SwitchController()
    {
        //Debug.Log("Calling SwitchController()");

        bool isController = false;

        if (Input.GetKey(KeyCode.Joystick1Button0) ||
            Input.GetKey(KeyCode.Joystick1Button1) ||
            Input.GetKey(KeyCode.Joystick1Button2) ||
            Input.GetKey(KeyCode.Joystick1Button3) ||
            Input.GetKey(KeyCode.Joystick1Button4) ||
            Input.GetKey(KeyCode.Joystick1Button5) ||
            Input.GetKey(KeyCode.Joystick1Button6) ||
            Input.GetKey(KeyCode.Joystick1Button7) ||
            Input.GetKey(KeyCode.Joystick1Button8) ||
            Input.GetKey(KeyCode.Joystick1Button9) ||
            Input.GetKey(KeyCode.Joystick1Button10) ||
            Input.GetKey(KeyCode.Joystick1Button11) ||
            Input.GetKey(KeyCode.Joystick1Button12) ||
            Input.GetKey(KeyCode.Joystick1Button13) ||
            Input.GetKey(KeyCode.Joystick1Button14) ||
            Input.GetKey(KeyCode.Joystick1Button15) ||
            Input.GetKey(KeyCode.Joystick1Button16) ||
            Input.GetKey(KeyCode.Joystick1Button17) ||
            Input.GetKey(KeyCode.Joystick1Button18) ||
            Input.GetKey(KeyCode.Joystick1Button19))
        {
            isController = true;
        }
        else if(Input.GetAxisRaw("c_changeX") != 0 ||
            Input.GetAxisRaw("c_changeY") != 0 ||
            Input.GetAxisRaw("c_Horizontal") != 0 ||
            Input.GetAxisRaw("c_Vertical") != 0 ||
            Input.GetAxisRaw("c_previewX") != 0 ||
            Input.GetAxisRaw("c_previewY") != 0)
        {
            isController = true;
        }

            return isController;
    }

    private void SetKeyboard()
    {
        Debug.Log("KeyboardSet");

        jump = "k_Jump";
        dash = "k_Dash";
        swap = "k_Swap";
        run = "k_Run";
        changeX = "k_changeX";
        changeY = "k_changeY";
        horizontal = "k_Horizontal";
        vertical = "k_Vertical";
        previewX = "k_previewX";
        previewY = "k_previewY";
        useItem = "k_UseItem";

    keySet = true;
    }

    private void SetController()
    {
        Debug.Log("ControllerSet");

        jump = "c_Jump";
        dash = "c_Dash";
        swap = "c_Swap";
        run = "c_Run";
        changeX = "c_changeX";
        changeY = "c_changeY";
        horizontal = "c_Horizontal";
        vertical = "c_Vertical";
        previewX = "c_previewX";
        previewY = "c_previewY";
        useItem = "c_UseItem";

        controllerSet = true;
    }

    //::::::::::::::FOR LATER -- SWAP CONTROLS ON THE FLY:::::::::::::::://
    //
    //public static InputManager IM;

    ////player movements
    ////public KeyCode Jump { get; set; }
    //public KeyCode Swap { get; set; }
    //public KeyCode Dash { get; set; }
    //public KeyCode Run { get; set; }
    //public KeyCode Left { get; set; }
    //public KeyCode Right { get; set; }
    //public KeyCode Use { get; set; }

    ////change worlds
    //public KeyCode C1 { get; set; }
    //public KeyCode C2 { get; set; }
    //public KeyCode C3 { get; set; }
    //public KeyCode C4 { get; set; }

    ////preview other worlds
    //public KeyCode P1 { get; set; }
    //public KeyCode P2 { get; set; }
    //public KeyCode P3 { get; set; }
    //public KeyCode P4 { get; set; }

    //private void Awake()
    //{
    //    if(IM == null)
    //    {
    //        DontDestroyOnLoad(IM);
    //        IM = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

}
