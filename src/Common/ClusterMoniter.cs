using System;
using Akka.Actor;
using Akka.Cluster;

namespace Common
{
    public class ClusterMoniter : ReceiveActor
    {
        protected Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);
        public ClusterMoniter()
        {
            Receive<ClusterEvent.ClusterShuttingDown>(msg =>
            {
                ConsoleLog($"ClusterShuttingDown");
            });

            Receive<ClusterEvent.CurrentClusterState>(msg =>
            {
                ConsoleLog($"MemberExited,Leader:{msg.Leader},{msg.Unreachable.Count}");
            });

            Receive<ClusterEvent.RoleLeaderChanged>(msg =>
            {
                ConsoleLog($"RoleLeaderChanged,Leader:{msg.Leader},Role:{msg.Role}");
            });

            #region MemberStatusChange
            Receive<ClusterEvent.MemberExited>(msg =>
            {
                ConsoleLog($"MemberExited,Member Address: {msg.Member.Address}");
            });
            Receive<ClusterEvent.MemberJoined>(msg =>
            {
                ConsoleLog($"MemberJoined,Member Address: {msg.Member.Address}");
            });
            Receive<ClusterEvent.MemberLeft>(msg =>
            {
                ConsoleLog($"MemberLeft,Member Address:{msg.Member.Address}");
            });
            Receive<ClusterEvent.MemberRemoved>(msg =>
            {
                ConsoleLog($"MemberRemoved,Member Address: {msg.Member.Address}");
            });
            Receive<ClusterEvent.MemberUp>(msg =>
            {
                ConsoleLog($"MemberUp,Member Address:{msg.Member.Address}");
            });
            Receive<ClusterEvent.MemberWeaklyUp>(msg =>
            {
                ConsoleLog($"MemberWeaklyUp,Member Address: {msg.Member.Address}");
            });
            Receive<ClusterEvent.MemberStatusChange>(msg =>
            {
                ConsoleLog($"MemberStatusChange,Member Address: {msg.Member.Address}");
            });
            #endregion

            #region ReachabilityEvent
            Receive<ClusterEvent.UnreachableMember>(msg =>
            {
                Cluster.Down(msg.Member.Address);
                ConsoleLog($"UnreachableMember,Member Address:{msg.Member.Address}");
            });
            Receive<ClusterEvent.ReachabilityEvent>(msg =>
            {
                ConsoleLog($"ReachabilityEvent,Member Address:{msg.Member.Address}");
            });
            #endregion

        }
        protected override void PreStart()
        {
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.IClusterDomainEvent) });
        }
        private void ConsoleLog(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}