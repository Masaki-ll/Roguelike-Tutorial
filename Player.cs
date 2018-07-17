using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject{

	public int wallDamage = 1;                          //壁へのダメージ量
	public int pointsPerFood = 10;                      //フードの回復量
	public int pointsPerSoda = 20;                      //ソーダの回復量
	public float restartlevelDelay = 1f;                //次レベルへ行くときの時間差

	public Text foodText;                               //FoodText

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;
    
    
    private Animator animator;                          //PlayerChop,PlayerHit用
	private int food;                                   //プレイヤーの体力
    
	protected override void Start()                     //MovingObjectのStartを継承　baseで呼び出し
	{
		animator = GetComponent<Animator>();            //Animatorをキャッシュ

		//シングルトンであるGameManagerのplayerFoodPointsを使ってレベルを跨いでも値を保持
		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food:" + food;

		base.Start();                                   //MovingObjectのStart呼び出し
	}

	private void OnDisable()
	{
		GameManager.instance.playerFoodPoints = food;
	}

	private void Update()
	{
		if (!GameManager.instance.playersTurn)
			return;
		
		int horizontal = 0;                             //-1:左,1:右
		int vertical = 0;                               //-1:下,1:上

		horizontal = (int)Input.GetAxisRaw("Horizontal");
		vertical = (int)Input.GetAxisRaw("Vertival");

		if (horizontal != 0){                 
			vertical = 0;                               //上下もしくは左右に移動を制限
		}

		if (horizontal != 0 || vertical != 0){
			AttemptMove<Wall>(horizontal, vertical);    //Playerの場合はWall以外判定する必要なし
		}
	}

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		food--;                                         //移動１回につき１ポイント失う
        foodText.text = "Food:" + food;
         
		base.AttemptMove<T>(xDir, yDir);

		RaycastHit2D hit;

		if(Move(xDir,yDir,out hit)){
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		CheckIfGameOver();
		GameManager.instance.playersTurn = false;
	}

	protected override void OnCantMove<T>(T component)
	{
		Wall hitWall = component as Wall;               //Wall型を定義、Wallスクリプトを表す
		hitWall.DamageWall(wallDamage);                 //WallスクリプトのDamageWallを呼び出し
		animator.SetTrigger("PlayerChop");              //Wallに攻撃するアニメーションを実行
	}

	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Exit")
		{
			Invoke("Restart", restartlevelDelay);
			enabled = false;                            //Playerを無効にする
		}
		else if (other.tag == "Food")
		{
			food += pointsPerFood;                      //体力を回復する

			foodText.text = "+" + pointsPerFood + "Food:" + food;

            //Foodを取った時、eatSound1かeatSound2を鳴らす
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);  

			other.gameObject.SetActive(false);          //otherオブを無効　
		}
		else if (other.tag == "Soda")
		{
			food += pointsPerSoda;                      //体力を回復する

			foodText.text = "+" + pointsPerSoda + "Food:" + food;

			//Sodaを取った時、drinkSound1かdrinkSounda2を鳴らす
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);

			other.gameObject.SetActive(false);          //otherオブを削除
		}
	}

	private void ReStart(){
		Application.LoadLevel(Application.loadedLevel);          //同じシーンを読み込む
	}

	public void LoseFood(int loss)
	{
		animator.SetTrigger("PlayerHit");
		food -= loss;

		foodText.text = "-" + loss + "Food:" + food;

		CheckIfGameOver();
	}

	private void CheckIfGameOver(){
		if(food<=0){

			SoundManager.instance.PlaySingle(gameOverSound);    //gameOverSoundを鳴らす
			SoundManager.instance.musicSource.Stop();           //BGMは停止する

			GameManager.instance.GameOver();                    //GameOverを実行
		}
	}
}

