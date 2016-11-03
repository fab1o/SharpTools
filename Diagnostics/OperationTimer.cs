using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Fabio.SharpTools.Diagnostics
{
    public sealed class OperationTimer : IDisposable
    {
        private Int64 m_startTime = 0;
        private string m_text = null;
        private Int32 m_collectionCount = 0;

        public event EventHandler<OperationTimerEventArgs> OperationTimerElapsed;

        public OperationTimer(string text = null)
        {
            PrepareForOperation();

            m_text = text;

            m_collectionCount = GC.CollectionCount(0);

            // This should be the last statement in this
            // method to keep timing as accurate as possible
            m_startTime = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            //Console.WriteLine("{0,6:###.00} seconds (GCs={1,3}) {2}",
            //(Stopwatch.GetTimestamp() - m_startTime) /
            //(Double)Stopwatch.Frequency,
            //GC.CollectionCount(0) - m_collectionCount, m_text);

            OperationTimerEventArgs e = new OperationTimerEventArgs(m_startTime,
                (Stopwatch.GetTimestamp() - m_startTime) / (Double)Stopwatch.Frequency,
                m_text,
                GC.CollectionCount(0) - m_collectionCount);

            OnOperationTimerElapsed(e);
        }

        private static void PrepareForOperation()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void OnOperationTimerElapsed(OperationTimerEventArgs e)
        {
            // Copy a reference to the delegate field now into a temporary field for thread safety
            EventHandler<OperationTimerEventArgs> temp =
            Interlocked.CompareExchange(ref OperationTimerElapsed, null, null);
            // If any methods registered interest with our event, notify them
            if (temp != null) temp(this, e);
        }

    }

    public class OperationTimerEventArgs : EventArgs
    {
        private readonly Int64 m_startTime;
        private readonly Double m_timeElapsed;
        private readonly string m_text;
        private readonly Int32 m_collectionCount;

        public OperationTimerEventArgs(Int64 m_startTime, Double m_timeElapsed, string m_text, Int32 m_collectionCount)
        {
            this.m_startTime = m_startTime;
            this.m_timeElapsed = m_timeElapsed;
            this.m_text = m_text;
            this.m_collectionCount = m_collectionCount;
        }

        public Int64 StartTime { get { return m_startTime; } }

        public Double TimeElapsed { get { return m_timeElapsed; } }

        public string Text { get { return m_text; } }

        public Int32 CollectionCount { get { return m_collectionCount; } }


    }

}
