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
    [SerializeField] private Text controllerModeText;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (controllerMode)
            {
                case (int) ControllerMode.Ball:
                    Transform tmpTransform = transform;
                    // tmpTransform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);;
                    // tmpTransform.rotation = transform.rotation;
                    for (int i = 0; i < 6; i++)
                    {
                        {
                            GameObject tmpBall =
                                GameObject.Instantiate(ballGameObject) as GameObject; //runcherbulletにbulletのインスタンスを格納
                            tmpBall.GetComponent<Rigidbody>().velocity =
                                transform.forward; // * 3; //アタッチしているオブジェクトの前方にbullet speedの速さで発射
                            tmpBall.transform.position = tmpTransform.position;
                            tmpBall.transform.rotation = tmpTransform.rotation;
                            //BallAdded();
                            tmpTransform.RotateAround(CenterAxisGameObject.transform.position, Vector3.up, 360 / 6);
                        }
                    }
                    break;
                case (int) ControllerMode.BlackHole:
                    GameObject tmpBlackHole =
                        GameObject.Instantiate(blackHoleGameObject) as GameObject; //runcherbulletにbulletのインスタンスを格納
                    // tmpBlackHole.GetComponent<Rigidbody>().velocity = transform.forward;// * 3; //アタッチしているオブジェクトの前方にbullet speedの速さで発射
                    tmpBlackHole.transform.position = transform.position;
                    BlackHoleAdded(tmpBlackHole);
                    break;
                case (int) ControllerMode.WhiteHole:
                    GameObject tmpWhiteHole =
                        GameObject.Instantiate(whiteHoleGameObject) as GameObject; //runcherbulletにbulletのインスタンスを格納
                    // tmpWhiteHole.GetComponent<Rigidbody>().velocity = transform.forward;// * 3; //アタッチしているオブジェクトの前方にbullet speedの速さで発射
                    tmpWhiteHole.transform.position = transform.position;
                    WhiteHoleAdded(tmpWhiteHole);
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            controllerMode++;
            controllerMode %= 3;
            switch (controllerMode)
            {
                case (int) ControllerMode.Ball:
                    controllerModeText.text = "Ball";
                    break;
                case (int) ControllerMode.BlackHole:
                    controllerModeText.text = "BlackHole";
                    break;
                case (int) ControllerMode.WhiteHole:
                    controllerModeText.text = "WhiteHole";
                    break;
            }
        }
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
                switch (controllerMode)
                {
                    case (int) ControllerMode.Ball:
                        for (int i = 0; i < 6; i++)
                        {
                            Transform tmpTransform = transform;
                            GameObject tmpBall =
                                GameObject.Instantiate(ballGameObject) as GameObject; //runcherbulletにbulletのインスタンスを格納
                            tmpBall.GetComponent<Rigidbody>().velocity =
                                transform.forward; // * 3; //アタッチしているオブジェクトの前方にbullet speedの速さで発射
                            tmpBall.transform.position = transform.position;
                            tmpBall.transform.rotation = transform.rotation;
                            //BallAdded();
                            tmpTransform.RotateAround(CenterAxisGameObject.transform.position,Vector3.up, 360/6);
                        }

                        break;
                    case (int) ControllerMode.BlackHole:
                        GameObject tmpBlackHole =
                            GameObject.Instantiate(blackHoleGameObject) as GameObject; //runcherbulletにbulletのインスタンスを格納
                        // tmpBlackHole.GetComponent<Rigidbody>().velocity = transform.forward;// * 3; //アタッチしているオブジェクトの前方にbullet speedの速さで発射
                        tmpBlackHole.transform.position = transform.position;
                        BlackHoleAdded(tmpBlackHole);
                        break;
                    case (int) ControllerMode.WhiteHole:
                        GameObject tmpWhiteHole =
                            GameObject.Instantiate(whiteHoleGameObject) as GameObject; //runcherbulletにbulletのインスタンスを格納
                        // tmpWhiteHole.GetComponent<Rigidbody>().velocity = transform.forward;// * 3; //アタッチしているオブジェクトの前方にbullet speedの速さで発射
                        tmpWhiteHole.transform.position = transform.position;
                        WhiteHoleAdded(tmpWhiteHole);
                        break;
                    case (int) ControllerMode.CenterAxis:
                        CenterAxisGameObject.transform.position = transform.position;
                        break;
                }

                break;

            case MLInput.Controller.Button.HomeTap:
                Application.Quit();
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
        controllerMode++;
        controllerMode %= controllerModeMax;
        switch (controllerMode)
        {
            case (int) ControllerMode.Ball:
                controllerModeText.text = "Ball";
                break;
            case (int) ControllerMode.BlackHole:
                controllerModeText.text = "BlackHole";
                break;
            case (int) ControllerMode.WhiteHole:
                controllerModeText.text = "WhiteHole";
                break;
            case (int) ControllerMode.CenterAxis:
                controllerModeText.text = "CenterAxis";
                break;
        }
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
        SceneManager.LoadScene("MagicLeapArFoundationReferencePoints");

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
}