using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CoffeeMachine.Properties;
using PropertyChanged;
using Stateless;

namespace CoffeeMachine
{
    [ImplementPropertyChanged]
    public class CoffeeMachineModel : StateMachine<CoffeeMachineState, CoffeeMachineTrigger>, INotifyPropertyChanged
    {
        private const double CoffeePrice = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoffeeMachineModel"/> class.
        /// </summary>
        public CoffeeMachineModel()
            : base(CoffeeMachineState.Idle)
        {
            this.ConfigureMachine();
        }

        /// <summary>
        /// Gets the inserted money.
        /// </summary>
        public double InsertedMoney { get; private set; }

        /// <summary>
        /// Gets the preparation process.
        /// </summary>
        public double PreparationProcess { get; private set; }

        /// <summary>
        /// Inserts a coin.
        /// </summary>
        public void InsertCoin(double amount)
        {
            this.InsertedMoney += amount;
            if (this.State == CoffeeMachineState.WithMoney && this.InsertedMoney >= CoffeePrice)
            {
                this.Fire(CoffeeMachineTrigger.EnoughMoney);
            }
        }

        #region Privates

        /// <summary>
        /// Configures the machine.
        /// </summary>
        private void ConfigureMachine()
        {
            // Idle
            this.Configure(CoffeeMachineState.Idle)
                .Permit(CoffeeMachineTrigger.InsertMoney, CoffeeMachineState.WithMoney);

            // Refund money
            this.Configure(CoffeeMachineState.RefundMoney)
                .OnEntry(RefundMoney)
                .Permit(CoffeeMachineTrigger.MoneyRefunded, CoffeeMachineState.Idle);


            // WithMoney
            this.Configure(CoffeeMachineState.WithMoney)
                .PermitReentry(CoffeeMachineTrigger.InsertMoney)
                .Permit(CoffeeMachineTrigger.RefundMoney, CoffeeMachineState.RefundMoney)
                .Permit(CoffeeMachineTrigger.EnoughMoney, CoffeeMachineState.CanSelectCoffee);

            // CanSelectCoffee
            this.Configure(CoffeeMachineState.CanSelectCoffee)
                .PermitReentry(CoffeeMachineTrigger.InsertMoney)
                .Permit(CoffeeMachineTrigger.RefundMoney, CoffeeMachineState.RefundMoney)
                .Permit(CoffeeMachineTrigger.PrepareCoffee, CoffeeMachineState.PreparingCoffee);

            // PreparingCoffee
            this.Configure(CoffeeMachineState.PreparingCoffee)
                .OnEntry(PrepareCoffee)
                .Permit(CoffeeMachineTrigger.CoffeePrepared, CoffeeMachineState.CoffeeReady);

            // CoffeeReady
            this.Configure(CoffeeMachineState.CoffeeReady)
                .Permit(CoffeeMachineTrigger.TakeCoffe, CoffeeMachineState.RefundMoney);

            this.OnTransitioned(NotifyStateChanged);
        }

        /// <summary>
        /// Refunds the money.
        /// </summary>
        private void RefundMoney()
        {
            Task refundMoneyTask = new Task(() =>
            {
                while (this.InsertedMoney > 1)
                {
                    Thread.Sleep(200);
                    this.InsertedMoney = this.InsertedMoney - 1;
                }

                this.InsertedMoney = 0;

                this.Fire(CoffeeMachineTrigger.MoneyRefunded);

            });

            refundMoneyTask.Start();
        }

        /// <summary>
        /// Prepares the coffee.
        /// </summary>
        private void PrepareCoffee()
        {
            Task prepareCoffeeTask = new Task(() =>
            {
                this.InsertedMoney = this.InsertedMoney - CoffeePrice;
                while (this.PreparationProcess < 100)
                {
                    Thread.Sleep(50);
                    this.PreparationProcess += 1;
                }

                this.PreparationProcess = 0;
                this.Fire(CoffeeMachineTrigger.CoffeePrepared);
            });

            prepareCoffeeTask.Start();
        }

        private void NotifyStateChanged(Transition transition)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            this.OnPropertyChanged("State");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
