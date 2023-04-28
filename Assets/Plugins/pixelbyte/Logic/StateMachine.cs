using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixelbyte
{
    public class StateMachine
    {
        MonoBehaviour mb;

        public StateMachine(MonoBehaviour behavior)
        {
            mb = behavior;
        }

        protected Func<IEnumerator> state, prevState;
        public Func<IEnumerator> State
        {
            get { return state; }
            set
            {
                //If the current state is dying, the next state must be Death
                //if (state == Dying && value != Dead) return;
                prevState = state;
                state = value;
                if (state == null) prevState = null;
            }
        }

        Coroutine co;
        IEnumerator Machine()
        {
            while (state != null)
            {
                var enumerator = state();
                while (enumerator.MoveNext()) yield return enumerator.Current;
            }
        }

        public void StartMachine(Func<IEnumerator> startState)
        {
            state = startState;
            co = mb.StartCoroutine(Machine());
        }

        public void StopMachine()
        {
            if (mb != null)
                mb.StopCoroutine(co);
        }

        /// <summary>
        /// Only transitions to the next state if State == currentState
        /// </summary>
        /// <param name="currentState"></param>
        /// <param name="nextState"></param>
        /// <returns></returns>
        public bool SafeTransition(Func<IEnumerator> currentState, Func<IEnumerator> nextState)
        {
            if (State == currentState)
            {
                State = nextState;
                return true;
            }
            else return false;
        }
    } 
}
