using Script.Menu;
using UnityEngine;

namespace Script.Login
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        //Screen object variables
        public Menu.Menu loginUI;
        public Menu.Menu registerUI;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }
        //Functions to change the login screen UI
        public void LoginScreen() //Back button
        {
            MenuManager.Instance.OpenMenu(loginUI);
        }
        public void RegisterScreen() // Regester button
        {
            MenuManager.Instance.OpenMenu(registerUI);
        }
    }
}