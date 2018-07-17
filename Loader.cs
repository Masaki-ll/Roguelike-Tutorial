using System.Collections;
using UnityEngine;

public class Loader : MonoBehaviour {
	public GameObject gameManeger;          //GameManagerのプレハブを指定

	private void Awake()
	{
		if (GameManager.instance==null){
			Instantiate(gameManeger);
		}
	}
}
