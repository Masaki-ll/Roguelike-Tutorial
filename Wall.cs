using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour {

	public AudioClip chopSound1;
	public AudioClip chopSound2;

	public Sprite dmgSprite;                //攻撃された時に表示する内壁のスプライト画像
	public int hp = 3;                      //内壁の体力

	private SpriteRenderer spriteRenderer;

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer>();        //SpriteRendererを隠しておく
	}
    
    //プレイヤーが障壁を攻撃した時に、PlayerのOnCantMoveから呼び出し
	public void DamageWall(int loss){
		spriteRenderer.sprite = dmgSprite;  //public変数で指定画像を表示
		hp -= loss;         //体力を因数分だけ減らす

		if (hp <= 0)        //体力が0になった時
			gameObject.SetActive(false);

	}
}
