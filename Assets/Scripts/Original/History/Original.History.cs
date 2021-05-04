using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// History
// UIで制御したい
public partial class Original : MonoBehaviour
{
    //public MyHistory MyHistories=new MyHistory();
    public class MyHistory
    {
        public int nowHistLine = 0; //LOGに表示する行
        public int latestHistLine = 0; //新たに出力する行

        public int histLineMax;
        public MyHistoryUnit[] histories;

        //Constructor
        public MyHistory(int histLineMax=100)
        {
            this.histories = new MyHistoryUnit[histLineMax];
            this.histLineMax = histLineMax;
        }

        //セットする
        public void Set(string text)
        {
            //textを改行コードなどで分割し、それに応じてSetCommand,SetResultを実行するのもありだけど、ひとまずは実装していない
        }
        public void SetCommand(string command)
        {
            histories[nowHistLine].Command += command+"\n";
        }
        public void SetResultSuccess(string result)
        {
            histories[nowHistLine].Result += "["+ (int)ResultType.Success +"]" + result+"\n";
        }
        public void SetResultError(string result)
        {
            histories[nowHistLine].Result += "[" + (int)ResultType.Success +"]" + result + "\n";
        }
        public void NewHistLine()
        {
            if (latestHistLine == histLineMax-1)
            {
                //これ以上作れないため、改行したを警告
                MyHistoryUnit[] new_histories = new MyHistoryUnit[histLineMax];
                System.Array.Copy(histories, new_histories, histLineMax - 1);
                histories = new_histories;
                histories[nowHistLine].Result += "[" + (int)ResultType.MyWarning + "]" + "保存できる履歴がマックスに達したため、最初の履歴が削除されました。\n";
            }
            else
            {
                latestHistLine++;
            }
        }

        public void Clear()
        {
            latestHistLine = 0;
            histories = new MyHistoryUnit[histLineMax];
        }

        //match 文字を含むHistoryを検索し、そのHistLineを返す
        public int Find_History(string match)
        {
            for(int i = nowHistLine; i <=0; i--)
            {
                if(histories[i].Command.Contains(match) || histories[i].Result.Contains(match) || histories[i].ResultError.Contains(match))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    //inputをCommandにするのではなく、実行するものをCommandにしたい。
    //現在は...できていないのだが、、、
    public class MyHistoryUnit
    {
        public string Command;
        public string Result; //毎行、先頭に「[ResultType]」がかかれる
    }
    public enum ResultType
    {
        Success,
        Error,
        MyWarning
    }

}
