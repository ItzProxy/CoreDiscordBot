using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core_Discord.CoreMusic
{
    public class CoreMusicQueue
    {
        private LinkedList<MusicInfo> Songs { get; set; } = new LinkedList<MusicInfo>(); //linked list
        private int _currIndex = 0;
        public int CurrIndex //get amd set current index on thread
        {
            get
            {
                return _currIndex;
            }
            set
            {
                lock (locker)
                {

                }
            }
        }
        private readonly object locker = new object(); // semaphore
        private TaskCompletionSource<bool> nextSource { get; } = new TaskCompletionSource<bool>(); //get next source after completeion
        /// <summary>
        /// returns current count of 
        /// </summary>
        public int Count
        {
            get
            {
                lock (locker)
                {
                    return Songs.Count;
                }
            }
        }
        private uint _maxQueueSize;
        public uint MaxQueueSize
        {
            get => _maxQueueSize;
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                lock (locker)
                {
                    _maxQueueSize = value;
                }
            }
        }
        public void Add(MusicInfo song)
        {
            
        }
    }
}
