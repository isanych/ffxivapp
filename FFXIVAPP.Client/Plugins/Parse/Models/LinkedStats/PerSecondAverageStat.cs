// FFXIVAPP.Client
// PerSecondAverageStat.cs
// 
// � 2013 Ryan Wilson

using System;
using System.Runtime.Remoting;
using System.Timers;
using FFXIVAPP.Client.Plugins.Parse.Models.Stats;
using SmartAssembly.Attributes;

namespace FFXIVAPP.Client.Plugins.Parse.Models.LinkedStats
{
    [DoNotObfuscate]
    public class PerSecondAverageStat : LinkedStat
    {
        public PerSecondAverageStat(string name, params Stat<decimal>[] dependencies) : base(name, 0m)
        {
            SetupDepends(dependencies[0]);
        }

        public PerSecondAverageStat(string name, decimal value) : base(name, 0m)
        {
        }

        public PerSecondAverageStat(string name) : base(name, 0m)
        {
        }

        public Timer UpdateTimer { get; set; }

        private DateTime? FirstEventReceived { get; set; }
        private DateTime LastEventReceived { get; set; }
        private decimal LastTimeDifference { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="dependency"> </param>
        private void SetupDepends(Stat<decimal> dependency)
        {
            AddDependency(dependency);
            UpdateTimer = new Timer(1000);
            UpdateTimer.Elapsed += UpdateTimerOnElapsed;
            UpdateTimer.Start();
        }

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (ParseControl.Instance.Timeline.FightingRightNow)
            {
                var originalValue = Value * LastTimeDifference;
                UpdateDPSTick(originalValue);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="previousValue"> </param>
        /// <param name="newValue"> </param>
        public override void DoDependencyValueChanged(object sender, object previousValue, object newValue)
        {
            UpdateDPSTick((decimal) newValue);
        }

        private void UpdateDPSTick(decimal value)
        {
            if (FirstEventReceived == default(DateTime) || FirstEventReceived == null)
            {
                FirstEventReceived = Constants.Parse.PluginSettings.TrackXPSFromParseStartEvent ? ParseControl.Instance.StartTime : DateTime.Now;
            }
            LastEventReceived = DateTime.Now;
            var timeDifference = Convert.ToDecimal(LastEventReceived.Subtract((DateTime)FirstEventReceived)
                                                                    .TotalSeconds);
            if (value == 0 || timeDifference == 0)
            {
                return;
            }
            LastTimeDifference = timeDifference;
            Value = value / timeDifference;
        }
    }
}
