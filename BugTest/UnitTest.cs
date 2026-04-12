// BugTests/UnitTest1.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using Stateless;

namespace BugTests
{
    [TestClass]
    public class BugWorkflowTests
    {
        [TestMethod]
        public void Test_InitialState_ShouldBeOpen()
        {
            
            var bug = new Bug();
            
            Assert.AreEqual(BugState.Open, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Assign_ShouldTransitionToAssigned()
        {
            
            var bug = new Bug();
            
            bug.Assign("John");
            
            Assert.AreEqual(BugState.Assigned, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Assign_ShouldStoreDeveloperName()
        {
            
            var bug = new Bug();
            string developer = "Jane Developer";
            
            bug.Assign(developer);
            
            Assert.AreEqual(developer, bug.AssignedTo);
        }

        [TestMethod]
        public void Test_StartWork_FromAssigned_ShouldTransitionToInProgress()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            
            bug.StartWork();
            
            Assert.AreEqual(BugState.InProgress, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Fix_FromInProgress_ShouldTransitionToFixed()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            
            bug.Fix();
            
            Assert.AreEqual(BugState.Fixed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Verify_FromFixed_ShouldTransitionToVerified()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            bug.Fix();
            
            bug.Verify();
            
            Assert.AreEqual(BugState.Verified, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Close_FromVerified_ShouldTransitionToClosed()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            bug.Fix();
            bug.Verify();
            
            bug.Close();
            
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Reopen_FromClosed_ShouldTransitionToReopened()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            bug.Fix();
            bug.Verify();
            bug.Close();
            
            bug.Reopen();
            
            Assert.AreEqual(BugState.Reopened, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Reject_FromOpen_ShouldTransitionToRejected()
        {
            
            var bug = new Bug();
            
            bug.Reject();
            
            Assert.AreEqual(BugState.Rejected, bug.CurrentState);
        }

        [TestMethod]
        public void Test_MarkDuplicate_FromOpen_ShouldTransitionToDuplicate()
        {
            
            var bug = new Bug();
            
            bug.MarkDuplicate();
            
            Assert.AreEqual(BugState.Duplicate, bug.CurrentState);
        }

        [TestMethod]
        public void Test_RequestInfo_FromOpen_ShouldTransitionToNeedMoreInfo()
        {
            
            var bug = new Bug();
            
            bug.RequestInfo();
            
            Assert.AreEqual(BugState.NeedMoreInfo, bug.CurrentState);
        }

        [TestMethod]
        public void Test_ProvideInfo_FromNeedMoreInfo_ShouldTransitionToOpen()
        {
            
            var bug = new Bug();
            bug.RequestInfo();
            
            bug.ProvideInfo();
            
            Assert.AreEqual(BugState.Open, bug.CurrentState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_StartWork_FromOpen_ShouldThrowException()
        {
            
            var bug = new Bug();
            
            bug.StartWork();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Fix_FromAssigned_ShouldThrowException()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            
            bug.Fix();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Verify_FromInProgress_ShouldThrowException()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            
            bug.Verify();
        }

        [TestMethod]
        public void Test_Reopen_FromReopened_ShouldStayReopened()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            bug.Fix();
            bug.Verify();
            bug.Close();
            bug.Reopen();
            
        }

        [TestMethod]
        public void Test_CompleteWorkflow_ShouldEndInClosed()
        {
            
            var bug = new Bug();
            
            bug.Assign("John");
            bug.StartWork();
            bug.Fix();
            bug.Verify();
            bug.Close();
            
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_RejectFromAssigned_ShouldTransitionToRejected()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            
            bug.Reject();
            
            Assert.AreEqual(BugState.Rejected, bug.CurrentState);
        }

        [TestMethod]
        public void Test_RequestInfo_FromAssigned_ShouldTransitionToNeedMoreInfo()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            
            bug.RequestInfo();
            
            Assert.AreEqual(BugState.NeedMoreInfo, bug.CurrentState);
        }

        [TestMethod]
        public void Test_RejectedToOpenViaAssign()
        {
            
            var bug = new Bug();
            bug.Reject();
            Assert.AreEqual(BugState.Rejected, bug.CurrentState);
            
            bug.Assign("John");
            
            Assert.AreEqual(BugState.Open, bug.CurrentState);
        }

        [TestMethod]
        public void Test_DuplicateToClosed()
        {
            
            var bug = new Bug();
            bug.MarkDuplicate();
            Assert.AreEqual(BugState.Duplicate, bug.CurrentState);
            
            bug.Close();
            
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_ReopenFromVerified_ShouldTransitionToReopened()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            bug.Fix();
            bug.Verify();
            
            bug.Reopen();
            
            Assert.AreEqual(BugState.Reopened, bug.CurrentState);
        }

        [TestMethod]
        public void Test_ReopenedToAssigned()
        {
            
            var bug = new Bug();
            bug.Assign("John");
            bug.StartWork();
            bug.Fix();
            bug.Verify();
            bug.Close();
            bug.Reopen();
            Assert.AreEqual(BugState.Reopened, bug.CurrentState);
            
            bug.Assign("John");
            
            Assert.AreEqual(BugState.Assigned, bug.CurrentState);
        }
    }
}
