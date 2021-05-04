package main

//https://qiita.com/ike_dai/items/cc718a0e2d7c0849b008

import (
	"bufio"
	"bytes"
	"flag"
	"fmt"
	"io"
	"log"
	"os"
	"os/exec"
	"os/signal"
	"syscall"

	"github.com/kr/pty"
)

// scanner split custom (for split \r treating as new line)
func customScan(data []byte, atEOF bool) (advance int, token []byte, err error) {
	if atEOF && len(data) == 0 {
		return 0, nil, nil
	}
	var i int
	if i = bytes.IndexByte(data, '\n'); i >= 0 {
		return i + 1, dropCR(data[0:i]), nil
	}
	if i = bytes.IndexByte(data, '\r'); i >= 0 {
		return i + 1, data[0:i], nil
	}
	if atEOF {
		return len(data), dropCR(data), nil
	}
	return 0, nil, nil
}

// dropCR drops a terminal \r from the data.
func dropCR(data []byte) []byte {
	if len(data) > 0 && data[len(data)-1] == '\r' {
		return data[0 : len(data)-1]
	}
	return data
}

//https://github.com/creack/pty
func execShell(shell string) error {
	// Create arbitrary command.
	c := exec.Command(shell)

	// Start the command with a pty.
	ptmx, err := pty.Start(c)
	if err != nil {
		return err
	}
	defer func() { _ = ptmx.Close() }()

	// Handle pty size.
	ch := make(chan os.Signal, 1)
	signal.Notify(ch, syscall.SIGWINCH)
	go func() {
		for range ch {
			if err := pty.InheritSize(os.Stdin, ptmx); err != nil {
				log.Printf("error resizing pty: %s", err)
			}
		}
	}()
	ch <- syscall.SIGWINCH // Initial resize.

	// Set stdin in raw mode.
	/*
		oldState, err := terminal.MakeRaw(int(os.Stdin.Fd()))
		if err != nil {
			panic(err)
		}
		defer func() { _ = terminal.Restore(int(os.Stdin.Fd()), oldState) }() // Best effort.
	*/

	//ここまで

	// Copy stdin to the pty and the pty to stdout.
	// NOTE: The goroutine will keep reading until the next keystroke before returning.
	go func() {
		_, _ = io.Copy(ptmx, os.Stdin)
	}()
	//_, _ = io.Copy(os.Stdout, ptmx)*/

	/*
		// 標準入力の内容をptyで起動したbashに引き渡すのと、stdinのバッファに複製して取得
		stdin := io.TeeReader(os.Stdin, ptmx)
		in_scanner := bufio.NewScanner(stdin)
		go func() { // Scan処理は基本的に処理を待つので入力この処理はgo routineでバックで回す必要あり
			// 標準入力の情報をScanしてそのまま標準出力する
			in_scanner.Split(customScan)
			for in_scanner.Scan() {
				text := in_scanner.Text()
				fmt.Printf("[in_scanner]%s\r\n", text)
			}
		}()
	*/

	// bashの実行結果の標準出力・標準エラー出力の内容をos.Stdout(コマンドラインの標準出力)に引き渡すのと、stdoutのバッファに複製して取得
	stdout := io.TeeReader(ptmx, os.Stdout)
	out_scanner := bufio.NewScanner(stdout)
	out_scanner.Split(customScan)
	for out_scanner.Scan() {
		text := out_scanner.Text()
		fmt.Printf("[out_scanner]%s\r\n", text) //fmt.Printf("[out_scanner]> %s\r\n", text)
	}

	return nil
}

func getArg0() string {
	flag.Parse()
	args := flag.Args()
	if len(args) != 1 {
		log.Fatal("\x1b[31mArgument 0 missing: the name of shell to execute...\x1b[0m")
	}
	return args[0]
}

func main() {
	shell := getArg0()
	if err := execShell(shell); err != nil {
		log.Fatal(err)
	}
}
