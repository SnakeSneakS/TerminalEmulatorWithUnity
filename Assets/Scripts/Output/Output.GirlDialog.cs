using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

//Output like a dialog(chatting) with Girl 'TERMINAL CHAN'
public partial class Output: MonoBehaviour
{
    [SerializeField] public GameObject DialogDisplayPanel;
    [SerializeField] public Text DialogDisplayCommandText;
    [SerializeField] public Text DialogDisplayResultText;
    [SerializeField] public Text DialogDisplayResultText_ForTest;
    [SerializeField] public Text DialogDisplayNameText;
    [SerializeField] public Button DialogNextButton;

    [SerializeField] public Text TerminalChanFaceText;

    public static TerminalChan terminalChan;

    private void GirlDialog_Awake()
    {
        terminalChan = TerminalChan.NewTerminalChan(this,myHistory);

        //次に進める
        DialogNextButton.onClick.AddListener(() =>
        {
            //UnityEngine.Debug.Log("NEXT");
            terminalChan.ShowNext();
        });
    }

    //コマンド実行時
    public void GirlDialog_ExecuteCommand()
    {
        terminalChan.WhenExecute();
    }
    //成功出力時
    public void GirlDialog_Success()
    {
        terminalChan.WhenSuccess();
    }
    //失敗出力時
    public void GirlDialog_Error()
    {
        terminalChan.WhenError();
    }
    //スキップ時
    public void GirlDialog_WhenSkipButtonInput()
    {
        terminalChan.ShowNext();
    }

    
    public class TerminalChan: MonoBehaviour
    {
        public static string MyName = "You";
        public static string TerminalChanName = "TerminalChan";
        private static int CommandLengthMax=25;
        private static int NameLengthMax=15;
        private static int ResultLineLengthMax=4;
        private static float ShowThroughWaitTime = 0.05f; //一文字ずつ表示する際の、一文字表示にかかる時間
        private static float ShowThroughAutoNextTime = 1.0f; //表示し終わった際の、次ページへ自動で移る時間(特に4行以上)
        private static float ShowNextWaitTime = 0.5f; //一括表示した後、次ページ表示にするまでの禁止時間。連続クリックにより必要以上に移動することを防ぐ

        public static string DisplayCommand="";
        public static string DisplayResult="";
        public static string DisplayName=TerminalChanName;
        public static int DisplayResult_LastLineIndex = -1;

        
        public static bool IsShowThroughing = false;
        public static bool IsAbleToShowNext = true;

        public Original.MyHistory myHistory;
        public Output output;
        public Reaction reaction;

        //Like a constructor, return TerminalChan. Here you can use "Monobehavior"
        public static TerminalChan NewTerminalChan(Output output,Original.MyHistory myHistory)
        {
            TerminalChan terminalChan;
            terminalChan = new GameObject().AddComponent<TerminalChan>();//new TerminalChan(this, myHistory);
            terminalChan.output = output;
            terminalChan.myHistory = myHistory;
            terminalChan.reaction = Reaction.NewTerminalChan(output);
            return terminalChan;
        }

        //Constructor
        /*
        public  TerminalChan(Output output, Original.MyHistory myHistory)
        {
            this.myHistory = myHistory;
            this.output = output;
            this.reaction = new Reaction(output.TerminalChanFaceText);

            UnityEngine.Debug.LogError(this.reaction);

            if(output.DialogDisplayCommandText==null || output.DialogDisplayNameText==null || output.DialogDisplayPanel==null || output.DialogDisplayResultText==null||output.DialogDisplayResultText_ForTest==null || output.DialogNextButton == null || output.TerminalChanFaceText==null)
            {
                UnityEngine.Debug.LogError("No reference...");
            }
        }
        */

        public void WhenExecute()
        {
            ShowNew();
            reaction.WhenExecute();
        }
        public void WhenSuccess()
        {
            ShowNow();
            reaction.WhenSuccess();
        }
        public void WhenError()
        {
            ShowNow();
            reaction.WhenError();
        }
        public void WhenSkipButtonInput()
        {
            ShowNext();
        }


