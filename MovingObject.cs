using System.Collections;
using UnityEngine;
public abstract class MovingObject:MonoBehaviour{
	
	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;

	private float inverseMoveTime;              //MoveTimeを計算するのを単純化するための変数

	protected virtual void Start(){
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();     //常にBoxColider2D・RigidbodyにGetcomponent
		inverseMoveTime = 1f / moveTime;
	}

	protected bool Move (int xDir,int yDir, out RaycastHit2D hit){
		Vector2 start = transform.position;     //現在地を取得
		Vector2 end = start + new Vector2(xDir, yDir);      //目的地を取得

		boxCollider.enabled = false;            //自身のColiderを無効にし、Linecastで自分自身を判定しないようにする
		hit = Physics2D.Linecast(start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform==null)               //何もなければSmoothMovementへ遷移し移動処理
		{
			StartCoroutine(SmoothMovement(end));
			return true;                        //移動が成功したことを伝える
		}

		return false;                           //移動が失敗したことを伝える
	}

	protected IEnumerator SmoothMovement(Vector3 end)       //現在地と目的地の２点間の距離を求める(Vector3)
		{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;   //ベクトルを２乗した後、２点間の距離に変換(float)
		while (sqrRemainingDistance>float.Epsilon){         //２点間の距離が０になった時、ループ終わり
            
			//現在地と移動先の間を１秒間にinverseMoveTime分だけ移動する場合の1f分の移動距離を算出
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);     //算出した移動距離分、移動する

			//現在地が目的地に寄ると、sqrRemainDistanceが小さくなる
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;                  //1f待ってから、while分の先頭へ戻る
		}
	}

	protected virtual void AttemptMove<T>(int xDir,int yDir)            //移動
		where T:Component{

		RaycastHit2D hit;
        //Moveを実行→戻り値がtrueなら移動成功、falseなら移動失敗
		bool canMove = Move(xDir, yDir, out hit);           
		if(hit.transform==null)             //Moveで確認した障害物が何もなければ終了
		{
			return;
		}
		//障害物がある場合、障害物を型引数の型で取得、型が<T>で指定したものと違う場合、取得できない
		T hitComponent = hit.transform.GetComponent<T>();
        

		if(!canMove && hitComponent !=null){
			OnCantMove(hitComponent);           //障害物がある場合OncantMoveを呼び出す
		}
	}

	//障害物があり移動できなかった場合呼び出される
	protected abstract void OnCantMove<T>(T component) where T : Component;
    
}
