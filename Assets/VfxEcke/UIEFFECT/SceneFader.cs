using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{

    public float FadeDuration = 1f;
    public FadeType CurrentFadeType;
    private int _fadeAmount = Shader.PropertyToID("_FadeAmount");

    private int _useShutters = Shader.PropertyToID("_UseShutters");
    private int _useRadialWipe = Shader.PropertyToID("_UseRadialWipe");
    private int _usePlainBlack = Shader.PropertyToID("_UsePlainBlack");
    private int _useGoop = Shader.PropertyToID("_useGoop");

    private int? _lastEffect;

    private Image _image;
    private Material _material;

    public enum FadeType
    {
        Shutters,
        RadialWipe,
        PlainBlack,
        Goop
    }


    private void Awake()
    {
        _image = GetComponent<Image>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        Material mat = _image.material;
        _image.material = new Material(mat);
        _material = _image.material;

        _lastEffect = _useShutters;
    }

    private void Update()
    {
        if (Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            FadeOut(CurrentFadeType);
        }
        if (Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            FadeIn(CurrentFadeType);
        }

    }

    public void FadeOut(FadeType fadeType)
    {
        ChangeFadeEffect(fadeType);
        StartFadeOut();
    }

    public void FadeIn(FadeType fadeType)
    {
        ChangeFadeEffect(fadeType);
        StartFadeIn();
    }

    private void ChangeFadeEffect(FadeType fadeType)
    {
        if (_lastEffect.HasValue)
        {
            _material.SetFloat(_lastEffect.Value, 0f);
        }

        switch (fadeType)
        {
            case FadeType.Shutters:

                SwitchEffect(_useShutters);
                break;

            case FadeType.RadialWipe:
                SwitchEffect(_useRadialWipe);
                break;

            case FadeType.PlainBlack:
                SwitchEffect(_usePlainBlack);
                break;

            case FadeType.Goop:
                SwitchEffect(_useGoop);
                break;
        }
    }

    private void SwitchEffect(int effectToTurnOn)
    {
        _material.SetFloat(effectToTurnOn, 1f);
        _lastEffect = effectToTurnOn;
    }
    private void StartFadeIn()
    {
        _material.SetFloat(_fadeAmount, 1f);

        StartCoroutine(HandleFade(0f, 1f));
    }

    private void StartFadeOut()
    {
        _material.SetFloat(_fadeAmount, 0f);

        StartCoroutine(HandleFade(1f, 0f));
    }

    private IEnumerator HandleFade(float targetAmount, float startAmount)
    {
        float elapsedTime = 0f;
        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(startAmount, targetAmount, (elapsedTime / FadeDuration));
            _material.SetFloat(_fadeAmount, lerpedAmount);

            yield return null;
        }
        _material.SetFloat(_fadeAmount, targetAmount);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
            StartCoroutine(DelayedFadeIn());
    }

    private IEnumerator DelayedFadeIn()
    {
        yield return new WaitForSeconds(1f);
        FadeIn(CurrentFadeType);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}




    


