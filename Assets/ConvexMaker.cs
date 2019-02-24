using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexMaker : MonoBehaviour {

	private List<Vector2> _points = null;
	private List<Vector2> _convexPoints = null;
	public int POINT_NUM = 50;
	public float WIDTH = 30f;
	public float HEIGHT = 30f;

	public float POINT_RADIUS = 0.5f;
	[SerializeField]
	private Transform _directionTarget = null;

	void Awake() {
		_points = new List<Vector2> ();
		_convexPoints = new List<Vector2> ();
	}

	void Start () {
		MakePoint ();
		GetConvex ();
	}
	
	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		if (_points != null) {
			_points.ForEach (DrawPoint);
		}
		DrawConvex ();
		Vector2 direction = _directionTarget.position;

		Debug.DrawLine (Vector2.zero, direction);
		Vector2 supportPoint = GetSupportPoint (direction);
		Gizmos.color = Color.blue;
		DrawPoint (supportPoint);
	}

	//データセット用のランダム点の生成
	public void MakePoint(){
		for(int i=0; i<POINT_NUM; i++){
			float x = Random.Range(-WIDTH, WIDTH);
			float y = Random.Range(-HEIGHT, HEIGHT);
			_points.Add(new Vector2(x, y));
		}
	}

	//点の描画
	public void DrawPoint(Vector2 point){
		Gizmos.DrawSphere(point, POINT_RADIUS);
	}

	//凸包の描画
	public void DrawConvex(){
		if (_convexPoints == null) {
			return;
		}
		for (int i = 0; i < _convexPoints.Count - 1; i++) {
			Debug.DrawLine (_convexPoints[i], _convexPoints[i+1]);
		}
		Debug.DrawLine (_convexPoints[_convexPoints.Count-1], _convexPoints[0]);
	}

	//点のソート用
	private int Compare(Vector2 a, Vector2 b){
		return a.x.CompareTo (b.x);
	}

	//サポート頂点の取得
	public Vector2 GetSupportPoint(Vector2 direction) {
		if (_convexPoints == null) {
			return Vector2.zero;
		}

		float max = Vector2.Dot(_convexPoints[0], direction);
		int ans = 0;
		for (int i = 1; i < _convexPoints.Count; i++) {
			float now = Vector2.Dot (_convexPoints [i], direction);
			if (max < now) {
				max = now;
				ans = i;
			}
		}
		return _convexPoints [ans];
	}

	//凸包の取得
	private void GetConvex(){
		_convexPoints.Clear ();
		_points.Sort (Compare);
		List<Vector2> testPoint = new List<Vector2> ();
		//上辺
		testPoint.Clear();
		_points.ForEach (point => {
			while (true) {
				int count = testPoint.Count;
				if (count < 2) {
					testPoint.Add (point);
					break;
				}
				Vector2 source = testPoint [count - 2];
				Vector2 destination = testPoint [count - 1];
				Vector2 edge = destination - source;
				Vector2 testEdge = point - source;
				if (Cross (edge, testEdge) <= 0) {
					testPoint.Add (point);
					break;
				} else {
					testPoint.RemoveAt (count-1);
				}
			}
		});
		//下辺
		_points.Reverse();
		_points.ForEach (point => {
			while (true) {
				int count = testPoint.Count;
				if (count < 2) {
					testPoint.Add (point);
					break;
				}
				Vector2 source = testPoint [count - 2];
				Vector2 destination = testPoint [count - 1];
				Vector2 edge = destination - source;
				Vector2 testEdge = point - source;
				if (Cross (edge, testEdge) <= 0) {
					testPoint.Add (point);
					break;
				} else {
					testPoint.RemoveAt (count-1);
				}
			}
		});
		for(int i=0; i<testPoint.Count-1; i++){
			_convexPoints.Add (testPoint[i]);
		}
	}

	private float Cross(Vector2 a, Vector2 b){
		return a.x*b.y-a.y*b.x;
	}
}