        //1つのコマンドを表示. command実行時に用いる
        public void ShowNew()
        {
            DisplayResult_LastLineIndex = -1;

            DisplayCommand = myHistory.histories[myHistory.writeHistLine].Command_Uncolored;
            if (DisplayCommand.Length > CommandLengthMax) DisplayCommand = DisplayCommand.Substring(0, CommandLengthMax)+"...";
            //DisplayCommand = ColoringLine(DisplayCommand,LogDisplayColor.Green);
            UpdateText(output.DialogDisplayCommandText, DisplayCommand,true);

            if (DisplayName.Length > NameLengthMax) DisplayName = DisplayName.Substring(0, NameLengthMax) + "...";
            UpdateText(output.DialogDisplayNameText, DisplayName,false);

            DisplayResult = "";
            StopAllCoroutines();
            TotalManager.dispatcher.Invoke(() =>
            {
                ShowResultOnce();
            });
        }
        //Resultの最初ページのResultを表示. 新たに出力されたときに実行
        public void ShowNow()
        {
            TotalManager.dispatcher.Invoke(() =>
            {
                SetDisplayResultNow();
                StartCoroutine(ShowResultThrough());
            });
        }
        //1つのコマンドの中で、Enterなどにより出力の次のページへ行くor順に出力するのを一斉出力にすること
        public void ShowNext()
        {
            //連続表示している場合、一括表示する。その後、一定時間次へ移れなくする
            if (IsShowThroughing)
            {
                UnityEngine.Debug.Log("Next... ShowOnce.");
                TotalManager.dispatcher.Invoke(() =>
                {
                    ShowResultOnce();
                });

                StartCoroutine(ForbitShowNextForWhile(false));
            }
            //次へ移れない場合、映らない
            else if (!IsAbleToShowNext)
            {
                UnityEngine.Debug.Log("Unable To Show Next.");
            }
            else
            {
                UnityEngine.Debug.Log("Next... NextResult.");
                TotalManager.dispatcher.Invoke(() =>
                {
                    SetDisplayResultNext();

                    StartCoroutine( ShowResultThrough() );
                });
            }
        }
        //一斉に出力を表示する. ShowResultThroughの後に呼び出す
        public void ShowResultOnce()
        {
            IsShowThroughing = false;
            StopCoroutine(ShowResultThrough());
            output.DialogDisplayResultText.text = DisplayResult;

            UnityEngine.Debug.Log("DIALOG RESULT as showOnece: "+DisplayResult);
        }
        //順次出力を表示する
        public IEnumerator ShowResultThrough()
        {
            if (IsShowThroughing) { UnityEngine.Debug.Log("ShowReultThroughSuspended"); yield break; }

            UnityEngine.Debug.Log("START RESULT THROUGH");

            IsShowThroughing = true;

            //colorTagを処理しながら一文字ずつ表示する
            //例: <color=#ffffffff>ShellFileName: /bin/zsh</color>\n
            MatchCollection TagGroups = Regex.Matches(DisplayResult, "<color=#........>.*</color>");
            string tempDisplayResult = "";

            output.DialogDisplayResultText.text="";
            for (int i = 0; i < TagGroups.Count; i++)
            {
                string[] texts = Regex.Split(TagGroups[i].Value, "<color=#........>|</color>");
                string text = (texts.Length > 1) ? texts[1] : "";
                string tag = Regex.Match(TagGroups[i].Value, "<color=#........>").Value;
                //UnityEngine.Debug.Log("text: "+text); UnityEngine.Debug.Log("tag: "+tag);
                for (int j = 0; j < text.Length; j++)
                {
                    if (!IsShowThroughing) yield break; 
                    output.DialogDisplayResultText.text = tempDisplayResult + tag + text.Substring(0, j+1) + "</color>" ;
                    //UnityEngine.Debug.Log("DIALOG RESULT as showThrough: " + tempDisplayResult + tag + text.Substring(0, j + 1) + "</color>");
                    yield return new WaitForSeconds(ShowThroughWaitTime);
                }
                tempDisplayResult += TagGroups[i].Value + "\n";

                //DisplayResultが変わった場合のアップデータ処理
                TagGroups= Regex.Matches(DisplayResult, "<color=#........>.*</color>");
            }

            if(TagGroups.Count!=0) StartCoroutine(ForbitShowNextForWhile(true));
            IsShowThroughing = false;
            yield break;
        }
        //同じpageとしてDisplayResultを設定
        public void SetDisplayResultNow()
        {
            string[] results = myHistory.histories[myHistory.writeHistLine].Result_Colored.Split('\n');

            for (int i = DisplayResult_LastLineIndex+1; i < results.Length - 1 ; i++)
            {
                string t = DisplayResult + results[i] + "\n";
                
                output.DialogDisplayResultText_ForTest.text = t;
                Canvas.ForceUpdateCanvases();
                //UnityEngine.Debug.Log("LINE COUNT: "+ output.DialogDisplayResultText_ForTest.cachedTextGenerator.lines.Count);
                if (output.DialogDisplayResultText_ForTest.cachedTextGenerator.lines.Count > ResultLineLengthMax)
                {
                    output.DialogDisplayResultText_ForTest.text = "";
                    DisplayResult_LastLineIndex = i - 1;
                    break;
                }
                else
                {
                    output.DialogDisplayResultText_ForTest.text = "";
                    DisplayResult_LastLineIndex = i;
                    DisplayResult += results[i] + "\n";
                }
                
            }

            UnityEngine.Debug.Log("DIALOG RESULT as first: " + DisplayResult);
        }
        //２番以降としてDisplayResultを設定
        public void SetDisplayResultNext()
        {
            string[] results = myHistory.histories[myHistory.writeHistLine].Result_Colored.Split('\n');

            //resultのうち表示する最後の行(最後の行がIsNullOrEmptyの場合、それは最後の行とせずに１行戻る)
            //int last = (string.IsNullOrEmpty(results[results.Length - 1]) ? results.Length - 2 : results.Length-1);
            int last = results.Length - 1;

            //既に最後まで表示していた場合はDisplayResultを変更しない
            //if (DisplayResult_LastLineIndex == last) return;

            DisplayResult = "";
            for (int i = DisplayResult_LastLineIndex + 1; i <= last; i++)
            {
                output.DialogDisplayResultText_ForTest.text = DisplayResult + results[i] + "\n";
                Canvas.ForceUpdateCanvases();
                //UnityEngine.Debug.Log("LINE COUNT: " + output.DialogDisplayResultText_ForTest.cachedTextGenerator.lines.Count);
                if (output.DialogDisplayResultText_ForTest.cachedTextGenerator.lines.Count > ResultLineLengthMax)
                {
                    output.DialogDisplayResultText_ForTest.text = "";
                    DisplayResult_LastLineIndex = i - 1;
                    UnityEngine.Debug.Log("DIALOG RESULT as next max: " + DisplayResult);
                    break;
                }
                else
                {
                    output.DialogDisplayResultText_ForTest.text = "";
                    DisplayResult_LastLineIndex = i;
                    DisplayResult += results[i] + "\n";
                    UnityEngine.Debug.Log("DIALOG RESULT as next: " + DisplayResult);
                }
            }
        }
        //Textのtをsに変える。メインスレッドで実行
        public void UpdateText(Text t,string s, bool isForceUpdate=false)
        {
            TotalManager.dispatcher.Invoke(() =>
            {
                t.text = s;
                if (isForceUpdate)
                {
                    Canvas.ForceUpdateCanvases();
                    t.enabled = false;
                    t.enabled = true; //一度消して付けないと、ContentSizeFilterが働かない
                }
            });
        }
        //一定時間次へ移るのを禁止する. その後に次に移る場合show=trueにする
        private IEnumerator ForbitShowNextForWhile(bool show)
        {
            IsAbleToShowNext = false;
            yield return new WaitForSeconds(ShowNextWaitTime);
            IsAbleToShowNext = true;
            if (show)
            {
                yield return new WaitForSeconds(ShowThroughAutoNextTime);
                ShowNext();
            }
            yield break;
        }



