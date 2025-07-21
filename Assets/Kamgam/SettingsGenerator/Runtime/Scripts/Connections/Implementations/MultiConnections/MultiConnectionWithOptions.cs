using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    public class MultiConnectionWithOptions<TOption> : MultiConnection<int>, IConnectionWithOptions<TOption>
    {
        public bool HasOptions()
        {
            var options = GetOptionLabels();
            return options != null && options.Count > 0;
        }

        public List<TOption> GetOptionLabels()
        {
            var con = GetDefaultConnection() as IConnectionWithOptions<TOption>;
            return con.GetOptionLabels();
        }

        public void SetOptionLabels(List<TOption> optionLabels)
        {
            var con = GetDefaultConnection() as IConnectionWithOptions<TOption>;
            con.SetOptionLabels(optionLabels);
        }

        public void RefreshOptionLabels()
        {
            foreach (var connection in _connections)
            {
                if (connection == null)
                    continue;
                
                var con = connection as IConnectionWithOptions<TOption>;
                
                if (con == null)
                    continue;
                
                con.RefreshOptionLabels();
            }
        }
    }
}