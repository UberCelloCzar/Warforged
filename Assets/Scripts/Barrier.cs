//Remove this define to ignore this class.
#define Unity
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if Unity
namespace Warforged
{
    //This was used from Stack Overflow
    //http://stackoverflow.com/questions/6889837/how-to-implement-barrier-class-from-net-4-functionality-in-net-3-5
    public sealed class Barrier
    {
        public int mNumThreads;
        private ManualResetEvent[] mEventsA;
        private ManualResetEvent[] mEventsB;
        private ManualResetEvent[] mEventsC;
        private ManualResetEvent[] mEventsBootStrap;
        private Object mLockObject;
        private int[] mCounter;
        private int mCurrentThreadIndex = 0;

        public Barrier(int numThreads)
        {
            this.mNumThreads = numThreads;

            this.mEventsA = new ManualResetEvent[this.mNumThreads];
            this.mEventsB = new ManualResetEvent[this.mNumThreads];
            this.mEventsC = new ManualResetEvent[this.mNumThreads];
            this.mEventsBootStrap = new ManualResetEvent[this.mNumThreads];
            this.mCounter = new int[this.mNumThreads];
            this.mLockObject = new Object();

            for (int i = 0; i < this.mNumThreads; i++)
            {
                this.mEventsA[i] = new ManualResetEvent(false);
                this.mEventsB[i] = new ManualResetEvent(false);
                this.mEventsC[i] = new ManualResetEvent(false);
                this.mEventsBootStrap[i] = new ManualResetEvent(false);
                this.mCounter[i] = 0;
            }
        }

        /// <summary>
        /// Adds a new thread to the gate system.
        /// </summary>
        /// <returns>Returns a thread ID for this thread, to be used later when waiting.</returns>
        public int AddThread()
        {
            lock (this.mLockObject)
            {
                this.mEventsBootStrap[this.mCurrentThreadIndex].Set();
                this.mCurrentThreadIndex++;
                return this.mCurrentThreadIndex - 1;
            }
        }

        /// <summary>
        /// Stop here and wait for all the other threads in the NThreadGate. When all the threads have arrived at this call, they
        /// will unblock and continue.
        /// </summary>
        /// <param name="myThreadID">The thread ID of the caller</param>
        public void SignalAndWait(int myThreadID)
        {
            // Make sure all the threads are ready.
            WaitHandle.WaitAll(this.mEventsBootStrap);

            // Rotate between three phases.
            int phase = this.mCounter[myThreadID];
            if (phase == 0)        // Flip
            {
                this.mEventsA[myThreadID].Set();
                WaitHandle.WaitAll(this.mEventsA);
                this.mEventsC[myThreadID].Reset();
            }
            else if (phase == 1)    // Flop
            {
                this.mEventsB[myThreadID].Set();
                WaitHandle.WaitAll(this.mEventsB);
                this.mEventsA[myThreadID].Reset();
            }
            else    // Floop
            {
                this.mEventsC[myThreadID].Set();
                WaitHandle.WaitAll(this.mEventsC);
                this.mEventsB[myThreadID].Reset();
                this.mCounter[myThreadID] = 0;
                return;
            }

            this.mCounter[myThreadID]++;
        }
    }
}
#endif
