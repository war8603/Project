using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public enum FadeType
    {
        FadeIn,
        FadeOut,
    }
    private static FadeManager _instance;
    public static FadeManager Instance
    {
        get { return _instance; }
    }

    [SerializeField]
    private Image _backImage;

    System.Action _callBack;
    bool _isFading = false;
    FadeType _fadeType;
    public void Awake()
    {
        _instance = this;
    }

    public void Init()
    {
        this.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    public void Update()
    {
        if (_isFading == true)
        {
            if (_fadeType == FadeType.FadeIn)
            {
                Color tempColor = _backImage.color;
                float alphaValue = tempColor.a;
                alphaValue -= Time.deltaTime;
                if (alphaValue < 0)
                {
                    _isFading = false;
                    alphaValue = 0f;
                    _backImage.color = tempColor;
                    OnActionCallBack();
                }
                tempColor.a = alphaValue;
                _backImage.color = tempColor;
                if (alphaValue == 0f) OnActionCallBack();
            }
            else if (_fadeType == FadeType.FadeOut)
            {
                Color tempColor = _backImage.color;
                float alphaValue = tempColor.a;
                alphaValue += Time.deltaTime;
                if (alphaValue > 1f)
                {
                    _isFading = false;
                    alphaValue = 1f;
                    _backImage.color = tempColor;
                    OnActionCallBack();
                }
                tempColor.a = alphaValue;
                _backImage.color = tempColor;
                if (alphaValue == 0f) OnActionCallBack();
            }
        }
    }

    public void OnStartFadeIn(System.Action callBack, bool isImmediately = false)
    {
        // 알파값 0 -> 1
        _callBack = callBack;
        float alphaValue = 1f;
        if (isImmediately == true)
        {
            alphaValue = 0f;
        }

        Color tempColor = _backImage.color;
        tempColor.a = alphaValue;
        _backImage.color = tempColor;
        _isFading = true;
        _fadeType = FadeType.FadeIn;
    }

    public void OnStartFadeOut(System.Action callBack, bool isImmediately = false)
    {
        // 알파값 0 -> 1
        _callBack = callBack;
        float alphaValue = 0f;
        if (isImmediately == true)
        {
            alphaValue = 1f;
        }

        Color tempColor = _backImage.color;
        tempColor.a = alphaValue;
        _backImage.color = tempColor;
        _isFading = true;
        _fadeType = FadeType.FadeOut;
    }

    void OnActionCallBack()
    {
        System.Action tempAction = _callBack;
        _callBack = null;
        tempAction?.Invoke();
    }
}
