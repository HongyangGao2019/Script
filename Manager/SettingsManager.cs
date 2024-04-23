using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : MonoBehaviour
{
    private float _sfxVolume=1;
    private float _musicVolume=1;
    public event UnityAction<float> OnSfxVolumeChanged;
    public event UnityAction<float> OnMusicVolumeChanged;
    public static SettingsManager Singleton { get; private set; }

    public float SfxVolume
    { 
        get { if (_sfxVolume <= 1&&_sfxVolume>=0) { return _sfxVolume; } else { _sfxVolume = 1; return _sfxVolume; } }
        private set { if (value > 1 || value < 0) { return; }else { _sfxVolume = value; } }
    }

    public float MusicVolume 
    { 
        get { if (_musicVolume <= 1&&_musicVolume>=0) { return _musicVolume; } else { _musicVolume = 1; return _musicVolume; } }
        private set { if (value > 1 || value < 0) { return; }else { _musicVolume = value; } }
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Singleton = this;
    }

    private void OnEnable()
    {
        OnSfxVolumeChanged += ChangedSfxVolume;
        OnMusicVolumeChanged += ChangedMusicVolume;
    }

    private void OnDisable()
    {
        OnSfxVolumeChanged -= ChangedSfxVolume;
        OnMusicVolumeChanged -= ChangedMusicVolume;

    }

    /// <summary>
    /// 提供改变SFX音量的方法给外部调用
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSfxVolume(float value)
    {
        OnSfxVolumeChanged?.Invoke(value);
    }

    /// <summary>
    /// 提供改变Music音量的方法给外部调用
    /// </summary>
    /// <param name="value"></param>
    public void ChangeMusicVolume(float value)
    {
        OnMusicVolumeChanged?.Invoke(value);
    }
    
    private void ChangedSfxVolume(float value)
    {
        _sfxVolume=value;
    }
    private void ChangedMusicVolume(float value)
    {
        _musicVolume=value;
    }
}
