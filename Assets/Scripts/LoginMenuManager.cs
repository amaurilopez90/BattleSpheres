//This script controls the UI in the Database Control (Free) demo scene
//It uses database control to login, register and send and recieve data

using UnityEngine;
using System; //allows string.Split to be used with SplitStringOptions.none
using System.Collections;
using DatabaseControl;

public class LoginMenuManager : MonoBehaviour
{
    ////These variables are set in the Inspector:

    //they are enabled and disabled to show and hide the different parts of the UI
    public GameObject login_object;
    public GameObject register_object;
    public GameObject loading_object;

    //these are the login input fields:
    public UnityEngine.UI.InputField input_login_username;
    public UnityEngine.UI.InputField input_login_password;

    //these are the register input fields:
    public UnityEngine.UI.InputField input_register_username;
    public UnityEngine.UI.InputField input_register_password;
    public UnityEngine.UI.InputField input_register_confirmPassword;

    //red error UI Texts:
    public UnityEngine.UI.Text login_error;
    public UnityEngine.UI.Text register_error;

    ////These variables cannot be set in the Inspector:

    //the part of UI currently being shown
    // 0 = login, 1 = register, 2 = logged in, 3 = loading
    int part = 0;
    //scene starts showing login

    bool isDatabaseSetup = true;

    void Start()
    {

        isDatabaseSetup = true;
        
        //sets error Texts string to blank
        blankErrors();
    }

    void Update()
    {

        if (isDatabaseSetup == true)
        {

            //enables and disables the defferent objects to show correct part
            if (part == 0)
            {
                login_object.gameObject.SetActive(true);
                register_object.gameObject.SetActive(false);
                loading_object.gameObject.SetActive(false);
            }
            if (part == 1)
            {
                login_object.gameObject.SetActive(false);
                register_object.gameObject.SetActive(true);
                loading_object.gameObject.SetActive(false);
            }
            if (part == 2)
            {
                //Should be logged in, and already transitioned to a new scene
            }
            if (part == 3)
            {
                login_object.gameObject.SetActive(false);
                register_object.gameObject.SetActive(false);
                loading_object.gameObject.SetActive(true);
            }

        }

    }

    void blankErrors()
    {
        //blanks all error texts when part is changed e.g. login > Register
        login_error.text = "";
        register_error.text = "";
    }

    public void login_Register_Button()
    { //called when the 'Register' button on the login part is pressed
        part = 1; //show register UI
        blankErrors();
    }

    public void register_Back_Button()
    { //called when the 'Back' button on the register part is pressed
        part = 0; //goes back to showing login UI
        blankErrors();
    }

    public void data_LogOut_Button()
    { //called when the 'Log Out' button on the data part is pressed
        part = 0; //goes back to showing login UI
        UserAccountManager.instance.LogOut();
        blankErrors();
    }

    public void login_login_Button()
    { //called when the 'Login' button on the login part is pressed

        if (isDatabaseSetup == true)
        {

            //check fields aren't blank
            if ((input_login_username.text != "") && (input_login_password.text != ""))
            {

                //check fields don't contain '-' (if they do, login request will return with error and take longer)
                if ((input_login_username.text.Contains("-")) || (input_login_password.text.Contains("-")))
                {
                    //string contains "-" so return error
                    login_error.text = "Unsupported Symbol '-'";
                    input_login_password.text = ""; //blank password field
                }
                else
                {
                    //ready to send request
                    StartCoroutine(sendLoginRequest(input_login_username.text, input_login_password.text)); //calls function to send login request
                    part = 3; //show 'loading...'
                }

            }
            else
            {
                //one of the fields is blank so return error
                login_error.text = "Field Blank!";
                input_login_password.text = ""; //blank password field
            }

        }

    }

