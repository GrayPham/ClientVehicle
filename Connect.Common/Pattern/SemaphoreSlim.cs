﻿// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
// 
// SemaphoreSlim.cs 
//
// <owner>[....]</owner> 
//
// A lightweight semahore class that contains the basic semaphore functions plus some useful functions like interrupt
// and wait handle exposing to allow waiting on multiple semaphores.
// 
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-


using Connect.Common.Contract;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

// The class will be part of the current System.Threading namespace 
namespace System.Pattern
{
    /// <summary> 
    /// Limits the number of threads that can access a resource or pool of resources concurrently.
    /// </summary>
    /// <remarks>
    /// <para> 
    /// The <see cref="SemaphoreSlim"> provides a lightweight semaphore class that doesn't
    /// use Windows kernel semaphores. 
    /// </see></para> 
    /// <para>
    /// All public and protected members of <see cref="SemaphoreSlim"> are thread-safe and may be used 
    /// concurrently from multiple threads, with the exception of Dispose, which
    /// must only be used when all other operations on the <see cref="SemaphoreSlim"> have
    /// completed.
    /// </see></see></para> 
    /// </remarks>
    [ComVisible(false)]
    //[HostProtection(Synchronization = true, ExternalThreading = true)]
    [DebuggerDisplay("Current Count = {m_currentCount}")]
    public class SemaphoreSlim : IDisposable
    {
        #region Private Fields

        // The semaphore count, initialized in the constructor to the initial value, every release call incremetns it 
        // and every wait call decrements it as long as its value is positive otherwise the wait will block.
        // Its value must be between the maximum semaphore value and zero 
        private volatile int m_currentCount;

        // The maximum semaphore value, it is initialized to Int.MaxValue if the client didn't specify it. it is used 
        // to check if the count excceeded the maxi value or not.
        private readonly int m_maxCount;

        // The number of waiting threads, it is set to zero in the constructor and increments before blocking the 
        // threading and decrements it back after that. It is used as flag for the release call to know if there are
        // waiting threads in the monitor or not. 
        private volatile int m_waitCount;

        // Dummy object used to in lock statements to protect the semaphore count, wait handle and cancelation 
        private object m_lockObj;

        // Act as the semaphore wait handle, it's lazily initialized if needed, the first WaitHandle call initialize it
        // and wait an release sets and resets it respectively as long as it is not null 
        private ManualResetEvent m_waitHandle;

        // No maximum constant 
        private const int NO_MAXIMUM = Int32.MaxValue;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the current count of the <see cref="SemaphoreSlim">. 
        /// </see></summary> 
        /// <value>The current count of the <see cref="SemaphoreSlim">.</see></value>
        public int CurrentCount
        {
            get { return m_currentCount; }
        }

