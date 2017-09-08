using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private InputField m_profileName;

    public void SetProfile(ProfileInfo profileInfo)
    {
        m_profileName.text = profileInfo.ProfileName();
    }

    public ProfileInfo GetProfileInfo()
    {
        ProfileInfo profileInfo = new ProfileInfo(m_profileName.text);

        return profileInfo;
    }
}
