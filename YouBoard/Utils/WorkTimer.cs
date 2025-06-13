using System;
using System.Windows.Threading;
using Prism.Mvvm;

namespace YouBoard.Utils
{
    public class WorkTimer : BindableBase
    {
        private readonly DispatcherTimer timer;
        private DateTime startTime;
        private TimeSpan accumulatedTime = TimeSpan.Zero;
        private bool isRunning;

        public WorkTimer()
        {
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };

            timer.Tick += (_, _) =>
            {
                Tick?.Invoke(this, EventArgs.Empty);
                RaisePropertyChanged(nameof(Elapsed));
            };
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
        public bool IsRunning { get => isRunning; set => SetProperty(ref isRunning, value); }

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
            IsRunning = true;
        }

        /// <summary>
        /// タイマーを一時停止します。
        /// </summary>
        public void Pause()
        {
            if (!IsRunning)
            {
                return;
            }

            accumulatedTime += DateTime.Now - startTime;
            timer.Stop();
            IsRunning = false;
        }

        /// <summary>
        /// タイマーをリセットします。
        /// </summary>
        public void Reset()
        {
            timer.Stop();
            accumulatedTime = TimeSpan.Zero;
            IsRunning = false;
        }
    }
}