        //Class TerminalChan.Reaction
        public class Reaction:MonoBehaviour
        {
            public static ReactionType nowReactionType = ReactionType.Idle;

            public static int koukando = 0;
            public static int koukandoPerCommand = 0;
            public static int koukando_ResultSuccess = 1;
            public static int koukando_ResultError = -5;

            public static float reactionDelayTime = 1.0f; //コマンド実行してからリアクションするまでの時間

            public Text faceText;
            //public GameObject gameObject;
            //public Animator animator;

            public static Dictionary<ReactionType, string[]> reactionFaceDictionary = new Dictionary<ReactionType, string[]>()
            {
                {ReactionType.Sad, new string[]{ "( T-T) ｳﾙｳﾙ","。゜(つω｀）゜。", ">_<" } },
                {ReactionType.Angry, new string[]{ "(*･ε･*)ﾑｰ", "＼(○｀ε´○)ｺﾗ!ｺﾗ!", "ヽ(｀Д´#)ﾉ", "╰(◣﹏◢)╯", "(๑˘･з･˘)" } },
                {ReactionType.Idle, new string[]{"(^_^)", "(^Ｏ^)" } },
                {ReactionType.Happy, new string[]{ "ヾ(*´∀｀*)ﾉ", "(*´∇`*)", "(*^o^*)", "(〃▽〃)", "( *¯ ꒳¯*)" } },
                {ReactionType.Dere, new string[]{ "|_-。) ポッ", "（///ω///）", "【照//∀//】","(//∇//) テレテレ","（*´ｪ｀*）ﾎﾟｯ" } },
            };

