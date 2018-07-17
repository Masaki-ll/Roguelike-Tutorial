using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                                    //UI用の宣言

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;                  //レベル表示画面で２秒待つ
	public float turnDelay = .1f;                       //Enemyの動作時間(0.1秒)
	public static GameManager instance = null;
    
	                                                    //シーン間で変数を共有、ゲーム内で固有の変数
                                                        //オブジェクトに属さずクラスに属す
	public BoardManager boardScript;

	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;                             //レベルテキスト
	private GameObject levelImage;                      //レベルイメージ
	private int level = 1;                              //レベルは1
	private bool doingSetup;                            //levelImageの表示などで活用

	private List<Enemy> enemies;                        //Enemyクラスの配列
	private bool enemiesMoving;                         //Enemyのターン中true

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;                            //ゲーム開始時にGameManagerをinstanceに指定
		}
		else if (instance != this)                      //このオブジェクト以外にGameManagerが存在するとき
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad (gameObject);                 //シーン遷移時にこのオブジェクトを引き継ぐ
		enemies = new List<Enemy>();                    //Enemyを格納する配列の作成

		boardScript = GetComponent<BoardManager>();     //BoardManagerを取得
		InitGame();
	}

	void InitGame(){
		doingSetup = true;                              //trueの間、プレイヤーは動けない
														//LevelImage,LevelTextオブの取得
		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Day" + level;                 //最新のレベルに更新
		levelImage.SetActive(true);                     //LebelImageをアクティブにして表示
		Invoke("HideLevelImage", levelStartDelay);      //２秒後にメソッド呼び出し

		enemies.Clear();                                //EnemyのList(配列)を初期化
		boardScript.SetupScene(level);                  //BoardManagerのSetupSceneを呼び出す
	}

	private void HideLevelImage(){
		levelImage.SetActive(false);                    //LevelImage非アクティブ化
		doingSetup = false;                             //プレイヤーが動けるようになる
       }

	public void GameOver(){
		                                                //ゲームオーバーメッセを表示
		levelText.text = "After" + level + "days,you starved";
		levelImage.SetActive(true);                      
		          
		enabled = false;                                //GameManagerを無効にする
	}
    
	// Update is called once per frame
	void Update () {
		                                                //doingSetup=trueの時はEnemyは動かない
		if (playersTurn || enemiesMoving)               //プレイヤーのターンかEnemyが動いた後ならUpdateしない
		
		{
			return;
		}

		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy script){
		enemies.Add(script);
	}
    
	IEnumerator MoveEnemies(){
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);
		if (enemies.Count==0){
			yield return new WaitForSeconds(turnDelay);
		}
		for (int i = 0; i < enemies.Count;i++)          //Enemyの数だけEnemyスクのMoveEnemyを実行
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		playersTurn = true;
		enemiesMoving = false;
	}
}
