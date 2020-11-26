using System;
using Akka.Actor;
using Akka.Cluster;
using Akka.Remote;
using Akka.Util.Internal;

namespace Common
{
    public class ClusterMoniter : ReceiveActor
    {
        protected Cluster Cluster = Akka.Cluster.Cluster.Get(Context.System);
        public ClusterMoniter()
        {
            #region RemotingLifecycleEvent
            Receive<RemotingListenEvent>(msg =>
            {
                ConsoleLog($"RemotingListenEvent", ConsoleColor.Green);
            });
            Receive<RemotingShutdownEvent>(msg =>
            {
                ConsoleLog($"RemotingShutdownEvent", ConsoleColor.Green);
            });
            Receive<RemotingErrorEvent>(msg =>
            {
                ConsoleLog($"RemotingErrorEvent", ConsoleColor.Green);
            });
            Receive<QuarantinedEvent>(msg =>
            {
                ConsoleLog($"QuarantinedEvent", ConsoleColor.Green);
            });
            Receive<ThisActorSystemQuarantinedEvent>(msg =>
            {
                ConsoleLog($"ThisActorSystemQuarantinedEvent", ConsoleColor.Green);
            });
            #endregion

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
                ConsoleLog($"MemberExited,Member Address: {msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(",")}", ConsoleColor.DarkRed);
            });
            Receive<ClusterEvent.MemberJoined>(msg =>
            {
                ConsoleLog($"MemberJoined,Member Address: {msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(",")}", ConsoleColor.DarkRed);
            });
            Receive<ClusterEvent.MemberLeft>(msg =>
            {
                ConsoleLog($"MemberLeft,Member Address:{msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(", ")}", ConsoleColor.DarkRed);
            });
            Receive<ClusterEvent.MemberRemoved>(msg =>
            {
                ConsoleLog($"MemberRemoved,Member Address: {msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(",")}", ConsoleColor.DarkRed);
            });
            Receive<ClusterEvent.MemberUp>(msg =>
            {
                ConsoleLog($"MemberUp,Member Address:{msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(",")}", ConsoleColor.DarkRed);
            });
            Receive<ClusterEvent.MemberWeaklyUp>(msg =>
            {
                ConsoleLog($"MemberWeaklyUp,Member Address: {msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(",")}", ConsoleColor.DarkRed);
            });
            Receive<ClusterEvent.MemberStatusChange>(msg =>
            {
                ConsoleLog($"MemberStatusChange,Member Address: {msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(",")}", ConsoleColor.DarkRed);
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

            Context.System.EventStream.Subscribe(Self, typeof(RemotingLifecycleEvent));
        }
        private void ConsoleLog(string msg, ConsoleColor color = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}