namespace CoffeeMachine
{
    public enum CoffeeMachineTrigger
    {
        InsertMoney,
        RefundMoney,
        PrepareCoffee,
        TakeCoffe,

        // Automatic triggers
        EnoughMoney,
        CoffeePrepared,
        MoneyRefunded
    }
}