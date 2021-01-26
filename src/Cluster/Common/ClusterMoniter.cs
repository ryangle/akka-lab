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
                ConsoleLog($"RemotingErrorEvent {msg.Cause}", ConsoleColor.Green);
            });
            Receive<QuarantinedEvent>(msg =>
            {
                ConsoleLog($"QuarantinedEvent {msg.Address}", ConsoleColor.Green);
            });
            Receive<ThisActorSystemQuarantinedEvent>(msg =>
            {
                ConsoleLog($"ThisActorSystemQuarantinedEvent", ConsoleColor.Green);
            });
            #endregion

            Receive<ClusterEvent.ClusterShuttingDown>(msg =>
            {
                ConsoleLog($"ClusterShuttingDown {ClusterEvent.ClusterShuttingDown.Instance}");
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
                ConsoleLog($"MemberExited,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address: {msg.Member.Address},{msg.Member.UniqueAddress}", ConsoleColor.DarkRed);
            });
            Receive<ClusterEvent.MemberJoined>(msg =>
            {
                ConsoleLog($"MemberJoined,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address: {msg.Member.Address},{msg.Member.UniqueAddress}", ConsoleColor.DarkGreen);
            });
            Receive<ClusterEvent.MemberLeft>(msg =>
            {
                ConsoleLog($"MemberLeft,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address:{msg.Member.Address},{msg.Member.UniqueAddress}", ConsoleColor.Red);
            });
            Receive<ClusterEvent.MemberRemoved>(msg =>
            {
                ConsoleLog($"MemberRemoved,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address: {msg.Member.Address},{msg.Member.UniqueAddress},Roles:{msg.Member.Roles.Join(",")}", ConsoleColor.Yellow);
            });
            Receive<ClusterEvent.MemberUp>(msg =>
            {
                ConsoleLog($"MemberUp,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address:{msg.Member.Address},{msg.Member.UniqueAddress}", ConsoleColor.Green);
            });
            Receive<ClusterEvent.MemberWeaklyUp>(msg =>
            {
                ConsoleLog($"MemberWeaklyUp,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address: {msg.Member.Address},{msg.Member.UniqueAddress}", ConsoleColor.Black);
            });
            Receive<ClusterEvent.MemberStatusChange>(msg =>
            {
                ConsoleLog($"MemberStatusChange,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address: {msg.Member.Address},{msg.Member.UniqueAddress}", ConsoleColor.DarkBlue);
            });
            #endregion

            #region ReachabilityEvent
            Receive<ClusterEvent.UnreachableMember>(msg =>
            {
                //Cluster.Down(msg.Member.Address);
                ConsoleLog($"UnreachableMember,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address:{msg.Member.Address}");
            });
            Receive<ClusterEvent.ReachabilityEvent>(msg =>
            {
                ConsoleLog($"ReachabilityEvent,Roles:{msg.Member.Roles.Join(",")},Status:{msg.Member.Status},Member Address:{msg.Member.Address}");
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