using System.Collections.Generic;
using UnityEngine;

// =============================================
// UI management class / UI管理クラス
// =============================================
// EN: Manages showing/hiding UI across the whole game. Assumes all canvases and UI
//     elements are organized as children of an object tagged "UI".
//     Call SetUIGroup() whenever a new scene loads to rescan its UI hierarchy
//     (SceneChangeManager already does this automatically).
//
//     Show(canvasName)              -> shows an entire canvas
//     Hide(canvasName)              -> hides an entire canvas
//     Show(canvasName, uiName)      -> shows a single UI element within a canvas
//     Hide(canvasName, uiName)      -> hides a single UI element within a canvas
//
// JP: ゲーム全体のUIの表示・非表示を管理する。全てのキャンバスとUIは、
//     「UI」タグが付いたオブジェクトの子として構成されている前提で動作する。
//     新しいシーンが読み込まれるたびにSetUIGroup()を呼び出し、UI階層を再取得する
//     （SceneChangeManagerが自動的に呼び出す）。
//
//     Show(canvasName)              -> 指定のキャンバス全体を表示する
//     Hide(canvasName)              -> 指定のキャンバス全体を非表示にする
//     Show(canvasName, uiName)      -> キャンバス内の指定のUIのみを表示する
//     Hide(canvasName, uiName)      -> キャンバス内の指定のUIのみを非表示にする

// EN: Holds info about a single UI element / JP: 1つのUI要素の情報を保持する
class UIState
{
    public string name;
    public GameObject obj;
}

// EN: Holds info about a canvas and its child UI elements / JP: キャンバスとその子UI要素の情報を保持する
class CanvasState
{
    public List<UIState> uiStates;
    public GameObject obj;
}

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    private Transform uiGroup; // EN: the root object tagged "UI" / JP: 「UI」タグが付いたルートオブジェクト
    private Dictionary<string, CanvasState> canvasDictionary;

    protected override void Init()
    {
        SetUIGroup();
    }

    // EN: Rescans the current scene's UI hierarchy and rebuilds the lookup table.
    //     Must be called after every scene load.
    // JP: 現在のシーンのUI階層を再スキャンし、検索用の辞書を再構築する。
    //     シーンが読み込まれるたびに呼び出す必要がある。
    public void SetUIGroup()
    {
        GameObject uiRoot = GameObject.FindGameObjectWithTag("UI");

        if (uiRoot == null)
        {
            Debug.LogError("No UI object found in the current scene. / 現在のシーンにはUIが存在しません。");
            return;
        }

        uiGroup = uiRoot.transform;
        canvasDictionary = new Dictionary<string, CanvasState>();

        // EN: Walk through each canvas under "UI", then each UI element under each canvas.
        // JP: 「UI」の子であるキャンバスと、さらにその子であるUI要素を順に走査する。
        foreach (Transform canvas in uiGroup)
        {
            List<UIState> uiStateList = new List<UIState>();
            foreach (Transform ui in canvas)
            {
                UIState uiState = new UIState();
                uiState.name = ui.name;
                uiState.obj = ui.gameObject;
                uiStateList.Add(uiState);
            }

            CanvasState canvasState = new CanvasState();
            canvasState.uiStates = uiStateList;
            canvasState.obj = canvas.gameObject;

            canvasDictionary[canvas.name] = canvasState;
        }

        Debug.Log("Canvas/UI information collected. / キャンバス・UI情報を取得しました。");
    }

    // EN: Shows an entire canvas (and everything inside it). / JP: キャンバス全体（内部の全UI）を表示する。
    public void Show(string canvasName)
    {
        if (canvasDictionary.TryGetValue(canvasName, out var canvas))
        {
            canvas.obj.SetActive(true);
        }
        else
        {
            Debug.LogError("Canvas not found: " + canvasName + " / キャンバス:" + canvasName + "は存在しないキャンバスです。");
        }
    }

    // EN: Hides an entire canvas. / JP: キャンバス全体を非表示にする。
    public void Hide(string canvasName)
    {
        if (canvasDictionary.TryGetValue(canvasName, out var canvas))
        {
            canvas.obj.SetActive(false);
        }
        else
        {
            Debug.LogError("Canvas not found: " + canvasName + " / キャンバス:" + canvasName + "は存在しないキャンバスです。");
        }
    }

    // EN: Shows a single UI element within a canvas. The canvas itself must already be active.
    // JP: キャンバス内の指定のUIのみを表示する。キャンバス自体が非表示の場合は表示されない。
    public void Show(string canvasName, string uiName)
    {
        if (canvasDictionary.TryGetValue(canvasName, out var canvas))
        {
            if (!canvas.obj.activeSelf)
            {
                Debug.LogError("Canvas " + canvasName + " is inactive, cannot show UI: " + uiName +
                    " / キャンバス:" + canvasName + "が非表示のため、UI:" + uiName + "は表示できません。");
                return;
            }

            bool isFound = false;
            foreach (UIState ui in canvas.uiStates)
            {
                if (ui.name == uiName)
                {
                    ui.obj.SetActive(true);
                    isFound = true;
                }
            }

            if (!isFound)
            {
                Debug.LogError("UI not found: " + uiName + " in canvas " + canvasName +
                    " / UI:" + uiName + "はキャンバス:" + canvasName + "には存在しないUIです。");
            }
        }
        else
        {
            Debug.LogError("Canvas not found: " + canvasName + " / キャンバス:" + canvasName + "は存在しないキャンバスです。");
        }
    }

    // EN: Hides a single UI element within a canvas. / JP: キャンバス内の指定のUIのみを非表示にする。
    public void Hide(string canvasName, string uiName)
    {
        if (canvasDictionary.TryGetValue(canvasName, out var canvas))
        {
            bool isFound = false;
            foreach (UIState ui in canvas.uiStates)
            {
                if (ui.name == uiName)
                {
                    ui.obj.SetActive(false);
                    isFound = true;
                }
            }

            if (!isFound)
            {
                Debug.LogError("UI not found: " + uiName + " in canvas " + canvasName +
                    " / UI:" + uiName + "はキャンバス:" + canvasName + "には存在しないUIです。");
            }
        }
        else
        {
            Debug.LogError("Canvas not found: " + canvasName + " / キャンバス:" + canvasName + "は存在しないキャンバスです。");
        }
    }
}