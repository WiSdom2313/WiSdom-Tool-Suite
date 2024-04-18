using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;

namespace WiSdom.DesignPattern
{
    public class MessageBus
    {
        private static MessageBus _instance;
        public static MessageBus I => _instance ?? (_instance = new MessageBus());
        private readonly Dictionary<Type, object> _messagePools = new Dictionary<Type, object>();
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();
#if UNITY_EDITOR
        private readonly ConcurrentDictionary<Type, int> _messageCounts = new ConcurrentDictionary<Type, int>();
#endif
        private readonly object _lock = new object();

        private MessageBus() { }

        /// <summary>
        /// Get the object pool for a message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private ObjectPool<T> GetPool<T>() where T : class, new()
        {
            var type = typeof(T);
            if (!_messagePools.TryGetValue(type, out var pool))
            {
                pool = new ObjectPool<T>(
                    createFunc: () => new T(),
                    actionOnGet: (item) => { /* Reset các thuộc tính cần thiết của item ở đây, nếu cần */ },
                    actionOnRelease: ResetMessage,
                    actionOnDestroy: (item) => { /* Có thể log hoặc thực hiện bước cuối cùng trước khi huỷ item */ },
                    maxSize: 1000
                );
                _messagePools[type] = pool;
            }
            return pool as ObjectPool<T>;
        }

        /// <summary>
        /// Publish a message to all subscribers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configure"></param>
        public void Notify<T>(Action<T> configure) where T : class, new()
        {
            var pool = GetPool<T>();
            var message = pool.Get();
            configure?.Invoke(message);
            Dispatch(message);
            pool.Release(message);
#if UNITY_EDITOR
            Debug.Log($"Message published: {typeof(T).Name}");
            _messageCounts.AddOrUpdate(typeof(T), 1, (type, count) => count + 1);
#endif
        }

        /// <summary>
        /// Dispatch a message to all subscribers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        private void Dispatch<T>(T message)
        {
            var messageType = typeof(T);
            List<Delegate> subscribersCopy;
            lock (_lock)
            {
                if (_subscribers.TryGetValue(messageType, out var subscribers))
                {
                    subscribersCopy = new List<Delegate>(subscribers);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log($"No subscribers for message type: {messageType.Name}");
#endif
                    return;
                }
            }

            foreach (Delegate subscriber in subscribersCopy)
            {
                if (subscriber is Action<T> action)
                {
                    action(message);
                }
            }
        }

        /// <summary>
        /// Subscribe to a message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public void Subscribe<T>(Action<T> subscriber) where T : class
        {
            lock (_lock)
            {

                var type = typeof(T);
                if (!_subscribers.TryGetValue(type, out var list))
                {
                    list = new List<Delegate>();
                    _subscribers[type] = list;
                }
                list.Add(subscriber);

                // #if UNITY_EDITOR
                //             if (!list.Contains(subscriber)) // Kiểm tra để tránh đăng ký trùng lặp
                //             {
                //                 Debug.LogError($"Subscriber already exists for message type: {type.Name}");
                //             }
                // #endif
            }
        }

        /// <summary>
        /// Unsubscribe from a message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Unsubscribe<T>(Action<T> subscriber) where T : class
        {
            lock (_lock)
            {
                var type = typeof(T);
                if (_subscribers.TryGetValue(type, out var list))
                {
                    list.Remove(subscriber);
                }
            }
        }

        /// <summary>
        /// Reset the message object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void ResetMessage<T>(T message) where T : class
        {
            if (message is IResettable resettable)
            {
                resettable.Reset();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Get the number of messages published for each message type
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetMessageCounts()
        {
            var counts = new Dictionary<string, int>();
            foreach (var key in _messageCounts.Keys)
            {
                counts[key.Name] = _messageCounts[key];
            }
            return counts;
        }

        /// <summary>
        /// Get the number of subscribers for each message type
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetSubscriberCounts()
        {
            var counts = new Dictionary<string, int>();
            foreach (var key in _subscribers.Keys)
            {
                counts[key.Name] = _subscribers[key].Count;
            }
            return counts;
        }
#endif
    }
    /// <summary>
    /// Interface for resettable objects
    /// </summary>
    public interface IResettable
    {
        void Reset();
    }

}
