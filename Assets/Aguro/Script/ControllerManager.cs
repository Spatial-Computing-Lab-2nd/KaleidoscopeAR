using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] MLInput.Controller controller;
    [SerializeField] private GameObject ballGameObject;
    [SerializeField] private GameObject blackHoleGameObject;
    [SerializeField] private GameObject whiteHoleGameObject;
    [SerializeField] private GameObject CenterAxisGameObject;

    public delegate void AddBallEventHandler();

    public delegate void AddBlackHoleEventHandler(GameObject blackHoleGameObject);

    public delegate void AddWhiteHoleEventHandler(GameObject whiteHoleGameObject);

    public event AddBallEventHandler BallAdded;
    public event AddBlackHoleEventHandler BlackHoleAdded;
    public event AddWhiteHoleEventHandler WhiteHoleAdded;
    [SerializeField] private TextMesh controllerModeText;
    private float controllerModeTextAlpha;

    private int resetCount;
    private int resetCountMax = 120;

    enum ControllerMode
    {
        Ball,
        BlackHole,
        WhiteHole,
        CenterAxis
    }
    private int controllerModeMax = 4;

    private int controllerMode;

    void Start()
    {
        // コントローラの入力を有効にし,対応するイベントハンドラを登録する.
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerButtonUp += OnButtonUp;

        MLInput.OnTriggerDown += OnTriggerDown;
        MLInput.OnTriggerUp += OnTriggerUp;

        MLInput.OnControllerTouchpadGestureStart += OnTouchPadGestureStart;
        MLInput.OnControllerTouchpadGestureContinue += OnTouchPadGestureContinue;
        MLInput.OnControllerTouchpadGestureEnd += OnTouchPadGestureEnd;

        //MoveBallのBlackHoleAddedイベントを追加しなければならないため、最初にボールを出現させてエラーを回避しています。
        //この書き方は良くないです。
        GameObject tmpBall = Instantiate(ballGameObject, transform);
        Destroy(tmpBall,0.1f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateEffect(0.5f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeEffect();
        }

        if (controller.TriggerValue >= 0.1f)
        {
            GenerateEffect(controller.TriggerValue);
        }

        controllerModeTextAlpha = Mathf.Max(controllerModeTextAlpha - 0.01f, 0.0f);
        controllerModeText.color = new Color(1.0f,1.0f,1.0f,controllerModeTextAlpha);

    }

    void OnDestroy()
    {
        // コントローラの入力を無効にし,登録していたイベントハンドラを削除する.
        MLInput.Stop();
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;

        MLInput.OnTriggerDown -= OnTriggerDown;
        MLInput.OnTriggerUp -= OnTriggerUp;

        MLInput.OnControllerTouchpadGestureStart -= OnTouchPadGestureStart;
        MLInput.OnControllerTouchpadGestureContinue -= OnTouchPadGestureContinue;
        MLInput.OnControllerTouchpadGestureEnd -= OnTouchPadGestureEnd;
    }


    /// <summary>
    /// ボタン押下時の処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="button"></param>
    void OnButtonDown(
        byte controllerId,
        MLInput.Controller.Button button)
    {
        switch (button)
        {
            case MLInput.Controller.Button.Bumper:
                ChangeEffect();
                break;
            case MLInput.Controller.Button.HomeTap:
                //Application.Quit();
                break;
        }
    }


    /// <summary>
    /// ボタン押上時の処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="button"></param>
    void OnButtonUp(
        byte controllerId,
        MLInput.Controller.Button button)
    {
        switch (button)
        {
            case MLInput.Controller.Button.Bumper:
                break;

            case MLInput.Controller.Button.HomeTap:
                break;
        }
    }


    /// <summary>
    /// トリガーの押下処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="value"></param>
    void OnTriggerDown(
        byte controllerId,
        float value)
    {
        //GenerateEffect(value);

        // controllerMode++;
        // controllerMode %= controllerModeMax;
        // switch (controllerMode)
        // {
        //     case (int) ControllerMode.Ball:
        //         controllerModeText.text = "Ball";
        //         break;
        //     case (int) ControllerMode.BlackHole:
        //         controllerModeText.text = "BlackHole";
        //         break;
        //     case (int) ControllerMode.WhiteHole:
        //         controllerModeText.text = "WhiteHole";
        //         break;
        //     case (int) ControllerMode.CenterAxis:
        //         controllerModeText.text = "CenterAxis";
        //         break;
        //}
    }


    /// <summary>
    /// トリガーの押上処理.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="value"></param>
    void OnTriggerUp(
        byte controllerId,
        float value)
    {
    }


    /// <summary>
    /// タッチパッドのジェスチャー始点.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="gesture"></param>
    void OnTouchPadGestureStart(
        byte controllerId,
        MLInput.Controller.TouchpadGesture gesture)
    {
        //SceneManager.LoadScene("MagicLeapArFoundationReferencePoints");

        //resetCount = 0;
    }

    /// <summary>
    /// タッチパッドのジェスチャー操作中.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="gesture"></param>
    void OnTouchPadGestureContinue(
        byte controllerId,
        MLInput.Controller.TouchpadGesture gesture)
    {
        // resetCount++;
        // if (resetCount>=resetCountMax)
        // {
        //     SceneManager.LoadScene("MagicLeapArFoundationReferencePoints");
        // }
    }


    /// <summary>
    /// タッチパッドのジェスチャ終点.
    /// </summary>
    /// <param name="controllerId"></param>
    /// <param name="gesture"></param>
    void OnTouchPadGestureEnd(
        byte controllerId,
        MLInput.Controller.TouchpadGesture gesture)
    {
    }

    void GenerateEffect(float triggerPower)
    {
        switch (controllerMode)
        {
            case (int) ControllerMode.Ball:
                AddBall(triggerPower);
                break;
            case (int) ControllerMode.BlackHole:
                AddBlackHole();
                break;
            case (int) ControllerMode.WhiteHole:
                AddWhiteHole();
                break;
            case (int) ControllerMode.CenterAxis:
                var tmpTransform = transform;
                CenterAxisGameObject.transform.position = tmpTransform.position;
                CenterAxisGameObject.transform.rotation = tmpTransform.rotation;
                break;
        }
    }

    void ChangeEffect()
    {
        controllerMode++;
        controllerMode %= controllerModeMax;
        switch (controllerMode)
        {
            case (int) ControllerMode.Ball:
                controllerModeText.text = "エフェクトの種類\n流れ星";
                break;
            case (int) ControllerMode.BlackHole:
                controllerModeText.text = "エフェクトの種類\nブラックホール";
                break;
            case (int) ControllerMode.WhiteHole:
                controllerModeText.text = "エフェクトの種類\nホワイトホール";
                break;
            case (int) ControllerMode.CenterAxis:
                controllerModeText.text = "エフェクトの種類\n万華鏡の中心軸を配置する";
                break;
        }
        controllerModeTextAlpha = 1.0f;
    }

    void AddBall(float triggerValue)
    {
        for (int i = 0; i < 6; i++)
        {
            Transform tmpTransform = transform;
            GameObject tmpBall =
                GameObject.Instantiate(ballGameObject, CenterAxisGameObject.transform, true) as GameObject; //runcherbulletにbulletのインスタンスを格納
            tmpBall.GetComponent<Rigidbody>().velocity =
                transform.forward * (5 * triggerValue);
            tmpBall.transform.position = tmpTransform.position;
            tmpBall.transform.rotation = tmpTransform.rotation;
            //BallAdded();
            tmpTransform.RotateAround(CenterAxisGameObject.transform.position, CenterAxisGameObject.transform.up, 360 / 6);
            Destroy(tmpBall, 10.0f);
        }
    }

    void AddBlackHole()
    {
        for (int i = 0; i < 6; i++)
        {
            Transform tmpTransform = transform;
            GameObject tmpBlackHole =
                GameObject.Instantiate(blackHoleGameObject) as GameObject;
            tmpBlackHole.transform.parent = CenterAxisGameObject.transform;
            tmpBlackHole.transform.position = transform.position;
            BlackHoleAdded(tmpBlackHole);
            tmpBlackHole.transform.position = transform.position;
            tmpBlackHole.transform.rotation = transform.rotation;
            tmpTransform.RotateAround(CenterAxisGameObject.transform.position, CenterAxisGameObject.transform.up, 360 / 6);
            Destroy(tmpBlackHole, 20.0f);
        }
    }

    void AddWhiteHole()
    {
        for (int i = 0; i < 6; i++)
        {
            Transform tmpTransform = transform;
            GameObject tmpWhiteHole =
                GameObject.Instantiate(whiteHoleGameObject) as GameObject;
            tmpWhiteHole.transform.parent = CenterAxisGameObject.transform;
            tmpWhiteHole.transform.position = transform.position;
            WhiteHoleAdded(tmpWhiteHole);
            tmpWhiteHole.transform.position = transform.position;
            tmpWhiteHole.transform.rotation = transform.rotation;
            tmpTransform.RotateAround(CenterAxisGameObject.transform.position, CenterAxisGameObject.transform.up, 360 / 6);
            Destroy(tmpWhiteHole, 20.0f);
        }
    }
    
    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}