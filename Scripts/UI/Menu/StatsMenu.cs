using UnityEngine;
using System.Collections;

public class StatsMenu : MonoBehaviour
{
    [SerializeField] private TimeNumericDisplay m_bestSoloTime;
    [SerializeField] private NumericDisplay m_totalBarrelsDisplay;
    [SerializeField] private NumericDisplay m_wastedBeerDisplay;

    public void UpdateValues()
    {
        m_totalBarrelsDisplay.SetValue(ProfileInfo.SelectedProfileInfo().TotalBarrels());
        m_wastedBeerDisplay.SetValue(ProfileInfo.SelectedProfileInfo().WastedBeer());
        m_bestSoloTime.SetTime(ProfileInfo.SelectedProfileInfo().BestSoloTime());
    }
}
