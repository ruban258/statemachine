using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Threading;
using CoffeeMachine.Wpf.Comands;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using PropertyChanged;
using Stateless;

namespace CoffeeMachine.Wpf.ViewModels
{
    [ImplementPropertyChanged]
    public class ShellViewModel : BindableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
            this.UserMessage = "Ready";

            this.CoffeeMachine = new CoffeeMachineModel();
            this.CoffeeMachine.OnTransitioned(OnTransitionAction);

            InsertCoinCommand = CoffeeMachine.CreateCommand<CoffeeMachineState, CoffeeMachineTrigger, double?>(
                trigger: CoffeeMachineTrigger.InsertMoney,
                execute: param => this.CoffeeMachine.InsertCoin(param ?? 0));

            RefundMoneyCommand = CoffeeMachine.CreateCommand(CoffeeMachineTrigger.RefundMoney);

            PrepareCoffeeCommand = CoffeeMachine.CreateCommand(CoffeeMachineTrigger.PrepareCoffee);

            TakeCoffeeCommand = CoffeeMachine.CreateCommand(CoffeeMachineTrigger.TakeCoffe);

            this.CoffeeMachine.PropertyChanged += CoffeeMachineOnPropertyChanged;
        }

        /// <summary>
        /// Gets or sets the coffee machine.
        /// </summary>
        public CoffeeMachineModel CoffeeMachine { get; set; }

        /// <summary>
        /// Gets or sets the user message.
        /// </summary>
        public string UserMessage { get; set; }

        #region Commands

        public DelegateCommand<double?> InsertCoinCommand { get; set; }
        public DelegateCommand RefundMoneyCommand { get; set; }
        public DelegateCommand PrepareCoffeeCommand { get; set; }
        public DelegateCommand TakeCoffeeCommand { get; set; }

        #endregion

        #region Privates

        private void OnTransitionAction(StateMachine<CoffeeMachineState, CoffeeMachineTrigger>.Transition transition)
        {
            Debug.WriteLine("Transition from {0} to {1}, trigger = {2}.", transition.Source, transition.Destination, transition.Trigger);

            // Update the screen message.
            UpdateScreenMessage();

            // Enable or disable buttons according to the state machine current status
            this.RefreshCommands();
        }

        private void CoffeeMachineOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "InsertedMoney" ||
                propertyChangedEventArgs.PropertyName == "PreparationProcess")
            {
                this.UpdateScreenMessage();
            }
        }

        /// <summary>
        /// Updates the screen message.
        /// </summary>
        private void UpdateScreenMessage()
        {
            switch (this.CoffeeMachine.State)
            {
                case CoffeeMachineState.Idle:
                    this.UserMessage = "Ready";
                    break;
                case CoffeeMachineState.RefundMoney:
                    this.UserMessage = "Refunding money...";
                    break;
                case CoffeeMachineState.WithMoney:
                case CoffeeMachineState.CanSelectCoffee:
                    this.UserMessage = string.Empty;
                    break;
                case CoffeeMachineState.PreparingCoffee:
                    this.UserMessage = string.Format("Preparing coffee {0} %...", this.CoffeeMachine.PreparationProcess);
                    break;
                case CoffeeMachineState.CoffeeReady:
                    this.UserMessage = "You coffee is ready!";
                    break;
                default:
                    this.UserMessage = "Out of order";
                    break;
            }
        }

        private void RefreshCommands()
        {
            InsertCoinCommand.RaiseCanExecuteChanged();
            RefundMoneyCommand.RaiseCanExecuteChanged();
            PrepareCoffeeCommand.RaiseCanExecuteChanged();
            TakeCoffeeCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}

