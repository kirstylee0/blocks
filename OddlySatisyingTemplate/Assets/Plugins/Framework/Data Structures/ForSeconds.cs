using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    public class ForSeconds : CustomYieldInstruction
    {
        public override bool keepWaiting => Evaluate();

        private float _startTime;
        private float _time;
        private Action<float> _function;
        private Func<bool> _continueCondition;

        public ForSeconds(float time, Action<float> function, Func<bool> continueCondition = null)
        {
            _startTime = Time.time;
            _time = time;
            _function = function;
            _continueCondition = continueCondition;
        }

        public ForSeconds(float time, Func<bool> continueCondition = null)
        {
            _startTime = Time.time;
            _time = time;
            _function = null;
            _continueCondition = continueCondition;
        }

        bool Evaluate()
        {
            if (_continueCondition != null && !_continueCondition()) return false;

            if (Time.time - _startTime >= _time)
            {
                _function?.Invoke(_time);

                return false;
            }

            _function?.Invoke(Time.time - _startTime);

            return true;
        }
    }

    public class ForSecondsRealtime : CustomYieldInstruction
    {
        public override bool keepWaiting => Evaluate();

        private float _startTime;
        private float _time;
        private Action<float> _function;
        private Func<bool> _continueCondition;

        public ForSecondsRealtime(float time, Action<float> function, Func<bool> continueCondition = null)
        {
            _startTime = Time.realtimeSinceStartup;
            _time = time;
            _function = function;
            _continueCondition = continueCondition;
        }

        public ForSecondsRealtime(float time, Func<bool> continueCondition = null)
        {
            _startTime = Time.realtimeSinceStartup;
            _time = time;
            _function = null;
            _continueCondition = continueCondition;
        }

        bool Evaluate()
        {
            if (_continueCondition != null && !_continueCondition()) return false;

            if (Time.realtimeSinceStartup - _startTime >= _time)
            {
                _function?.Invoke(_time);
                return false;
            }

            _function?.Invoke(Time.realtimeSinceStartup - _startTime);

            return true;
        }
    }

    public class ForSecondsNormalized : CustomYieldInstruction
    {
        public override bool keepWaiting => Evaluate();

        private float _startTime;
        private float _time;
        private Action<float> _function;
        private Func<bool> _continueCondition;

        public ForSecondsNormalized(float time, Action<float> function, Func<bool> continueCondition = null)
        {
            _startTime = Time.time;
            _time = time;
            _function = function;
            _continueCondition = continueCondition;
        }

        bool Evaluate()
        {
            if (_continueCondition != null && !_continueCondition()) return false;

            if (Time.time - _startTime >= _time)
            {
                _function(1f);
                return false;
            }

            _function((Time.time - _startTime) / _time);
            return true;
        }
    }

    public class ForSecondsRealtimeNormalized : CustomYieldInstruction
    {
        public override bool keepWaiting => Evaluate();

        private float _startTime;
        private float _time;
        private Action<float> _function;
        private Func<bool> _continueCondition;

        public ForSecondsRealtimeNormalized(float time, Action<float> function, Func<bool> continueCondition = null)
        {
            _startTime = Time.realtimeSinceStartup;
            _time = time;
            _function = function;
            _continueCondition = continueCondition;
        }

        bool Evaluate()
        {
            if (_continueCondition != null && !_continueCondition()) return false;

            if (Time.realtimeSinceStartup - _startTime >= _time)
            {
                _function(1f);
                return false;
            }

            _function((Time.realtimeSinceStartup - _startTime) / _time);
            return true;
        }
    }
}
