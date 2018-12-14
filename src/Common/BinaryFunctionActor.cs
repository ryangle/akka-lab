using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;

namespace Common
{
    public class BinaryFunctionActor : ReceiveActor
    {
        public class Arguments
        {
            /// <summary>
            /// 左参数
            /// </summary>
            public double LeftArg { get; set; }
            /// <summary>
            /// 右参数
            /// </summary>
            public double RightArg { get; set; }
            /// <summary>
            /// 函数名 add,subtract,multiply,divide
            /// </summary>
            public string Function { get; set; }
        }
        public class Result
        {
            public int Code { get; set; }
            public string Info { get; set; }
            public double Data { get; set; }
        }

        public BinaryFunctionActor()
        {
            Receive<ServiceDiscovery>(msg =>
            {
                ConsoleLog($"BinaryFunctionActor Receive ServiceDiscovery");
                Sender.Tell(msg);
            });
            Receive<Arguments>(msg =>
            {
                ConsoleLog($"{DateTime.Now.ToString()} BinaryFunctionActor Receive LeftArg:{msg.LeftArg},RightArg:{msg.RightArg},Function:{msg.Function}");
                switch (msg.Function)
                {
                    case "add":
                        Sender.Tell(new Result { Data = msg.LeftArg + msg.RightArg });
                        break;
                    case "subtract":
                        Sender.Tell(new Result { Data = msg.LeftArg - msg.RightArg });
                        break;
                    case "multiply":
                        Sender.Tell(new Result { Data = msg.LeftArg * msg.RightArg });
                        break;
                    case "divide":
                        if (msg.RightArg == 0)
                        {
                            Sender.Tell(new Result { Code = -1, Info = "参数错误" });
                        }
                        else
                        {
                            Sender.Tell(new Result { Data = msg.LeftArg / msg.RightArg });
                        }
                        break;
                    default:
                        Sender.Tell(new Result { Code = 2, Info = "找不到 Function" });
                        break;
                }
            });
        }
        private void ConsoleLog(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

    }
}
