using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public int playerDamage;            //プレイヤーへのダメージ量

	private Animator animator;
	private Transform target;           //プレイヤーの位置情報
	private bool skipMove;              //敵キャラが動くかどうかの判定

	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;

	protected override void Start()     //MovingObjectのStartを継承
	{
		GameManager.instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();        //Animatorをキャッシュ
		target = GameObject.FindGameObjectWithTag("Player").transform;      //Playerの位置情報を取得
		base.Start();                   //MovObのStart呼び出し
	}

	protected override void AttemptMove<T>(int xDir,int yDir){
		if(skipMove){
			skipMove = false;
			return;
		}

		base.AttemptMove<T>(xDir, yDir);
		skipMove = true;                //移動が終了したらtrueにする
	}

	public void MoveEnemy(){
		int xDir = 0;
		int yDIr = 0;
		if(Mathf.Abs(target.position.x-transform.position.x)<float.Epsilon){
			yDIr = target.position.y > transform.position.y ? 1 : -1;       //プレイヤーが上なら＋１、下なら−１
		}else{
			xDir = target.position.x > transform.position.x ? 1 : -1;       //プレイヤーが右なら＋１、左なら−１
		}
		AttemptMove<Player>(xDir, yDir);
	}

	protected override void OnCantMove <T>(T component){
		Player hitPlayer = component as Player;
		animator.SetTrigger("enemyAttack");
		hitPlayer.LoseFood(playerDamage);

		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);     //攻撃用の効果音をSoundManagerに渡してランダム再生

	}
}
