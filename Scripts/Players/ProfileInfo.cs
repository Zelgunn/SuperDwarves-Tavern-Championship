using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class ProfileInfo
{
    static List<ProfileInfo> s_allProfileInfos = null;
    static int s_selectedProfile = -1;

    private string m_profileName;
    private int m_totalBarrels = 0;
    private int m_wastedBeer = 0;
    private int m_bestSoloTime = 3600;

    public ProfileInfo()
    {
        m_profileName = "Inconnu";
    }

    public ProfileInfo(string profileName)
    {
        m_profileName = profileName;
    }

    public string ProfileName()
    {
        return m_profileName;
    }

    public void SetProfileName(string profileName)
    {
        m_profileName = profileName;
    }

    public void IncreaseTotalBarrel(int amount)
    {
        m_totalBarrels += amount;
    }

    public int TotalBarrels()
    {
        return m_totalBarrels;
    }

    public int WastedBeer()
    {
        return m_wastedBeer;
    }

    public void IncreaseWastedBeer(int amount)
    {
        m_wastedBeer += amount;
    }

    public int BestSoloTime()
    {
        return m_bestSoloTime;
    }

    public void SetSoloTime(int time)
    {
        if(time < m_bestSoloTime)
        {
            m_bestSoloTime = time;
        }
    }

    static public bool ProfileInfosLoaded()
    {
        return s_allProfileInfos != null;
    }

    static public void LoadProfileInfos()
    {
        if(s_allProfileInfos != null)
        {
            Debug.LogWarning("LoadProfileInfos : Informations de profil déjà chargées.");
            return;
        }

        if (!File.Exists("config.txt"))
            return;

        s_allProfileInfos = new List<ProfileInfo>();

        StreamReader streamReader = new StreamReader("config.txt", Encoding.UTF8);
        string line = "";
        ProfileInfo profileInfo = null;
        using(streamReader)
        {
            while(line != null)
            {
                line = streamReader.ReadLine();
                if (line == null)
                    break;

                string[] values = line.Split('=');
                // values[0] => Champ
                // values[1] => Valeur du champ
                switch(values[0])
                {
                    case "Profile":
                        if (profileInfo != null)
                        s_allProfileInfos.Add(profileInfo);

                        profileInfo = new ProfileInfo(values[1]);
                        break;
                    case "TotalBarrels":
                        profileInfo.m_totalBarrels = Int32.Parse(values[1]);
                        break;

                    case "WastedBeer":
                        profileInfo.m_wastedBeer = Int32.Parse(values[1]);
                        break;

                    case "BestSoloTime":
                        profileInfo.m_bestSoloTime = Int32.Parse(values[1]);
                        break;
                }
            }
        }

        // Ajout du dernier élément
        if((profileInfo != null) && !s_allProfileInfos.Contains(profileInfo))
            s_allProfileInfos.Add(profileInfo);

        if (s_allProfileInfos.Count > 0)
            s_selectedProfile = 0;
        else
            s_allProfileInfos = null;
    }

    static public void AddProfileInfo(ProfileInfo profileInfo)
    {
        if(s_allProfileInfos == null)
        {
            s_allProfileInfos = new List<ProfileInfo>();
        }

        s_allProfileInfos.Add(profileInfo);
        s_selectedProfile = s_allProfileInfos.Count - 1;
    }

    /// <summary>
    /// Remplace les informations du profil actuel.
    /// Si aucun profil n'a été créé, la liste est initialisée et le profil inséré dedans.
    /// </summary>
    /// <param name="profileInfo">Nouveau profil</param>
    static public void ReplaceProfileInfo(ProfileInfo profileInfo)
    {
        ReplaceProfileInfo(s_selectedProfile, profileInfo);
    }

    /// <summary>
    /// Remplace les informations de profil à l'index donné par les nouvelles informations.
    /// Si aucun profil n'a été créé, la liste est initialisée.
    /// Si l'index est invalide, le nouveau profil est inséré à la fin de la liste.
    /// </summary>
    /// <param name="index">Index du profil à remplacer</param>
    /// <param name="profileInfo">Nouveau profil</param>
    static public void ReplaceProfileInfo(int index, ProfileInfo profileInfo)
    {
        if (s_allProfileInfos == null)
        {
            s_allProfileInfos = new List<ProfileInfo>();
            s_selectedProfile = 0;
        }

        if((s_allProfileInfos.Count <= index) || (index < 0))
        {
            s_allProfileInfos.Add(profileInfo);
        }
        else
        {
            s_allProfileInfos[index] = profileInfo;
        }
    }

    static public ProfileInfo SelectedProfileInfo(int index = -1)
    {
        if(s_allProfileInfos == null)
        {
            Debug.LogWarning("SelectedProfileInfo : Les informations de profil n'ont pas été chargées.");
            return null;
        }

        if (index < 0)
            index = s_selectedProfile;

        if ((index >= s_allProfileInfos.Count)
            || (index < 0))
        {
            Debug.LogWarning("SelectedProfileInfo : Le profil à l'index donné n'existe pas.");
            return null;
        }

        return s_allProfileInfos[index];
    }

    static public void SaveProfileInfos()
    {
        StreamWriter streamWriter = new StreamWriter("config.txt", false, Encoding.UTF8);
        using(streamWriter)
        {
            foreach (ProfileInfo profileInfo in s_allProfileInfos)
            {
                streamWriter.WriteLine("Profile=" + profileInfo.ProfileName());
                streamWriter.WriteLine("TotalBarrels=" + profileInfo.m_totalBarrels);
                streamWriter.WriteLine("WastedBeer=" + profileInfo.m_wastedBeer);
                streamWriter.WriteLine("BestSoloTime=" + profileInfo.m_bestSoloTime);
            }
        }
    }
}
