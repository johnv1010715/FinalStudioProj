using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandConsole : MonoBehaviour
{
    InputField field;
    GameController gameController;

    string previousCommand;

    bool isDay = true;
    bool downfall;

    void Start()
    {
        field = GetComponentInChildren<InputField>(true);
        gameController = FindObjectOfType<GameController>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            field.gameObject.SetActive(true);
            field.Select();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            field.text = previousCommand;
        }
    }

    public void Command(string input)
    {
        string[] parts = input.Split(new char[] { ' ' }, 2);
        if (parts.Length == 2)
        {
            SendMessage(SetCase(parts[0]), parts[1]);
        }
        else
        {
            SendMessage(SetCase(parts[0]));
        }
        previousCommand = field.text;
        field.text = null;
        field.gameObject.SetActive(false);
    }

    void Log(string input)
    {
        Debug.Log(input);
    }

    void Get(string input)
    {
        string[] parts = input.Split(new char[] { ' ' }, 2);
        switch (SetCase(parts[0]))
        {
            case ("Weapon"):
                int id;
                if (parts.Length == 2)
                {
                    if (parts[1].Length > 2 && SetCase(parts[1]) == "All")
                    {
                        for (int i = 0; i < gameController.weapons.Length; i++)
                        {
                            gameController.SpawnWeapon(i, 0, false);
                        }
                    }
                    else if (int.TryParse(parts[1], out id))
                    {
                        gameController.SpawnWeapon(id, 0, false);
                    }
                }
                return;
            case ("Resource"):
                int value;
                if (parts.Length == 2 && int.TryParse(parts[1], out value))
                {
                    gameController.AddResource(value);
                }
                else
                {
                    gameController.AddResource(100);
                }
                return;
            case ("Tokens"):
                if (parts.Length == 2 && int.TryParse(parts[1], out value))
                {
                    gameController.AddTokens(value);
                }
                else
                {
                    gameController.AddTokens(100);
                }
                return;
            case ("Health"):
                GameController.players[0].Heal(100);
                return;
        }
    }

    void Kill()
    {
        Kill("Player");
    }

    void Kill(string input)
    {
            switch (SetCase(input))
            {
                case ("Player"):
                GameController.players[0].Damage(1000, Vector3.zero, Vector3.zero, new HitData());
                    return;
                case ("All"):
                gameController.KillAll();
                    return;
            }
    }

    void Game(string input)
    {
        switch (SetCase(input))
        {
            case ("Start"):
                gameController.StartGame();
                return;
            case ("Reset"):
                gameController.ResetGame();
                return;
        }
    }

    void Time(string input)
    {
        string[] parts = input.Split(new char[] { ' ' }, 2);
        switch (SetCase(parts[0]))
        {
            case ("Set"):
                if (parts.Length == 2)
                {
                    Light light = GameObject.FindGameObjectWithTag("Directional Light").GetComponent<Light>();
                    if (SetCase(parts[1]) == "Day")
                    {
                        light.intensity = downfall ? 0.5f : 1;
                    }
                    if (SetCase(parts[1]) == "Night")
                    {
                        light.intensity = 0;
                    }
                }
                return;
        }
    }

    void Toggledownfall()
    {
        Light light = GameObject.FindGameObjectWithTag("Directional Light").GetComponent<Light>();
        ParticleSystem ps = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<ParticleSystem>();
        var em = ps.emission;

        if (downfall)
        {
            downfall = false;
            em.enabled = true;
            em.rateOverTime = 0;
            light.intensity = isDay ? 1 : 0;
        }
        else
        {
            downfall = true;
            em.enabled = true;
            em.rateOverTime = 200;
            ps.Play();
            light.intensity = isDay ? 0.5f : 0;
        }
    }

    string SetCase(string input)
    {
        return input.Remove(1).ToUpper() + input.Remove(0, 1).ToLower();
    }
}
