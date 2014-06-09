using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Usi2usi
{
    public class UsiEngine : IDisposable
    {
        private const int ProcessExitWaitMs = 10000; // Exitするときの最大待ち 10秒値は暫定

        private Process engineProcess = null;

        private bool connected = false;

        public UsiEngine()
        {
        }

        // 接続
        public void Connect(string path)
        {
            this.engineProcess = new Process();

            this.engineProcess.StartInfo.FileName = path;
#if DEBUG
            this.engineProcess.StartInfo.CreateNoWindow = false;
#else
            this.engineProcess.StartInfo.CreateNoWindow = true;
#endif
            this.engineProcess.StartInfo.UseShellExecute = false;  // リダイレクトする場合はfalse
            this.engineProcess.StartInfo.RedirectStandardInput = true;
            this.engineProcess.StartInfo.RedirectStandardOutput = true;

            this.engineProcess.OutputDataReceived += this.process_DataRecieved;

            this.engineProcess.EnableRaisingEvents = true;  // プロセス終了を受け取るためにtrueにする
            this.engineProcess.Exited += this.process_Exited;    // 終了イベント

            this.engineProcess.Start();
            this.engineProcess.BeginOutputReadLine();

            this.connected = true;
        }

        /// <summary>
        /// 切断
        /// </summary>
        public void Disconnect()
        {
            if (this.connected)
            {
                this.Send("quit");
                this.connected = false;


                if (!this.engineProcess.WaitForExit(ProcessExitWaitMs))
                {
                    try
                    {
                        this.engineProcess.Kill(); // 強制終了させる
                    }
                    catch
                    {
                    }
                }

                this.engineProcess.Exited -= this.process_Exited;    // イベント削除 必要？

                this.engineProcess.Dispose();
                this.engineProcess = null;             
            }
        }

        // 送信
        public void Send(string str)
        {
            if (this.connected)
            {
                this.engineProcess.StandardInput.WriteLine(str);
                this.engineProcess.StandardInput.Flush();
            }
        }
       
        public void Dispose()
        {
            try
            {
                if (this.engineProcess != null)
                {
                    this.engineProcess.Dispose();
                    this.engineProcess = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// データの非同期受信
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="outLine"></param>
        private void process_DataRecieved(object sender, DataReceivedEventArgs outLine)
        {
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                Console.WriteLine(outLine.Data);
                Log.WriteLine(outLine.Data);
            }
        }

        /// <summary>
        /// プロセス終了時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void process_Exited(object sender, EventArgs e)
        {
        }
    }
}
