using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core_Discord.CoreExtensions;
using static Core_Discord.CoreMusic.CoreMusicExceptions;
using System.Linq;

namespace Core_Discord.CoreMusic
{
    public class CoreMusicQueue : IDisposable
    {
        private LinkedList<MusicInfo> Songs { get; set; } = new LinkedList<MusicInfo>(); //linked list
        private int _currIndex = 0;
        private readonly object locker = new object(); // semaphore
        private TaskCompletionSource<bool> nextSource { get; } = new TaskCompletionSource<bool>(); //get next source after completeion
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
                    if(Songs.Count == 0)
                    {
                        _currIndex = 0;
                    }
                    else
                    {
                        _currIndex = value %= Songs.Count;
                    }
                }
            }
        }
        public (int Index, MusicInfo Song) Current
        {
            get
            {
                var cur = CurrIndex;
                return (cur, Songs.ElementAtOrDefault(cur));
            }
        }
       
        /// <summary>
        /// returns current count in list
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
        /// <summary>
        /// Add song to end of list
        /// </summary>
        /// <param name="song"></param>
        public void Add(MusicInfo song)
        {
            song.ThrowIfNull(nameof(song));
            lock (locker)
            {
                if(MaxQueueSize != 0 && Songs.Count >= MaxQueueSize)
                {
                    throw new QueueFullException();
                }
                Songs.AddLast(song);    
            }
        }
        /// <summary>
        /// add song to be next played
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        public int AddNext(MusicInfo song)
        {
            song.ThrowIfNull(nameof(song));
            lock (locker)
            {
                if(MaxQueueSize !=0 && Songs.Count >= MaxQueueSize)
                {
                    throw new QueueFullException();
                }
                var CurrSong = Current.Song;
                if (CurrSong == null)
                {
                    Songs.AddLast(song);
                    return Songs.Count;
                }
                var songlist = Songs.ToList();
                songlist.Insert(CurrIndex + 1, song);
                Songs = new LinkedList<MusicInfo>(songlist);
                return CurrIndex + 1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">value in between</param>
        /// <returns>Song at the front of the queue</returns>
        public MusicInfo RemoveAt(int index)
        {
            lock (locker)
            {
                if (index < 0 || index >= Songs.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var current = Songs.First.Value;
                for (int i = 0; i < Songs.Count; i++)
                {
                    if (i == index)
                    {
                        current = Songs.ElementAt(index);
                        Songs.Remove(current);
                        if (CurrIndex != 0)
                        {
                            if (CurrIndex >= index)
                            {
                                --CurrIndex;
                            }
                        }
                        break;
                    }
                }
                return current;
            }
        }
        /// <summary>
        /// Skips song
        /// </summary>
        /// <param name="skipCount"></param>
        public void Next(int skipCount = 1)
        {
            lock (locker)
            {
                CurrIndex += skipCount;
            }
        }
        /// <summary>
        /// Dispose method calls clear
        /// </summary>
        public void Dispose()
        {
            Clear();
        }
        public (int CurrIndex, MusicInfo[] Songs) ToArray()
        {
            lock (locker)
            {
                return (CurrIndex, Songs.ToArray());
            }
        }
        /// <summary>
        /// Destroys all nodes in Songs linked list and resets index to 0
        /// </summary>
        public void Clear()
        {
            lock (locker)
            {
                Songs.Clear();
                CurrIndex = 0;
            }
        }
        /// <summary>
        /// Picks a random song from queue between 0 and the max count of the list
        /// </summary>
        public void Random()
        {
            lock (locker)
            {
                CurrIndex = new Random().Next(Songs.Count);
            }
        }
        /// <summary>
        /// move song to index b and removes index a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public MusicInfo MoveSong(int a, int b)
        {
            lock (locker)
            {
                var currentSong = Current.Song;
                var playlist = Songs.ToList();
                if(a >= playlist.Count || b >= playlist.Count || a == b)
                {
                    return null;
                }

                var temp = playlist[a];
                playlist.RemoveAt(a);
                playlist.Insert(b, temp);

                Songs = new LinkedList<MusicInfo>(playlist);

                if(currentSong != null)
                {
                    CurrIndex = playlist.IndexOf(currentSong);
                }

                return temp;
            }
        }

        public void RemoveSong(MusicInfo song)
        {
            lock (locker)
            {
                Songs.Remove(song);
            }
        }

        public bool IsLast()
        {
            lock (locker)
                return CurrIndex == Songs.Count - 1;
        }
    }
}
