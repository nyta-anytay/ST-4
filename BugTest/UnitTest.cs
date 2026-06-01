using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;

namespace BugTests
{
    [TestClass]
    public class BugStateMachineTests
    {
        private Bug _bug;

        [TestInitialize]
        public void Setup()
        {
            _bug = new Bug();
        }

        [TestMethod]
        public void Test_01_InitialState_ShouldBeNew()
        {
            Assert.AreEqual(State.New, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_02_New_To_Triage()
        {
            _bug.Fire(Trigger.StartTriage);
            Assert.AreEqual(State.Triage, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_03_Triage_To_Fixing()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            Assert.AreEqual(State.Fixing, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_04_Triage_To_Deferred()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.Defer);
            Assert.AreEqual(State.DeferredOrMoreInfo, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_05_Triage_To_Rejected()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.Reject);
            Assert.AreEqual(State.Rejected, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_06_Deferred_BackTo_Triage()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.Defer);
            _bug.Fire(Trigger.ReturnToTriage);
            Assert.AreEqual(State.Triage, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_07_Fixing_To_Testing()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.Resolve);
            Assert.AreEqual(State.Testing, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_08_Fixing_To_ReviewCannotReproduce()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.MarkCannotReproduce);
            Assert.AreEqual(State.ReviewCannotReproduce, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_09_Testing_To_Closed_WhenVerifiedFixed()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.Resolve);
            _bug.Fire(Trigger.VerifyFixed);
            Assert.AreEqual(State.Closed, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_10_Testing_To_Triage_WhenVerifyFailed()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.Resolve);
            _bug.Fire(Trigger.VerifyFailed);
            Assert.AreEqual(State.Triage, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_11_ReviewCannotReproduce_To_Closed()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.MarkCannotReproduce);
            _bug.Fire(Trigger.ConfirmCannotReproduce);
            Assert.AreEqual(State.Closed, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_12_ReviewCannotReproduce_To_Triage_WhenRejected()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.MarkCannotReproduce);
            _bug.Fire(Trigger.ReturnToTriage);
            Assert.AreEqual(State.Triage, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_13_Closed_To_Triage_Via_Reopen()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.Resolve);
            _bug.Fire(Trigger.VerifyFixed); 
            _bug.Fire(Trigger.Reopen);      
            Assert.AreEqual(State.Triage, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_14_Rejected_To_Triage_Via_Reopen()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.Reject);
            _bug.Fire(Trigger.Reopen);
            Assert.AreEqual(State.Triage, _bug.CurrentState);
        }

        [TestMethod]
        public void Test_15_Exception_When_Assigning_New_Bug()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _bug.Fire(Trigger.AssignToDev));
        }

        [TestMethod]
        public void Test_16_Exception_When_Resolving_From_Triage()
        {
            _bug.Fire(Trigger.StartTriage);
            Assert.ThrowsException<InvalidOperationException>(() => _bug.Fire(Trigger.Resolve));
        }

        [TestMethod]
        public void Test_17_Exception_When_Reopening_New_Bug()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _bug.Fire(Trigger.Reopen));
        }

        [TestMethod]
        public void Test_18_Exception_When_Closing_Directly_From_Fixing()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            Assert.ThrowsException<InvalidOperationException>(() => _bug.Fire(Trigger.VerifyFixed));
        }

        [TestMethod]
        public void Test_19_Exception_When_Double_Rejecting()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.Reject);
            Assert.ThrowsException<InvalidOperationException>(() => _bug.Fire(Trigger.Reject));
        }

        [TestMethod]
        public void Test_20_Exception_When_Returning_Closed_Bug_To_Triage_Without_Reopen()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.AssignToDev);
            _bug.Fire(Trigger.Resolve);
            _bug.Fire(Trigger.VerifyFixed); 
            
            Assert.ThrowsException<InvalidOperationException>(() => _bug.Fire(Trigger.ReturnToTriage));
        }

        [TestMethod]
        public void Test_21_Exception_When_Verifying_From_Deferred()
        {
            _bug.Fire(Trigger.StartTriage);
            _bug.Fire(Trigger.Defer);
            Assert.ThrowsException<InvalidOperationException>(() => _bug.Fire(Trigger.VerifyFixed));
        }
    }
}