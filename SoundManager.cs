using System.Collections;
using UnityEngine;

public class Sound : MonoBehaviour {
    
	public AudioSource efxSource;                //効果音
	public AudioSource musivSource;              //BGM
	public static SoundManager instance = null;

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	private void Awake()                         //シングルトンの処理
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	public void PlaySingle(AudioClip clip)      //BGMの再生
	{
		efxSource.clip = clip;
		efxSource.Play();
	}

	public void RandomizeSfx(params AudioClip[] clips){
		int randomIndex = Random.Range(0, clips.Length);       //受け取った効果音番号をランダムで指定

		float randomPitch = Random.Range(lowPitchRange, highPitchRange);    //音の高さをランダムで指定
		efxSource.pitch = randomPitch;

		efxSource.clip = clips[randomIndex];                    //受け取った効果音を選択

		efxSource.Play();                       //効果音を再生
	}
}
