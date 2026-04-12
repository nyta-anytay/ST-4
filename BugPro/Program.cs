// BugPro/Program.cs
using Stateless;

namespace BugPro
{
    /// <summary>
    /// Состояния жизненного цикла бага
    /// </summary>
    public enum BugState
    {
        Open,           // Новый дефект
        Assigned,       // Назначен на разработчика
        InProgress,     // В работе
        Fixed,          // Исправлен
        Verified,       // Проверяется тестировщиком
        Closed,         // Закрыт
        Reopened,       // Переоткрыт
        Rejected,       // Отклонен (не дефект/не исправлять)
        Duplicate,      // Дубликат
        NeedMoreInfo    // Нужно больше информации
    }

    /// <summary>
    /// Действия (триггеры) для перехода между состояниями
    /// </summary>
    public enum BugTrigger
    {
        Assign,         // Назначить разработчику
        StartWork,      // Начать работу
        Fix,            // Исправить
        Verify,         // Проверить
        Close,          // Закрыть
        Reopen,         // Переоткрыть
        Reject,         // Отклонить
        MarkDuplicate,  // Пометить как дубликат
        RequestInfo,    // Запросить информацию
        ProvideInfo     // Предоставить информацию
    }

    public class Bug
    {
        private readonly StateMachine<BugState, BugTrigger> _machine;
        private readonly StateMachine<BugState, BugTrigger>.TriggerWithParameters<string> _assignTrigger;
        private string _assignedTo = string.Empty;

        public BugState CurrentState => _machine.State;
        public string AssignedTo => _assignedTo;

        public Bug()
        {
            _machine = new StateMachine<BugState, BugTrigger>(BugState.Open);
            
            _assignTrigger = _machine.SetTriggerParameters<string>(BugTrigger.Assign);

            ConfigureTransitions();
            
            ConfigureActions();
        }

        private void ConfigureTransitions()
        {
            
            _machine.Configure(BugState.Open)
                .Permit(BugTrigger.Assign, BugState.Assigned)
                .Permit(BugTrigger.Reject, BugState.Rejected)
                .Permit(BugTrigger.MarkDuplicate, BugState.Duplicate)
                .Permit(BugTrigger.RequestInfo, BugState.NeedMoreInfo);

            _machine.Configure(BugState.Assigned)
                .Permit(BugTrigger.StartWork, BugState.InProgress)
                .Permit(BugTrigger.Reject, BugState.Rejected)
                .Permit(BugTrigger.RequestInfo, BugState.NeedMoreInfo);

            _machine.Configure(BugState.InProgress)
                .Permit(BugTrigger.Fix, BugState.Fixed)
                .Permit(BugTrigger.Reject, BugState.Rejected);

            _machine.Configure(BugState.Fixed)
                .Permit(BugTrigger.Verify, BugState.Verified);

            _machine.Configure(BugState.Verified)
                .Permit(BugTrigger.Close, BugState.Closed)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            _machine.Configure(BugState.Closed)
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            _machine.Configure(BugState.Reopened)
                .Permit(BugTrigger.Assign, BugState.Assigned);

            _machine.Configure(BugState.Rejected)
                .Permit(BugTrigger.Assign, BugState.Open)
                .Permit(BugTrigger.ProvideInfo, BugState.Open);

            _machine.Configure(BugState.Duplicate)
                .Permit(BugTrigger.Close, BugState.Closed);

            _machine.Configure(BugState.NeedMoreInfo)
                .Permit(BugTrigger.ProvideInfo, BugState.Open)
                .Permit(BugTrigger.Reject, BugState.Rejected);
        }

        private void ConfigureActions()
        {
            _machine.OnTransitioned(transition =>
            {
                Console.WriteLine($"[Transition] {transition.Source} -> {transition.Destination} via {transition.Trigger}");
            });
        }

        public void Assign(string developer)
        {
            _assignedTo = developer;
            _machine.Fire(_assignTrigger, developer);
        }

        public void StartWork() => _machine.Fire(BugTrigger.StartWork);
        public void Fix() => _machine.Fire(BugTrigger.Fix);
        public void Verify() => _machine.Fire(BugTrigger.Verify);
        public void Close() => _machine.Fire(BugTrigger.Close);
        public void Reopen() => _machine.Fire(BugTrigger.Reopen);
        public void Reject() => _machine.Fire(BugTrigger.Reject);
        public void MarkDuplicate() => _machine.Fire(BugTrigger.MarkDuplicate);
        public void RequestInfo() => _machine.Fire(BugTrigger.RequestInfo);
        public void ProvideInfo() => _machine.Fire(BugTrigger.ProvideInfo);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Bug Workflow Demo ===\n");
            
            var bug = new Bug();
            Console.WriteLine($"Initial state: {bug.CurrentState}");
            
            bug.Assign("John Developer");
            Console.WriteLine($"After Assign: {bug.CurrentState}, Assigned to: {bug.AssignedTo}");
            
            bug.StartWork();
            Console.WriteLine($"After StartWork: {bug.CurrentState}");
            
            bug.Fix();
            Console.WriteLine($"After Fix: {bug.CurrentState}");
            
            bug.Verify();
            Console.WriteLine($"After Verify: {bug.CurrentState}");
            
            bug.Close();
            Console.WriteLine($"After Close: {bug.CurrentState}");
            
            bug.Reopen();
            Console.WriteLine($"After Reopen: {bug.CurrentState}");
            
            bug.Assign("John Developer");
            Console.WriteLine($"After Assign again: {bug.CurrentState}");
            
            Console.WriteLine("\n=== Demo Complete ===");
        }
    }
}