            //数字は境値の好感度
            public enum ReactionType
            {
                Sad = -10,  
                Angry = -5, 
                Idle =0,
                Happy=1, 
                Dere =10, 
            }


            /// <summary>  return TerminalChan.Reaction with "Monobehavior" </summary>
            /// <param name="output"> instance of class "Output" </param>
            public static Reaction NewTerminalChan(Output output)
            {
                Reaction reaction= new GameObject().AddComponent<Reaction>();
                reaction.faceText = output.TerminalChanFaceText;
                return reaction;
            }
            //constructor
            /*
            public Reaction(Text faceText,GameObject gameObject=null,Animator animator=null) {
                this.faceText = faceText;
            }
            */

            //コマンドexecute時に実行
            public void WhenExecute()
            {
                StopAllCoroutines();
                StartCoroutine(ReactPerCommand());
            }
            //success時の好感度書き換え
            public void WhenSuccess()
            {
                koukandoPerCommand += koukando_ResultSuccess;
            }
            //error時の好感度書き換え
            public void WhenError()
            {
                koukandoPerCommand += koukando_ResultError;
            }

            //コマンド毎に実行。初期化後、ある程度時間が経過してから、それまでのエラーメッセージ、サクセスメッセージによりリアクションタイプを決めてリアクションする。
            public IEnumerator ReactPerCommand()
            {
                koukandoPerCommand = 0;
                React_NewFace_Faceless();
                yield return new WaitForSeconds(reactionDelayTime);
                //この間にkoukandoPerCommandの値が書き変わっていく
                React_NewFace(koukandoPerCommand);
                yield break;
            }

            //顔を変える(リアクション
            public void React_NewFace(int koukandoPerCommand)
            {
                //UnityEngine.Debug.Log("koukandoPerCommand: "+koukandoPerCommand);
                string[] faces=null;
                if (koukandoPerCommand <= (int)ReactionType.Sad) reactionFaceDictionary.TryGetValue(ReactionType.Sad, out faces);
                else if (koukandoPerCommand <= (int)ReactionType.Angry) reactionFaceDictionary.TryGetValue(ReactionType.Angry, out faces);
                else if (koukandoPerCommand >= (int)ReactionType.Happy) reactionFaceDictionary.TryGetValue(ReactionType.Happy, out faces);
                else if (koukandoPerCommand >= (int)ReactionType.Dere) reactionFaceDictionary.TryGetValue(ReactionType.Dere, out faces);
                else reactionFaceDictionary.TryGetValue(ReactionType.Idle, out faces);

                ;
                faceText.text = ( (faces==null)?"":faces[Random.Range(0,faces.Length)] );
            }
            //顔を変える(顔なし
            public void React_NewFace_Faceless()
            {
                faceText.text = "";
            }
            //アニメーションを変える
            public void React_NewAnim()
            {

            }
            
        }

        
    }

}