    IEnumerator sendLoginRequest(string username, string password)
    {

        if (isDatabaseSetup == true)
        {

            IEnumerator e = DCF.Login(username, password);
            while (e.MoveNext())
            {
                yield return e.Current;
            }
            //WWW returned = e.Current as WWW;
            string response = e.Current as string;
            Debug.Log("The response is: " + response);
            if (response == "Success")
            {
                //Password was correct
                blankErrors();
                part = 2; //show logged in UI

                //blank username field
                input_login_username.text = ""; //password field is blanked at the end of this function, even when error is returned

                //set logged in username and password to variables
                UserAccountManager.instance.LogIn(username, password);
            }
            else
            {
                login_error.text = "Database Error. Either that account does not exist, or contains an unsupported symbol '-', or maybe it's just our fault.";
                part = 0;
            }

            if (response == "incorrectUser")
            {
                //Account with username not found in database
                login_error.text = "Username not found";
                part = 0; //back to login UI
            }
            if (response == "incorrectPass")
            {
                //Account with username found, but password incorrect
                part = 0; //back to login UI
                login_error.text = "Incorrect Password";
            }
            if (response == "ContainsUnsupportedSymbol")
            {
                //One of the parameters contained a - symbol
                part = 0; //back to login UI
                login_error.text = "Unsupported Symbol '-'";
            }
            if (response == "Error")
            {
                //Account Not Created, another error occurred
                part = 0; //back to login UI
                login_error.text = "Database Error. Try again later.";
            }

            //blank password field
            input_login_password.text = "";

        }
    }

    public void register_register_Button()
    { //called when the 'Register' button on the register part is pressed

        if (isDatabaseSetup == true)
        {

            //check fields aren't blank
            if ((input_register_username.text != "") && (input_register_password.text != "") && (input_register_confirmPassword.text != ""))
            {

                //check username is longer than 4 characters
                if (input_register_username.text.Length > 4)
                {

                    //check password is longer than 6 characters
                    if (input_register_password.text.Length >= 6)
                    {

                        //check passwords are the same
                        if (input_register_password.text == input_register_confirmPassword.text)
                        {

                            if ((input_register_username.text.Contains("-")) || (input_register_password.text.Contains("-")) || (input_register_confirmPassword.text.Contains("-")))
                            {

                                //string contains "-" so return error
                                register_error.text = "Unsupported Symbol '-'";
                                input_login_password.text = ""; //blank password field
                                input_register_confirmPassword.text = "";

                            }
                            else
                            {

                                //ready to send request
                                StartCoroutine(sendRegisterRequest(input_register_username.text, input_register_password.text, "[KILLS]0/[DEATHS]0")); //calls function to send register request
                                part = 3; //show 'loading...'
                            }

                        }
                        else
                        {
                            //return passwords don't match error
                            register_error.text = "Passwords don't match!";
                            input_register_password.text = ""; //blank password fields
                            input_register_confirmPassword.text = "";
                        }

                    }
                    else
                    {
                        //return password too short error
                        register_error.text = "Password too Short, must be at least 6 characters long";
                        input_register_password.text = ""; //blank password fields
                        input_register_confirmPassword.text = "";
                    }

                }
                else
                {
                    //return username too short error
                    register_error.text = "Username too Short";
                    input_register_password.text = ""; //blank password fields
                    input_register_confirmPassword.text = "";
                }

            }
            else
            {
                //one of the fields is blank so return error
                register_error.text = "Field Blank!";
                input_register_password.text = ""; //blank password fields
                input_register_confirmPassword.text = "";
            }

        }

    }

    IEnumerator sendRegisterRequest(string username, string password, string data)
    {

        if (isDatabaseSetup == true)
        {

            IEnumerator ee = DCF.RegisterUser(username, password, data);
            while (ee.MoveNext())
            {
                yield return ee.Current;
            }
            //WWW returnedd = ee.Current as WWW;
            string response = ee.Current as string;

            if (response == "Success")
            {
                //Account created successfully

                blankErrors();
                part = 2; //show logged in UI

                //blank username field
                input_register_username.text = ""; //password field is blanked at the end of this function, even when error is returned

                //set logged in username and password to variables
                UserAccountManager.instance.LogIn(username, password);
            }
            else
            {
                part = 1;
                register_error.text = "Database Error. Either the username is unavailable or contains an Unsupported Symbol '-'";
            }

            if (response == "usernameInUse")
            {
                //Account Not Created due to username being used on another Account
                part = 1;
                register_error.text = "Username Unavailable. Try another.";
            }
            if (response == "ContainsUnsupportedSymbol")
            {
                //Account Not Created as one of the parameters contained a - symbol
                part = 1;
                register_error.text = "Unsupported Symbol '-'";
            }
            if (response == "Error")
            {
                //Account Not Created, another error occurred
                part = 1;
                login_error.text = "Database Error. Try again later.";
            }

            input_register_password.text = "";
            input_register_confirmPassword.text = "";

        }
    }

}