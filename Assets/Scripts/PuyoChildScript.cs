using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoChildScript : MonoBehaviour
{
    GameObject[] puyos; //取得したPuyoを格納しておく為の配列
    float[] puyoX = new float[100]; //取得したPuyoのx座標の情報を格納しておく為の配列
    float[] puyoY = new float[100]; //取得したPuyoのy座標の情報を格納しておく為の配列
    GameObject parent;
    PuyoParentScript puyoParentScript;

    public bool canFall = true; //Puyoが自由落下できるか否か

    void Start()
    {
        puyos = GameObject.FindGameObjectsWithTag("Puyo");  //Hierarcy内の"Puyo"とタグのついているGameObjectを全て取得→配列puyosの中に格納
        parent = this.gameObject.transform.parent.gameObject;
        puyoParentScript = parent.GetComponent<PuyoParentScript>();

        for(int i = 0; i < puyos.Length; i++){ //配列puyosの要素数だけfor回す
            puyoX[i] = Mathf.RoundToInt(puyos[i].transform.position.x * 10.0f)/10.0f; //丸め誤差解消しつつ配列puyox内に情報を格納
            puyoY[i] = Mathf.RoundToInt(puyos[i].transform.position.y * 10.0f)/10.0f; //丸め誤差解消しつつ配列puyoy内に情報を格納
        }
    }

    void Update()
    {
        if(transform.root.gameObject == gameObject){ //親がいない → 自分の親が自分自身の場合

            if(puyoParentScript.isHorizontal == false) return; //Puyoの並びが縦の時 → 処理離脱
            if(canFall == false) return; //Puyoの位置関係確認 → 自由落下できない状態であれば処理離脱

            //丸め誤差解消
            float nowX = Mathf.RoundToInt(transform.position.x * 10.0f)/10.0f; //10x → 小数第一位切り捨て → 10で割る → 目的の値取得
            float nowY = Mathf.RoundToInt(transform.position.y * 10.0f)/10.0f; //10x → 小数第一位切り捨て → 10で割る → 目的の値取得

            // 落下判定①(地面上)
            if(nowY == -6.5f){ 
                canFall = false; //canFallのフラグ切り替え
                return;
            }

            // 落下判定②(Puyo上)
            for(int i = 0; i < puyos.Length; i++){ //配列puyosの要素数だけfor回す
                if(nowX == puyoX[i] && nowY == puyoY[i] + 1.0f){
                    canFall = false; //canFallのフラグ切り替え
                    return; //処理離脱
                }
            }

            //条件満たしてなければ下に移動
            transform.Translate(0, -1.0f, 0, Space.World); //下に1マス移動
        }
    }
}
