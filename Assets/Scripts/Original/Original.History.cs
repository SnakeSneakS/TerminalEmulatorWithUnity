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
        public int displayHistLine = 0; //LOGに表示する行
        public int writeHistLine = 0; //新たに出力する行

        public int HISTSIZE;
        public MyHistoryUnit[] histories=new MyHistoryUnit[100];

        //Constructor
        public MyHistory(int HISTSIZE=100)
        {
            this.HISTSIZE = HISTSIZE;
            this.histories = new MyHistoryUnit[HISTSIZE];
            for(int i = 0; i < HISTSIZE; i++) this.histories[i] = new MyHistoryUnit();
        }

        //セットする
        public void Set(string text)
        {
            //textを改行コードなどで分割し、それに応じてSetCommand,SetResultを実行するのもありだけど、ひとまずは実装していない
        }
        public void SetCommand(string command)
        {
            histories[writeHistLine].Command_Uncolored += command +"\n";
            histories[writeHistLine].Command_Colored += Output.ColoringLine(command, Output.LogDisplayColor.Green) + "\n";
        }
        public void SetResultSuccess(string result)
        {
            histories[writeHistLine].Result_Uncolored += result+"\n";
            histories[writeHistLine].Result_Colored += Output.ColoringLine(result, Output.LogDisplayColor.White) + "\n";
        }
        public void SetResultError(string result)
        {
            histories[writeHistLine].Result_Uncolored += result + "\n";
            histories[writeHistLine].Result_Colored += Output.ColoringLine(result, Output.LogDisplayColor.Red) + "\n";
        }
        public void SetMyWarning(string result)
        {
            histories[writeHistLine].Result_Uncolored += result + "\n";
            histories[writeHistLine].Result_Colored += Output.ColoringLine(result, Output.LogDisplayColor.Orange) + "\n";
        }
        public void NewHistLine()
        {
            if (writeHistLine == HISTSIZE-1)
            {
                //これ以上作れないため、一番最初を削除したことを警告
                MyHistoryUnit[] new_histories = new MyHistoryUnit[HISTSIZE];
                System.Array.Copy(histories, 1, new_histories, 0, HISTSIZE - 1);
                new_histories[writeHistLine] = new MyHistoryUnit();
                new_histories[writeHistLine].Result_Uncolored += "保存できる履歴がマックスに達したため、最初の履歴が削除されました。\"Clear\"をクリックで履歴を削除できます、多分...\n";
                new_histories[writeHistLine].Result_Colored += Output.ColoringLine("保存できる履歴がマックスに達したため、最初の履歴が削除されました。\"Clear\"をクリックで履歴を削除できます、多分...\n", Output.LogDisplayColor.Orange);
                histories = new_histories;
            }
            else
            {
                writeHistLine++;
            }
        }
        //表示する行と新たに書く行を一致させる
        public void setDisplayLineToWriteLine()
        {
            displayHistLine = writeHistLine;
        }

        public void Clear()
        {
            writeHistLine = 0;
            displayHistLine = 0;
            histories = new MyHistoryUnit[HISTSIZE];
            for(int i = 0; i < HISTSIZE; i++)
            {
                histories[i] = new MyHistoryUnit();
            }
        }

        //match 文字を含むHistoryを検索し、そのHistLineを返す。nowHistLineより前
        /*
        public int Find_History(string match)
        {
            for(int i = displayHistLine-1; i >= 0; i--)
            {
                if(histories[i].Command_Uncolored.Contains(match) || histories[i].Result_Uncolored.Contains(match))
                {
                    return i;
                }
            }
            return -1;
        }
        */
    }

    

    //inputをCommandにするのではなく、実行するものをCommandにしたい。
    //現在は...できていないのだが、、、
    public class MyHistoryUnit
    {
        public string Command_Uncolored="";
        public string Command_Colored = "";
        public string Result_Uncolored=""; 
        public string Result_Colored = "";
    }
    public enum ResultType
    {
        Command,
        Success,
        Error,
        MyWarning
    }

}
