using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionHandler : MonoBehaviour
{
    public TMP_Dropdown resDropDown;

    private Resolution[] _allResolutions;
    private bool _isFullScreen;
    private int _selectedResolution;

    private List<Resolution> selectedResolutionList = new List<Resolution>();
    private void Start()
    {
        _isFullScreen = true;
        _allResolutions = Screen.resolutions;

        string newRes;
        List<string> resolutionStringList = new List<string>();

        foreach (Resolution res in _allResolutions)
        {
            newRes = res.width.ToString() + "x" + res.height.ToString();

            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes.ToString());
                selectedResolutionList.Add(res);
            }

        }

        resDropDown.AddOptions(resolutionStringList);
        resDropDown.value = resDropDown.options.Count - 1;
    }

    public void ChangeResolution()
    {
        _selectedResolution = resDropDown.value;
        Screen.SetResolution(selectedResolutionList[_selectedResolution].width, selectedResolutionList[_selectedResolution].height, _isFullScreen);
    }
}
