using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoParentScript : MonoBehaviour
{
    GameObject[] puyos; //取得したPuyoを格納しておく為の配列
    float[] puyoX = new float[100]; //取得したPuyoのx座標の情報を格納しておく為の配列
    float[] puyoY = new float[100]; //取得したPuyoのy座標の情報を格納しておく為の配列

    float span = 1.0f; // 何秒毎に処理するか
    float delta = 0.0f; // timer

    public bool isHorizontal = false; //Puyoの向き(true：横, false：縦)
    bool canRotate = false; //Puyoの回転できるか否か(true：可能, false：不可能)
    int rotateCount = 0; //回転させた回数(偶数：縦, 奇数：横)

    void Start()
    {
        puyos = GameObject.FindGameObjectsWithTag("Puyo");  //Hierarcy内の"Puyo"とタグのついているGameObjectを全て取得→配列puyosの中に格納

        for(int i = 0; i < puyos.Length; i++){ //配列puyosの要素数だけfor回す
            puyoX[i] = Mathf.RoundToInt(puyos[i].transform.position.x * 10.0f)/10.0f; //丸め誤差解消しつつ配列puyox内に情報を格納
            puyoY[i] = Mathf.RoundToInt(puyos[i].transform.position.y * 10.0f)/10.0f; //丸め誤差解消しつつ配列puyoy内に情報を格納
        }
    }

    void Update()
    {
        // 下に自動移動
        delta += Time.deltaTime;
        if (delta > span){
            transform.Translate(0, -1.0f, 0, Space.World); //『必ずWorld座標に対して』下に移動
            delta = 0.0f; // timerリセット
            canRotate = true;
        }

        //丸め誤差解消
        float nowX = Mathf.RoundToInt(transform.position.x * 10.0f)/10.0f; //10x → 小数第一位切り捨て → 10で割る → 目的の値取得
        float nowY = Mathf.RoundToInt(transform.position.y * 10.0f)/10.0f; //10x → 小数第一位切り捨て → 10で割る → 目的の値取得
        // Debug.Log("nowX : " + nowX + " | nowY : " + nowY);

        // 左右移動
        float key = 0.0f;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) key = -1.0f; //左キーが押された時 → 変数keyに-1.0代入
        if (Input.GetKeyDown(KeyCode.RightArrow)) key = 1.0f; //右キーが押された時 → 変数keyに1.0代入
        transform.Translate(key, 0, 0, Space.World); 
    
        // 下に移動
        if (Input.GetKeyDown(KeyCode.DownArrow)){ //下キー押した時
            transform.Translate(0, -1.0f, 0, Space.World); //下に1マス移動
        }

        if(canRotate == true){
            Rotate(nowY);//回転の処理
        }

        Landing(nowX, nowY); //Puyo着地の処理
    }

    //Puyoの回転司る関数
    void Rotate(float nowY){
        // Puyo回転
        if (Input.GetKeyDown(KeyCode.Space)){ //spaceキー押した時
            transform.Rotate(0, 0, -90.0f); //90度回転
            rotateCount += 1;
            if(rotateCount % 2 == 0){ //roteteCountが偶数→縦向きの時
                isHorizontal = false;

                if((int)nowY != nowY){ //ParentのY座標が小数の時
                    if(rotateCount % 4 == 0){
                        this.transform.position += new Vector3(-0.5f, -0.5f, 0); //座標調整
                    }
                    else{
                        this.transform.position += new Vector3(0.5f, -0.5f, 0); //座標調整
                    }
                }
            }
            else{ //rotateCountが奇数→横向きの時
                isHorizontal = true;
                
                if((int)nowY == nowY){ //ParentのY座標が整数の時
                    if(rotateCount % 4 == 0){
                        this.transform.position += new Vector3(0.5f, 0.5f, 0); //座標調整
                    }
                    else{
                        this.transform.position += new Vector3(-0.5f, 0.5f, 0); //座標調整
                    }
                }
            }
        }
    }

    //Puyoの着地司る関数
    void Landing(float nowX, float nowY){
        if(isHorizontal == false){ //Puyoが縦向きの時

            // 下のPuyoの座標
            float nowChildX = nowX;
            float nowChildY = nowY - 0.5f;

            // 落下判定①-1(Puyoが縦並びの && 地面上)
            if(nowChildY == -6.5f) { //下のPuyoが基底面に達した時
                gameObject.transform.DetachChildren(); //親子関係を解除
                Destroy(this.gameObject); //親を削除

                Debug.Log("①-1");
                return; //処理離脱(Update自体から抜ける)
            }

            // 落下判定①-2(Puyoが縦並び && Puyo上)
            for(int i = 0; i < puyos.Length; i++){ //配列puyosの要素数だけ回す
                // Debug.Log("puyox[i] : " + puyoX[i] + " | puyoy[i] : " + puyoY[i]);
                if(nowChildX == puyoX[i] && nowChildY == puyoY[i] + 1.0f){ //いずれかのPuyoの直上に親要素が来た時
                    this.gameObject.transform.DetachChildren(); //親子関係解除
                    Destroy(this.gameObject); //親を削除

                    Debug.Log("①-2");
                    return; //処理離脱(Update自体から抜ける)
                }
            }
        }
        else{ //Puyoが横向きの時

            float[] nowChildX = new float[2];
            float[] nowChildY = new float[2];
            for(int i = 0; i < nowChildX.Length; i++){
                nowChildX[i] = nowX + 0.5f -1.0f * i;
                nowChildY[i] = nowY;
            }
            
            

            // 落下判定②-1(Puyoが横並び && 地面上)
            for(int i = 0; i < nowChildX.Length; i++){
                if(nowChildY[i] == -6.5f){
                    this.gameObject.transform.DetachChildren(); //親子関係解除
                    Destroy(this.gameObject); //親を削除

                    Debug.Log("②-1");
                    return; //処理離脱(Update自体から抜ける)
                }
            }
        
            // 落下判定②-2(Puyoが横並び && Puyo上)
            for(int i = 0; i < puyos.Length; i++){ //配列puyosの要素数だけ回す
                for(int j = 0; j < nowChildX.Length; j++){ //配列nowChildxの要素数だけfor回す
                    if(nowChildX[j] == puyoX[i] && nowChildY[j] == puyoY[i] + 1.0f){ //いずれかのPuyoの直上に親要素が来た時
                        this.gameObject.transform.DetachChildren(); //親子関係解除
                        Destroy(this.gameObject); //親を削除

                        Debug.Log("②-2");
                        return; //処理離脱(Update自体から抜ける)
                    }
                }
            }
        }
    }
}
