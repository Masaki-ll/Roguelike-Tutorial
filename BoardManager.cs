using System;
using System.Collections.Generic;       //Listを使うため
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]      //カウント用のクラスを設定
	public class Count
	{
		public int minimum;
		public int maximum;
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8;         //縦の列は8
	public int rows = 8;            //横の列は8
	public Count wallCount = new Count(5, 9);               //壁は5~9で出現
	public Count foodCount = new Count(1, 5);               //アイテムは1~5で出現

	public GameObject exit;                       //出口は単体
	public GameObject[] floorTiles;               //床は複数あるため配列
	public GameObject[] wallTiles;                //障壁
	public GameObject[] foodTiles;                //アイテム
	public GameObject[] enemyTiles;               //敵
	public GameObject[] outerWallTiles;           //外壁

	private Transform boardHolder;       //位置情報を保存する変数
	private List<Vector3> gridPositions = new List<Vector3>();   //配置可能範囲のリスト
    //Listは可変型の配列
    
	void InitialiseList()                                   //gridPositionsをクリア
		{
		gridPositions.Clear();                              //gridPotisionにオブジェクトの配置可能範囲を指定
        //x=1~6をループ
		for (int x = 1; x < columns - 1; x++)               //y=1~6をループ
		{
			for (int y = 1; y < rows - 1;y++){
				gridPositions.Add(new Vector3(x, y, 0f));   //6*6の範囲をgrodPositionsに指定
			}   
		}
	}
    //外壁、床を配置   
	void BoardSetup(){
		boardHolder = new GameObject("Board").transform;    //オブジェクト"Board"を作成し、transform情報をboardHolderに保存
                                                            
		for (int x = -1; x < columns + 1; x++)              //x=-1~8をループ
		{     for (int y = -1; y < rows + 1; y++)           //y=-1~8をループ
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];  //床をランダムで選択
                                                                                            //左右上下に外壁を作る
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)]; //外壁をランダムで選択し上書き
                }
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity)
                    as GameObject;                          //床または外壁を生成しinstance変数に格納
                instance.transform.SetParent(boardHolder);
            }
        }
		
	}
       
	Vector3 RandomPosition(){
		int randomIndex = Random.Range(0, gridPositions.Count);         //0~36からランダムで１つ決定し位置情報を確定
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);                            //ランダムで決定した数値は削除
		return randomPosition;                                          //確定した位置情報を返す
	}

	void LayoutObjectAtRandom(GameObject[]tileArray,int minimum,int maximum){
		int objectCount = Random.Range(minimum, maximum + 1);           //最小値〜最大値＋１のランダム回数分だけループ
		for (int i = 0; i < objectCount;i++){
			Vector3 randomPosition = RandomPosition();                  //gridPositionから位置情報を１つ取得
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];   //引数tileArrayからランダムで１つ選択
			Instantiate(tileChoice, randomPosition, Quaternion.identity);           //ランダムの種類・位置でオブジェクトを生成
		}
	}
    //オブジェクトを配置する
    //床を生成するときGameManagerに呼ばれる
	public void SetupScene(int level){
		BoardSetup();                                       //床と外壁を設置
		InitialiseList();                                   //敵・障壁・アイテムを配置可能な位置を決定
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);      //障壁・アイテム・敵キャラをランダムで配置
		int enemyCount = (int)Mathf.Log(level, 2f);
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);                   //対数で計算
		Instantiate (exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);         //Exitを(7,7)に配置
	}
}