        /// <summary>
        /// Returns a <see cref="T:System.Threading.WaitHandle"> that can be used to wait on the semaphore. 
        /// </see></summary> 
        /// <value>A <see cref="T:System.Threading.WaitHandle"> that can be used to wait on the
        /// semaphore.</see></value> 
        /// <remarks>
        /// A successful wait on the <see cref="AvailableWaitHandle"> does not imply a successful wait on
        /// the <see cref="SemaphoreSlim"> itself, nor does it decrement the semaphore's
        /// count. <see cref="AvailableWaitHandle"> exists to allow a thread to block waiting on multiple 
        /// semaphores, but such a wait should be followed by a true wait on the target semaphore.
        /// </see></see></see></remarks> 
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="SemaphoreSlim"> has been disposed.</see></exception>
        public WaitHandle AvailableWaitHandle
        {
            get
            {
                CheckDispose();

                // Return it directly if it is not null 
                if (m_waitHandle != null)
                    return m_waitHandle;

                //lock the count to avoid multiple threads initializing the handle if it is null
                lock (m_lockObj)
                {
                    if (m_waitHandle == null)
                    {
                        // The initial state for the wait handle is true if the count is greater than zero 
                        // false otherwise 
                        m_waitHandle = new ManualResetEvent(m_currentCount != 0);
                    }
                }
                return m_waitHandle;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreSlim"> class, specifying
        /// the initial number of requests that can be granted concurrently.
        /// </see></summary> 
        /// <param name="initialCount">The initial number of requests for the semaphore that can be granted
        /// concurrently. 
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="initialCount"> 
        /// is less than 0.</paramref></exception>
        public SemaphoreSlim(int initialCount)
            : this(initialCount, NO_MAXIMUM)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreSlim"> class, specifying 
        /// the initial and maximum number of requests that can be granted concurrently. 
        /// </see></summary>
        /// <param name="initialCount">The initial number of requests for the semaphore that can be granted 
        /// concurrently.
        /// <param name="maxCount">The maximum number of requests for the semaphore that can be granted
        /// concurrently.
        /// <exception cref="T:System.ArgumentOutOfRangeException"> <paramref name="initialCount"> 
        /// is less than 0. -or-
        /// <paramref name="initialCount"> is greater than <paramref name="maxCount">. -or- 
        /// <paramref name="maxCount"> is less than 0.</paramref></paramref></paramref></paramref></exception> 
        public SemaphoreSlim(int initialCount, int maxCount)
        {
            if (initialCount < 0 || initialCount > maxCount)
            {
                throw new ArgumentOutOfRangeException(
                    "initialCount", initialCount, GetResourceString("SemaphoreSlim_ctor_InitialCountWrong"));
            }

            //validate input 
            if (maxCount <= 0)
            {
                throw new ArgumentOutOfRangeException("maxCount", maxCount, GetResourceString("SemaphoreSlim_ctor_MaxCountWrong"));
            }

            m_maxCount = maxCount;
            m_lockObj = new object();
            m_currentCount = initialCount;
        }

        #endregion

        #region  Methods
        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim">. 
        /// </see></summary>
        /// <exception cref="T:System.ObjectDisposedException">The current instance has already been 
        /// disposed.</exception> 
        public void Wait()
        {
            // Call wait with infinite timeout
            Wait(Timeout.Infinite, new CancellationToken());
        }

        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim">, while observing a 
        /// <see cref="T:System.Threading.CancellationToken">. 
        /// </see></see></summary>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken"> token to 
        /// observe.
        /// <exception cref="T:System.OperationCanceledException"><paramref name="cancellationToken"> was
        /// canceled.</paramref></exception>
        /// <exception cref="T:System.ObjectDisposedException">The current instance has already been 
        /// disposed.</exception>
        public void Wait(CancellationToken cancellationToken)
        {
            // Call wait with infinite timeout
            Wait(Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim">, using a <see cref="T:System.TimeSpan"> to measure the time interval.
        /// </see></see></summary> 
        /// <param name="timeout">A <see cref="System.TimeSpan"> that represents the number of milliseconds 
        /// to wait, or a <see cref="System.TimeSpan"> that represents -1 milliseconds to wait indefinitely.
        ///  
        /// <returns>true if the current thread successfully entered the <see cref="SemaphoreSlim">;
        /// otherwise, false.</see></returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="timeout"> is a negative
        /// number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater 
        /// than <see cref="System.Int32.MaxValue">.</see></paramref></exception>
        public bool Wait(TimeSpan timeout)
        {
            // Validate the timeout
            Int64 totalMilliseconds = (Int64)timeout.TotalMilliseconds;
            if (totalMilliseconds < -1 || totalMilliseconds > Int32.MaxValue)
            {
                throw new System.ArgumentOutOfRangeException(
                    "timeout", timeout, GetResourceString("SemaphoreSlim_Wait_TimeoutWrong"));
            }

            // Call wait with the timeout milliseconds 
            return Wait((int)timeout.TotalMilliseconds, new CancellationToken());
        }

        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim">, using a <see cref="T:System.TimeSpan"> to measure the time interval, while observing a <see cref="T:System.Threading.CancellationToken">.
        /// </see></see></see></summary> 
        /// <param name="timeout">A <see cref="System.TimeSpan"> that represents the number of milliseconds 
        /// to wait, or a <see cref="System.TimeSpan"> that represents -1 milliseconds to wait indefinitely.
        ///  
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken"> to
        /// observe.
        /// <returns>true if the current thread successfully entered the <see cref="SemaphoreSlim">;
        /// otherwise, false.</see></returns> 
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="timeout"> is a negative
        /// number other than -1 milliseconds, which represents an infinite time-out -or- timeout is greater 
        /// than <see cref="System.Int32.MaxValue">.</see></paramref></exception> 
        /// <exception cref="System.OperationCanceledException"><paramref name="cancellationToken"> was canceled.</paramref></exception>
        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            // Validate the timeout
            Int64 totalMilliseconds = (Int64)timeout.TotalMilliseconds;
            if (totalMilliseconds < -1 || totalMilliseconds > Int32.MaxValue)
            {
                throw new System.ArgumentOutOfRangeException(
                    "timeout", timeout, GetResourceString("SemaphoreSlim_Wait_TimeoutWrong"));
            }

            // Call wait with the timeout milliseconds
            return Wait((int)timeout.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim">, using a 32-bit 
        /// signed integer to measure the time interval. 
        /// </see></summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite">(-1) to wait indefinitely.
        /// <returns>true if the current thread successfully entered the <see cref="SemaphoreSlim">;
        /// otherwise, false.</see></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"> is a 
        /// negative number other than -1, which represents an infinite time-out.</paramref></exception>
        public bool Wait(int millisecondsTimeout)
        {
            return Wait(millisecondsTimeout, new CancellationToken());
        }
        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="SemaphoreSlim">, 
        /// using a 32-bit signed integer to measure the time interval,
        /// while observing a <see cref="T:System.Threading.CancellationToken">. 
        /// </see></see></summary> 
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite">(-1) to
        /// wait indefinitely. 
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken"> to observe.
        /// <returns>true if the current thread successfully entered the <see cref="SemaphoreSlim">; otherwise, false.</see></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="millisecondsTimeout"> is a negative number other than -1,
        /// which represents an infinite time-out.</paramref></exception> 
        /// <exception cref="System.OperationCanceledException"><paramref name="cancellationToken"> was canceled.</paramref></exception>
        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            CheckDispose();

            // Validate input
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(
                    "totalMilliSeconds", millisecondsTimeout, GetResourceString("SemaphoreSlim_Wait_TimeoutWrong"));
            }

            cancellationToken.ThrowIfCancellationRequested();

            long startTimeTicks = 0;
            if (millisecondsTimeout != Timeout.Infinite && millisecondsTimeout > 0)
            {
                startTimeTicks = DateTime.UtcNow.Ticks;
            }

            bool lockTaken = false;

            //Register for cancellation outside of the main lock. 
            //NOTE: Register/deregister inside the lock can deadlock as different lock acquisition orders could
            //      occur for (1)this.m_lockObj and (2)cts.internalLock
            CancellationTokenRegistration cancellationTokenRegistration = cancellationToken.Register(s_cancellationTokenCanceledEventHandler, this);
            try
            {
                // Perf: first spin wait for the count to be positive, but only up to the first planned yield. 
                //       This additional amount of spinwaiting in addition 
                //       to Monitor.Enter()’s spinwaiting has shown measurable perf gains in test scenarios.
                // 
                SpinWait spin = new SpinWait();
                while (m_currentCount == 0 && !spin.NextSpinWillYield)
                {
                    spin.SpinOnce();
                }
                // entering the lock and incrementing waiters must not suffer a thread-abort, else we cannot 
                // clean up m_waitCount correctly, which may lead to deadlock due to non-woken waiters. 
                try { }
                finally
                {
                    Monitor.Enter(m_lockObj, ref lockTaken);
                    if (lockTaken)
                    {
                        m_waitCount++;
                    }
                }

                // If the count > 0 we are good to move on. 
                // If not, then wait if we were given allowed some wait duration
                if (m_currentCount == 0)
                {
                    if (millisecondsTimeout == 0)
                    {
                        return false;
                    }

                    // Prepare for the main wait... 
                    // wait until the count become greater than zero or the timeout is expired
                    if (!WaitUntilCountOrTimeout(millisecondsTimeout, startTimeTicks, cancellationToken))
                    {
                        return false;
                    }
                }

                // At this point the count should be greater than zero
                Contract.Assert(m_currentCount > 0);
                m_currentCount--;

                // Exposing wait handle which is lazily initialized if needed
                if (m_waitHandle != null && m_currentCount == 0)
                {
                    m_waitHandle.Reset();
                }
            }
            finally
            {
                // Release the lock
                if (lockTaken)
                {
                    m_waitCount--;
                    Monitor.Exit(m_lockObj);
                }

                // Unregister the cancellation callback. 
                cancellationTokenRegistration.Dispose();
            }

            return true;
        }

        /// <summary> 
        /// Local helper function, waits on the monitor until the monitor recieves signal or the
        /// timeout is expired
        /// </summary>
        /// <param name="millisecondsTimeout">The maximum timeout 
        /// <param name="startTimeTicks">The start ticks to calculate the elapsed time
        /// <param name="cancellationToken">The CancellationToken to observe. 
        /// <returns>true if the monitor recieved a signal, false if the timeout expired</returns> 
        private bool WaitUntilCountOrTimeout(int millisecondsTimeout, long startTimeTicks, CancellationToken cancellationToken)
        {
            int remainingWaitMilliseconds = Timeout.Infinite;

            //Wait on the monitor as long as the count is zero
            while (m_currentCount == 0)
            {
                // If cancelled, we throw. Trying to wait could lead to deadlock. 
                cancellationToken.ThrowIfCancellationRequested();

                if (millisecondsTimeout != Timeout.Infinite)
                {
                    remainingWaitMilliseconds = UpdateTimeOut(startTimeTicks, millisecondsTimeout);
                    if (remainingWaitMilliseconds <= 0)
                    {
                        // The thread has expires its timeout
                        return false;
                    }
                }
                // ** the actual wait ** 
                if (!Monitor.Wait(m_lockObj, remainingWaitMilliseconds))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Exits the <see cref="SemaphoreSlim"> once.
        /// </see></summary>
        /// <returns>The previous count of the <see cref="SemaphoreSlim">.</see></returns> 
        /// <exception cref="T:System.ObjectDisposedException">The current instance has already been
        /// disposed.</exception> 
        public int Release()
        {
            return Release(1);
        }

        /// <summary>
        /// Exits the <see cref="SemaphoreSlim"> a specified number of times. 
        /// </see></summary>
        /// <param name="releaseCount">The number of times to exit the semaphore. 
        /// <returns>The previous count of the <see cref="SemaphoreSlim">.</see></returns> 
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="releaseCount"> is less
        /// than 1.</paramref></exception> 
        /// <exception cref="T:System.Threading.SemaphoreFullException">The <see cref="SemaphoreSlim"> has
        /// already reached its maximum size.</see></exception>
        /// <exception cref="T:System.ObjectDisposedException">The current instance has already been
        /// disposed.</exception> 
        public int Release(int releaseCount)
        {
            CheckDispose();

            // Validate input 
            if (releaseCount < 1)
            {
                throw new ArgumentOutOfRangeException(
                    "releaseCount", releaseCount, GetResourceString("SemaphoreSlim_Release_CountWrong"));
            }

            lock (m_lockObj)
            {

                // If thre release count would result exceeding the maximum count throw SemaphoreFullException
                if (m_maxCount - m_currentCount < releaseCount)
                {
                    throw new SemaphoreFullException();
                }

                // Increment the count by the actual release count 
                m_currentCount += releaseCount;

                if (m_currentCount == 1 || m_waitCount == 1)
                {
                    Monitor.Pulse(m_lockObj);
                }
                else if (m_waitCount > 1)
                {
                    Monitor.PulseAll(m_lockObj);
                }


                // Exposing wait handle if it is not null
                if (m_waitHandle != null && m_currentCount - releaseCount == 0)
                {
                    m_waitHandle.Set();
                }
                return m_currentCount - releaseCount;
            }
        }
        public ResultInfo TInfo { get; set; }
        public int Release<T>(int releaseCount, ResultInfo data)
        {
            CheckDispose();
            TInfo = data;
            // Validate input 
            if (releaseCount < 1)
            {
                throw new ArgumentOutOfRangeException(
                    "releaseCount", releaseCount, GetResourceString("SemaphoreSlim_Release_CountWrong"));
            }

            lock (m_lockObj)
            {

                // If thre release count would result exceeding the maximum count throw SemaphoreFullException
                if (m_maxCount - m_currentCount < releaseCount)
                {
                    throw new SemaphoreFullException();
                }

                // Increment the count by the actual release count 
                m_currentCount += releaseCount;

                if (m_currentCount == 1 || m_waitCount == 1)
                {
                    Monitor.Pulse(m_lockObj);
                }
                else if (m_waitCount > 1)
                {
                    Monitor.PulseAll(m_lockObj);
                }


                // Exposing wait handle if it is not null
                if (m_waitHandle != null && m_currentCount - releaseCount == 0)
                {
                    m_waitHandle.Set();
                }

                return m_currentCount - releaseCount;
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of <see cref="SemaphoreSlim">. 
        /// </see></summary>
        /// <remarks> 
        /// Unlike most of the members of <see cref="SemaphoreSlim">, <see cref="Dispose()"> is not 
        /// thread-safe and may not be used concurrently with other members of this instance.
        /// </see></see></remarks> 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> 
        /// When overridden in a derived class, releases the unmanaged resources used by the
        /// <see cref="T:System.Threading.ManualResetEventSlim">, and optionally releases the managed resources. 
        /// </see></summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.
        /// <remarks> 
        /// Unlike most of the members of <see cref="SemaphoreSlim">, <see cref="Dispose(Boolean)"> is not
        /// thread-safe and may not be used concurrently with other members of this instance. 
        /// </see></see></remarks> 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_waitHandle != null)
                {
                    m_waitHandle.Close();
                    m_waitHandle = null;
                }
                m_lockObj = null;
            }
        }

        /// <summary>
        /// Helper function to measure and update the wait time 
        /// </summary>
        /// <param name="startTimeTicks"> The first time (in Ticks) observed when the wait started 
        /// <param name="originalWaitMillisecondsTimeout">The orginal wait timeoutout in milliseconds 
        /// <returns>The new wait time in milliseconds, -1 if the time expired</returns>
        private static int UpdateTimeOut(long startTimeTicks, int originalWaitMillisecondsTimeout)
        {
            // The function must be called in case the time out is not infinite
            Contract.Assert(originalWaitMillisecondsTimeout != Timeout.Infinite);

            long elapsedMilliseconds = (DateTime.UtcNow.Ticks - startTimeTicks) / TimeSpan.TicksPerMillisecond;

            // Check the elapsed milliseconds is greater than max int because this property is long 
            if (elapsedMilliseconds > int.MaxValue)
            {
                return 0;
            }

            // Subtract the elapsed time from the current wait time 
            int currentWaitTimeout = originalWaitMillisecondsTimeout - (int)elapsedMilliseconds; ;
            if (currentWaitTimeout <= 0)
            {
                return 0;
            }

            return currentWaitTimeout;
        }


        /// <summary> 
        /// Private helper method to wake up waiters when a cancellationToken gets canceled. 
        /// </summary>
        private static Action<object> s_cancellationTokenCanceledEventHandler = new Action<object>(CancellationTokenCanceledEventHandler);
        private static void CancellationTokenCanceledEventHandler(object obj)
        {
            SemaphoreSlim semaphore = obj as SemaphoreSlim;
            Contract.Assert(semaphore != null, "Expected a SemaphoreSlim");
            lock (semaphore.m_lockObj)
            {
                Monitor.PulseAll(semaphore.m_lockObj); //wake up all waiters. 
            }
        }

        /// <summary>
        /// Checks the dispose status by checking the lock object, if it is null means that object
        /// has been disposed and throw ObjectDisposedException 
        /// </summary>
        private void CheckDispose()
        {
            if (m_lockObj == null)
            {
                throw new ObjectDisposedException(null, GetResourceString("SemaphoreSlim_Disposed"));
            }
        }

        /// <summary>
        /// local helper function to retrieve the exception string message from the resource file 
        /// </summary> 
        /// <param name="str">The key string
        private static string GetResourceString(string str)
        {
            return "";//Environment.GetResourceString(str);
        }

        #endregion
    }
}

// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
// 
// SemaphoreSlim.cs 
//
// <owner>[....]</owner> 
//
// A lightweight semahore class that contains the basic semaphore functions plus some useful functions like interrupt
// and wait handle exposing to allow waiting on multiple semaphores.
// 
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-