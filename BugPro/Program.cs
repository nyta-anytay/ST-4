using System;
using Stateless;

namespace BugPro
{
    public enum State 
    { 
        New,                    
        Triage,                 
        DeferredOrMoreInfo,     
        Fixing,                 
        ReviewCannotReproduce,  
        Testing,                
        Closed,                 
        Rejected                
    }

    public enum Trigger 
    { 
        Report,                 
        StartTriage,            
        AssignToDev,            
        Defer,                  
        ReturnToTriage,         
        Reject,                 
        MarkCannotReproduce,    
        ConfirmCannotReproduce, 
        Resolve,                
        VerifyFixed,            
        VerifyFailed,           
        Reopen                  
    }

    public class Bug
    {
        private readonly StateMachine<State, Trigger> _machine;

        public State CurrentState => _machine.State;

        public Bug()
        {
            _machine = new StateMachine<State, Trigger>(State.New);

            _machine.Configure(State.New)
                .Permit(Trigger.StartTriage, State.Triage);

            _machine.Configure(State.Triage)
                .Permit(Trigger.AssignToDev, State.Fixing)
                .Permit(Trigger.Defer, State.DeferredOrMoreInfo)
                .Permit(Trigger.Reject, State.Rejected);

            _machine.Configure(State.DeferredOrMoreInfo)
                .Permit(Trigger.ReturnToTriage, State.Triage);

            _machine.Configure(State.Fixing)
                .Permit(Trigger.Resolve, State.Testing)
                .Permit(Trigger.MarkCannotReproduce, State.ReviewCannotReproduce);

            _machine.Configure(State.ReviewCannotReproduce)
                .Permit(Trigger.ConfirmCannotReproduce, State.Closed) 
                .Permit(Trigger.ReturnToTriage, State.Triage);        

            _machine.Configure(State.Testing)
                .Permit(Trigger.VerifyFixed, State.Closed)            
                .Permit(Trigger.VerifyFailed, State.Triage);          

            _machine.Configure(State.Closed)
                .Permit(Trigger.Reopen, State.Triage);                

            
            _machine.Configure(State.Rejected)
                .Permit(Trigger.Reopen, State.Triage);
        }

        public void Fire(Trigger trigger)
        {
            _machine.Fire(trigger);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрация работы конечного автомата Bug (Stateless)\n");
            
            var bug = new Bug();
            Console.WriteLine($"1. Исходное состояние: {bug.CurrentState}");
            
            bug.Fire(Trigger.StartTriage);
            Console.WriteLine($"2. После создания: {bug.CurrentState}");
            
            bug.Fire(Trigger.AssignToDev);
            Console.WriteLine($"3. Взято в работу: {bug.CurrentState}");
            
            bug.Fire(Trigger.Resolve);
            Console.WriteLine($"4. Разработчик исправил: {bug.CurrentState}");
            
            bug.Fire(Trigger.VerifyFixed);
            Console.WriteLine($"5. Тестировщик проверил: {bug.CurrentState}");

            Console.WriteLine("\nWorkflow успешно завершен! Нажмите любую клавишу...");
        }
    }
}