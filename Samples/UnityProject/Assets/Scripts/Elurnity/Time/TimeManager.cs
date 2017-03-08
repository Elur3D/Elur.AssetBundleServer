
using System;
using System.Collections;
using System.Collections.Generic;

namespace Elurnity.TimeManager
{
    public interface ITickUpdate
    {
        bool Update(float time);
    }

    public class TickUpdate : ITickUpdate
    {
        public Func<float, bool> keepWaiting;

        public bool Update(float time)
        {
            return keepWaiting == null || keepWaiting(time);
        }
    }

    public class Coroutine : ITickUpdate
    {
        private IEnumerator coroutine;

        public Coroutine(IEnumerator coroutine)
        {
            this.coroutine = coroutine;
        }

        public bool Update(float time)
        {
            return coroutine.MoveNext();
        }
    }

    public interface ITimeManager
    {
        void Add(ITickUpdate obj, bool weak = true);
        void Remove(ITickUpdate obj);
    }

    public class TimeManager
    {
        protected WeakList<ITickUpdate> weakUpdate = new WeakList<ITickUpdate>();
        protected List<ITickUpdate> hardUpdate = new List<ITickUpdate>();

        public void Update(float deltaTime)
        {
            for (int i = weakUpdate.Count - 1; i >= 0; i--)
            {
                var update = weakUpdate[i];
                if (update == null || !update.Update(deltaTime))
                {
                    weakUpdate.RemoveAt(i);
                }
            }

            for (int i = hardUpdate.Count - 1; i >= 0; i--)
            {
                var update = hardUpdate[i];
                if (update == null || !update.Update(deltaTime))
                {
                    hardUpdate.RemoveAt(i);
                }
            }
        }

        public void Add(ITickUpdate obj, bool weak = true)
        {
            if (weak)
            {
                weakUpdate.Add(obj);
            }
            else
            {
                hardUpdate.Add(obj);
            }
        }

        public void Add(IEnumerator coroutine)
        {
            hardUpdate.Add(new Coroutine(coroutine));
        }

        public void Remove(ITickUpdate obj)
        {
            weakUpdate.Remove(obj);
            hardUpdate.Remove(obj);
        }

        public int Count
        {
            get
            {
                return weakUpdate.Count + hardUpdate.Count;
            }
        }
    }
}
