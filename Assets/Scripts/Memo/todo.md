- 会話スキップ時の遅延を作る
- 2つめ以降が何故か表示されていない



# What is this?

- C# Console app

# やること

- echo $PS1の設定を変えることで、自分の表示名を隠す。
    - default: %n@%m %1~ %#
- mesh cannnot more than 65000? => stringの行数で制御すれば良いのでは? => コマンド毎にまとめた方が良いのでは?
- exitせずに終わった時のプロセスどうなってる？きちんと終了されてる？

# ???
- zsh -l VS zsh -i
- -iにすべき!!その場合、出力を「途中でも表示」にすべき!! (program の scanfなどが効かないため)(もしくは、PTYをgoでやる)

# TODOリスト
- アバター追加＆表情＆背景の色、history
- 改行の恐らく「canvasforceupdate」のタイミングがあってない
- 「Shell built-in command」など「zsh -l」でできていない?コマンドの実装、またはできない場合はできないのメッセージ
    - history, export, など
- 「mysql -u root -p」の「Enter your password...」などはどうやって表示すれば良いのだろう。。。=>DataReceivedEventは改行(など)検知。そのため改行ない場合の「入力待ち状態」においてRead&Outputしてやる
- \m81などの色付け, bboollddなどのbold, 
- historyが上手くいかない。man historyで確認してみたら、、、うーん、、、、、＜＝ターミあるで行っているboldなどのマークアップを置換すべし
- フォルダ名補完はC#でやろう？ 

# やりたいこと

- ツンデレ音声など入れたい
- タブで入力補完(zcompdump?)
- ギャルゲー風（イラスト or 3Dモデル）
- cdに対してファイル一覧表示などのコマンド補助
- サジェスト機能のコマンド入力補助。ヘルプ。コマンド説明。
- よく使うコマンドの予測、保存機能(StreamingAssetsにtxtファイルとして保存するなど)
- 画面サイズを変えても文字の大きさを一定にする。
- 重さ軽減
- 自分で表示サイズ、色、絵、3Dモデル、などをカスタマイズ可能。
- ターミナルへのより深い理解(シェル実行の仕組み、ttyとは？、lsの時のあの見やすさはどうやって、など！)
- ttyで実行とかってできるのかな？
- mac,linux,windowsでテスト
- 1行ごとではなく、StandardOutputが来る毎に文字表示させたい


# 余裕がありまくったらやる

- coloringやbold(エスケープシーケンス,)
- customでシェル実行し、表示名など「このアプリケーション内のみに用いる設定」を作る
- 複数のシェル起動
- フォルダ構成など整理
- zcompdumpでタブ補完している？？？
- ctrl+c(interrupt process)nados
- どうやってsignalを送ったらいいのか? goでやるか??

# sudo -l

/bin/zsh> sudo -l 
sudo: a terminal is required to read the password; either use the -S option to read from standard input or configure an askpass helper

# zsh -l
terminallでは/etc/zshrc実行されるが、プロセス(zsh -l)では実行されない ~> $HISTFILEなどがない


# man history の実行結果(terminal)
```
BUILTIN(1)                BSD General Commands Manual               BUILTIN(1)

NAME
     builtin, !, %, ., :, @, {, }, alias, alloc, bg, bind, bindkey, break,
     breaksw, builtins, case, cd, chdir, command, complete, continue, default,
     dirs, do, done, echo, echotc, elif, else, end, endif, endsw, esac, eval,
     exec, exit, export, false, fc, fg, filetest, fi, for, foreach, getopts,
     glob, goto, hash, hashstat, history, hup, if, jobid, jobs, kill, limit,
     local, log, login, logout, ls-F, nice, nohup, notify, onintr, popd,
     printenv, pushd, pwd, read, readonly, rehash, repeat, return, sched, set,
     setenv, settc, setty, setvar, shift, source, stop, suspend, switch,
     telltc, test, then, time, times, trap, true, type, ulimit, umask,
     unalias, uncomplete, unhash, unlimit, unset, unsetenv, until, wait,
     where, which, while -- shell built-in commands

SYNOPSIS
     builtin [-options] [args ...]

DESCRIPTION
     Shell builtin commands are commands that can be executed within the run-
     ning shell's process.  Note that, in the case of csh(1) builtin commands,
     the command is executed in a subshell if it occurs as any component of a
:
```


# escape

文字列内または文字列外のいずれの場合も、次のエスケープシーケンスが認識されます。

\ a
ベル文字

\ b
バックスペース

\ e、\ E
逃れる

\ f
フォームフィード

\ n
改行（改行）

\ r
キャリッジリターン

\ t
水平タブ

\ v
垂直タブ

\ NNN
8進数の文字コード

\ x NN
16進数の文字コード

\ u NNNN
16進数のUnicode文字コード

\ U NNNNNNNN
16進数のUnicode文字コード

\ M [ - ] X
メタビットが設定された文字

\ C [ - ] X
制御文字

^ X
制御文字

それ以外の場合はすべて、「\」は次の文字をエスケープします。削除は ' ^？'。' \ M ^？'と' ^ \ M？'は同じではなく、（emacsとは異なり）、バインディング' \ M- X 'と' \ e X 'は完全に区別されますが、' bindkey-m 'によって同じバインディングに初期化されます。



# 14.7ファイル名の拡張
各単語は、引用符で囲まれていない ' 〜 'で始まるかどうかを確認するためにチェックされます。含まれている場合は、「/」までの単語、または「/」がない場合は単語の終わりがチェックされ、ここで説明する方法のいずれかで置換できるかどうかが確認されます。その場合、「〜」とチェックされた部分は適切な代替値に置き換えられます。

' 〜 '自体は、$ HOMEの値に置き換えられます。' 〜 'の後に ' + 'または ' - 'が続くと、それぞれ現在または以前の作業ディレクトリに置き換えられます。

' 〜 'の後に数字が続く場合は、ディレクトリスタック内のその位置にあるディレクトリに置き換えられます。' 〜0 'は ' 〜+ 'と同等であり、 ' 〜1 'はスタックの最上位です。' 〜+ 'の後に数字が続くと、ディレクトリスタック内のその位置にあるディレクトリに置き換えられます。「〜+ 0「と等価です〜+」「及び〜+ 1が」スタックの最上位です。' 〜- 'の後に数字が続く場合は、スタックの最下位から多くの位置にあるディレクトリに置き換えられます。' 〜-0 'はスタックの一番下です。 PUSHD_MINUSの オプション取引所の効果〜+ 'と「〜 -」彼らは数が続いています。



# tty???
- SSH? SerialPort?
