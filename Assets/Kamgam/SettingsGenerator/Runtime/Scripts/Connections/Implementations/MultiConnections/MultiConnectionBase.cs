using System;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Please notice multi connection are experimental.
    /// The multi connections are a useful code-only tool to hook up a setting with multiple connections.
    /// However, the recommendation is to only have one connection per setting since getting default values
    /// if ambiguous if multiple connections are used. By default the first valid connection will define
    /// the default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiConnection<T> : IConnection<T>
    {
        /// <summary>
        /// If set the this will be used as the GET and DEFAULT connection.
        /// If NULL then the first valid connection will be used instead.
        /// </summary>
        public IConnection<T> DefaultConnection = null;
        
        protected List<IConnection<T>> _connections = new List<IConnection<T>>();

        public void AddConnection(IConnection<T> connection)
        {
            _connections.Add(connection);
            
            foreach (var listener in _changeListeners)
            {
                connection.AddChangeListener(listener);    
            }
        }
        
        public void RemoveConnection(IConnection<T> connection)
        {
            _connections.Remove(connection);
            
            foreach (var listener in _changeListeners)
            {
                connection.RemoveChangeListener(listener);    
            }
        }
        
        public void ClearConnections()
        {
            for (int i = _connections.Count-1; i >= 0; i--)
            {
                if (_connections[i] == null)
                    continue;
                
                RemoveConnection(_connections[i]);    
            }
            
            // Remove remaining null connections.
            _connections.Clear();
        }

        protected List<Action<T>> _changeListeners = new List<Action<T>>();

        public IConnection<T> GetDefaultConnection()
        {
            // Use defined default
            if (DefaultConnection != null)
                return DefaultConnection;
            
            // Or use first valid connection.
            foreach (var connection in _connections)
            {
                if (connection == null)
                    continue;
                
                return connection;
            }

            throw new Exception("Multi Connection has no connections. Can not get default connection.");
        }
        
        public T Get()
        {
            return GetDefaultConnection().Get();
        }

        public T GetDefault()
        {
            return Get();
        }

        public void Set(T value)
        {
            foreach (var connection in _connections)
            {
                if (connection == null)
                    continue;
                
                connection.Set(value);
            }
        }

        /// <summary>
        /// NOTICE that this will only modify the listeners of the current list of connections.
        /// </summary>
        /// <param name="listener"></param>
        public void AddChangeListener(Action<T> listener)
        {
            _changeListeners.AddIfNotContained(listener);
            
            foreach (var connection in _connections)
            {
                if (connection == null)
                    continue;
                
                connection.AddChangeListener(listener);
            }
        }

        public void RemoveChangeListener(Action<T> listener)
        {
            _changeListeners.Remove(listener);
            
            foreach (var connection in _connections)
            {
                if (connection == null)
                    continue;
                
                connection.RemoveChangeListener(listener);
            }
        }

        public void OnQualityChanged(int qualityLevel)
        {
            foreach (var connection in _connections)
            {
                if (connection == null)
                    continue;
                
                connection.OnQualityChanged(qualityLevel);
            }
        }

        public int GetOrder()
        {
            return GetDefaultConnection().GetOrder();
        }

        public void SetOrder(int order)
        {
            if (DefaultConnection != null)
                DefaultConnection.SetOrder(order);
            
            foreach (var connection in _connections)
            {
                if (connection == null)
                    continue;
                
                connection.SetOrder(order);
            }
        }
        
        public void Destroy()
        {
            ClearConnections();
            _changeListeners.Clear();
        }
    }
}