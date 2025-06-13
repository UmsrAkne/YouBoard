using System;
using System.Windows.Threading;

namespace YouBoard.Utils
{
    public class WorkTimer
    {
        private readonly DispatcherTimer timer;
        private DateTime startTime;
        private TimeSpan accumulatedTime;
        private bool isRunning;

        public WorkTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };

            timer.Tick += (_, _) => Tick?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 1秒ごとに更新される通知イベント
        /// </summary>
        public event EventHandler Tick;

        /// <summary>
        /// 現在までの経過時間
        /// </summary>
        public TimeSpan Elapsed => isRunning
            ? accumulatedTime + (DateTime.Now - startTime)
            : accumulatedTime;

        /// <summary>
        /// 現在の状態を取得します。
        /// </summary>
        public bool IsRunning => isRunning;

        /// <summary>
        /// タイマーを開始します。
        /// </summary>
        public void Start()
        {
            if (isRunning)
            {
                return;
            }

            startTime = DateTime.Now;
            timer.Start();
            isRunning = true;
        }

        /// <summary>
        /// タイマーを一時停止します。
        /// </summary>
        public void Pause()
        {
            if (!isRunning)
            {
                return;
            }

            accumulatedTime += DateTime.Now - startTime;
            timer.Stop();
            isRunning = false;
        }

        /// <summary>
        /// タイマーをリセットします。
        /// </summary>
        public void Reset()
        {
            timer.Stop();
            accumulatedTime = TimeSpan.Zero;
            isRunning = false;
        }
    }
}