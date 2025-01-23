using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBgm;
    private int bgmIndex;

    private void Awake()
    {
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Update()
    {
        if(!playBgm)
            StopAllBgm();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
                PlayBgm(bgmIndex);
        }
    }

    public void PlaySfx(int _sfxIndex,Transform _source = null)
    {
        if (sfx[_sfxIndex].isPlaying)
            return;

        if (_source != null)
        {
            if (Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinDistance)
                return;
        }

        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(.85f, 1.15f);
            sfx[_sfxIndex].Play();
        }
    }
    public void StopSfx(int _index) => sfx[_index].Stop();

    public void PlayRandomBgm() => bgmIndex = Random.Range(0, bgm.Length);

    public void PlayBgm(int _bgmIndex)
    {
        if (_bgmIndex >= bgm.Length)
            return;

        StopAllBgm();

        bgm[_bgmIndex].Play();
    }
    public void StopAllBgm()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}
