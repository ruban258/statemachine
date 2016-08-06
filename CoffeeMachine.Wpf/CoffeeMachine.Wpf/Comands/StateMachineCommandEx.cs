using System;
using Microsoft.Practices.Prism.Commands;
using Stateless;

namespace CoffeeMachine.Wpf.Comands
{
    public static class StateMachineCommandEx
    {
        /// <summary>
        /// Creates a DelegateCommand using a trigger and a state machine.
        /// The command can be executed if the trigger can be executed on the current state machine status and the specified "CanExecute" function is null or returns true.
        /// When the command is executed the specified action is executed and then the trigger is fired
        /// </summary>
        /// <typeparam name="TState">State machine status type.</typeparam>
        /// <typeparam name="TTrigger">State machine status trigger.</typeparam>
        /// <param name="stateMachine">A state machine instance</param>
        /// <param name="trigger">A trigger.</param>
        /// <param name="execute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">The command can be executed only if this function is null or return true and the current status of the machine supports the trigger.</param>
        public static DelegateCommand CreateCommand<TState, TTrigger>(this StateMachine<TState, TTrigger> stateMachine, TTrigger trigger, Action execute = null, Func<bool> canExecute = null)
        {
            if (canExecute == null)
            {
                canExecute = () => true;
            }

            if (execute == null)
            {
                execute = delegate { };
            }

            return new DelegateCommand(
                executeMethod: delegate
                {
                    execute();
                    stateMachine.Fire(trigger);
                },
                canExecuteMethod: () => stateMachine.CanFire(trigger) && canExecute());
        }

        /// <summary>
        /// Creates a DelegateCommand using a trigger and a state machine.
        /// The command can be executed if the trigger can be executed on the current state machine status and the specified "CanExecute" function is null or returns true.
        /// When the command is executed the specified action is executed and then the trigger is fired
        /// </summary>
        /// <typeparam name="TState">State machine status type.</typeparam>
        /// <typeparam name="TTrigger">State machine status trigger.</typeparam>
        /// <typeparam name="TCommandParam">Command parameter type</typeparam>
        /// <param name="stateMachine">A state machine instance</param>
        /// <param name="trigger">A trigger.</param>
        /// <param name="execute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">The command can be executed only if this function is null or return true and the current status of the machine supports the trigger.</param>
        public static DelegateCommand<TCommandParam> CreateCommand<TState, TTrigger, TCommandParam>(this StateMachine<TState, TTrigger> stateMachine, TTrigger trigger, Action<TCommandParam> execute = null, Func<bool> canExecute = null)
        {
            if (canExecute == null)
            {
                canExecute = () => true;
            }

            if (execute == null)
            {
                execute = delegate { };
            }

            return new DelegateCommand<TCommandParam>(
                executeMethod: delegate(TCommandParam param)
                {
                    execute(param);
                    stateMachine.Fire(trigger);
                },
                canExecuteMethod: arg => stateMachine.CanFire(trigger) && canExecute());
        }
    }
}